using Pubg.Net;
using PubgStatistic.Contracts.Records;

namespace PubgStatistic.PubgApi
{
    public class PubgManager : IPubgManager
    {
        private readonly string _apiKey;
        private readonly string _playerId;

        public PubgManager(string apiKey, string playerId)
        {
            _apiKey = apiKey;
            _playerId = playerId;
        }

        public async IAsyncEnumerable<PlayerStatistic> GetStatisticFromDate(DateTimeOffset fromDate)
        {
            IDictionary<string, IList<PubgParticipantStats>> playerMatchesStatistic = new Dictionary<string, IList<PubgParticipantStats>>();
            await foreach (var match in GetMatches())
            {
                if (DateTimeOffset.Parse(match.CreatedAt) < fromDate)
                {
                    continue;
                }

                UpdateParticipantsDictionaryFromMatch(match, playerMatchesStatistic);
            }
            
            foreach (var statistic in playerMatchesStatistic)
            {
                yield return GetPlayerStatistic(statistic.Value);
            }
        }

        public async IAsyncEnumerable<PlayerStatistic> GetStatisticForNumberOfMatches(int numberOfMatches)
        {
            IDictionary<string, IList<PubgParticipantStats>> playerMatchesStatistic = new Dictionary<string, IList<PubgParticipantStats>>();
            await foreach (var match in GetMatches().OrderByDescending(x => x.CreatedAt).Take(numberOfMatches))
            {
                UpdateParticipantsDictionaryFromMatch(match, playerMatchesStatistic);
            }

            foreach (var statistic in playerMatchesStatistic)
            {
                yield return GetPlayerStatistic(statistic.Value);
            }
        }

        private async IAsyncEnumerable<PubgMatch> GetMatches()
        {
            var player = await new PubgPlayerService(_apiKey).GetPlayerAsync(PubgPlatform.Steam, _playerId);

            var matchService = new PubgMatchService(_apiKey);
            foreach (var matchId in player.MatchIds)
            {
                yield return await matchService.GetMatchAsync(PubgPlatform.Steam, matchId);
            }
        }

        private void UpdateParticipantsDictionaryFromMatch(PubgMatch match, IDictionary<string, IList<PubgParticipantStats>> playerMatchesStatistic)
        {
            var myRoster = match.Rosters.FirstOrDefault(x => x.Participants.Select(y => y.Stats.PlayerId).Contains(_playerId));
            if (myRoster == null)
            {
                throw new NullReferenceException($"Can't find player with id {_playerId} in match with id {match.Id}");
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
    }
}
