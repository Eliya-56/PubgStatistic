using System.Runtime.CompilerServices;
using Pubg.Net;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.Contracts.Records;

namespace PubgStatistic.PubgApi
{
    public class PubgManager : IPubgManager
    {
        private string? _apiKey;
        
        public string ApiKey
        {
            private get
            {
                if (_apiKey == null)
                {
                    throw new NullReferenceException($"{nameof(ApiKey)} is not set");
                }

                return _apiKey;
            }
            set => _apiKey = value;
        }

        public async IAsyncEnumerable<PlayerStatistic> GetStatisticFromDate(
            string playerName,
            DateTimeOffset fromDate, 
            [EnumeratorCancellation]CancellationToken ct = default)
        {
            var player = await GetPlayer(playerName, ct);
            if (player == null)
            {
                throw new NullReferenceException($"Can't find player with name: {playerName}");
            }

            IDictionary<string, IList<PubgParticipantStats>> playerMatchesStatistic = new Dictionary<string, IList<PubgParticipantStats>>();
            await foreach (var match in GetMatches(player, ct))
            {
                if (DateTimeOffset.Parse(match.CreatedAt) < fromDate)
                {
                    continue;
                }

                UpdateParticipantsDictionaryFromMatch(player.Id, match, playerMatchesStatistic);
            }

            foreach (var statistic in playerMatchesStatistic)
            {
                yield return GetPlayerStatistic(statistic.Value);
            }
        }

        public async IAsyncEnumerable<PlayerStatistic> GetStatisticForNumberOfMatches(
            string playerName, 
            int numberOfMatches,
            [EnumeratorCancellation] CancellationToken ct = default)
        {
            var player = await GetPlayer(playerName, ct);
            if (player == null)
            {
                throw new NullReferenceException($"Can't find player with name: {playerName}");
            }

            IDictionary<string, IList<PubgParticipantStats>> playerMatchesStatistic = new Dictionary<string, IList<PubgParticipantStats>>();
            await foreach (var match in GetMatches(player, ct).OrderByDescending(x => x.CreatedAt).Take(numberOfMatches).WithCancellation(ct))
            {
                UpdateParticipantsDictionaryFromMatch(player.Id, match, playerMatchesStatistic);
            }

            foreach (var statistic in playerMatchesStatistic)
            {
                yield return GetPlayerStatistic(statistic.Value);
            }
        }

        private async IAsyncEnumerable<PubgMatch> GetMatches(
            PubgPlayer player, 
            [EnumeratorCancellation]CancellationToken ct = default)
        {
            var matchService = new PubgMatchService(ApiKey);
            foreach (var matchId in player.MatchIds)
            {
                yield return await matchService.GetMatchAsync(PubgPlatform.Steam, matchId, cancellationToken: ct);
            }
        }

        private void UpdateParticipantsDictionaryFromMatch(
            string playerId,
            PubgMatch match,
            IDictionary<string, IList<PubgParticipantStats>> playerMatchesStatistic)
        {
            var myRoster = match.Rosters.FirstOrDefault(x => x.Participants.Select(y => y.Stats.PlayerId).Contains(playerId));
            if (myRoster == null)
            {
                throw new NullReferenceException($"Can't find player with id {playerId} in match with id {match.Id}");
            }

            foreach (var participant in myRoster.Participants)
            {
                if (playerMatchesStatistic.ContainsKey(participant.Stats.PlayerId))
                {
                    playerMatchesStatistic[participant.Stats.PlayerId].Add(participant.Stats);
                }
                else
                {
                    playerMatchesStatistic.Add(participant.Stats.PlayerId, new List<PubgParticipantStats> { participant.Stats });
                }
            }
        }

        private static PlayerStatistic GetPlayerStatistic(ICollection<PubgParticipantStats> matchStatistics)
        {
            var name = matchStatistics.First().Name;
            var kills = matchStatistics.Sum(x => x.Kills);
            var damage = matchStatistics.Sum(x => x.DamageDealt);
            var headShotsKills = matchStatistics.Sum(x => x.HeadshotKills);
            var revives = matchStatistics.Sum(x => x.Revives);
            var walkDistance = matchStatistics.Sum(x => x.WalkDistance);
            var vehiclesDistance = matchStatistics.Sum(x => x.RideDistance);
            var swimDistance = matchStatistics.Sum(x => x.SwimDistance);

            return new PlayerStatistic(
                name,
                matchStatistics.Count,
                kills,
                (double)kills / matchStatistics.Count,
                damage,
                damage / matchStatistics.Count,
                headShotsKills,
                (double)headShotsKills / matchStatistics.Count,
                revives,
                (double)revives / matchStatistics.Count,
                walkDistance,
                walkDistance / matchStatistics.Count,
                vehiclesDistance,
                vehiclesDistance / matchStatistics.Count,
                swimDistance,
                swimDistance / matchStatistics.Count
            );
        }

        private async Task<PubgPlayer?> GetPlayer(string name, CancellationToken ct = default)
        {
            var players = await new PubgPlayerService(ApiKey)
                .GetPlayersAsync(
                    PubgPlatform.Steam,
                    new GetPubgPlayersRequest()
                    {
                        PlayerNames = new[] { name }
                    }, ct);
            return players.FirstOrDefault();
        }
    }
}
