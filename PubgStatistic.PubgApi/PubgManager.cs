using Pubg.Net;
using PubgStatistic.Core;
using PubgStatistic.Core.Records;

namespace PubgStatistic.PubgApi
{
    public class PubgManager
    {
        private readonly string _apiKey;
        private readonly string _playerId;
        private readonly DateTimeOffset _startDate;

        public PubgManager(string apiKey, string playerId, DateTimeOffset startDate)
        {
            _apiKey = apiKey;
            _playerId = playerId;
            _startDate = startDate;
        }

        public async IAsyncEnumerable<PubgMatch> GetMatches()
        {
            var player = await new PubgPlayerService(_apiKey).GetPlayerAsync(PubgPlatform.Steam, _playerId);

            var matchService = new PubgMatchService(_apiKey);
            foreach (var matchId in player.MatchIds)
            {
                var match = await matchService.GetMatchAsync(PubgPlatform.Steam, matchId);
                var createdTime = DateTimeOffset.Parse(match.CreatedAt);
                if (createdTime > _startDate)
                {
                    yield return match;
                }
            }
        }

        public async Task<IList<PlayerStatistic>> GetStatistic(IAsyncEnumerable<PubgMatch> matches)
        {

            IDictionary<string, IList<PlayerMatchStatistic>> playersStatistic = new Dictionary<string, IList<PlayerMatchStatistic>>();
            await foreach (var match in matches)
            {
                await foreach (var playerMatch in GetMatchStatistic(match))
                {
                    if (playersStatistic.ContainsKey(playerMatch.PlayerId))
                    {
                        playersStatistic[playerMatch.PlayerId].Add(playerMatch);
                    }
                    else
                    {
                        playersStatistic.Add(playerMatch.PlayerId, new List<PlayerMatchStatistic> { playerMatch });
                    }
                }
            }

            if (playersStatistic.Count <= 0)
            {
                return new List<PlayerStatistic>();
            }

            return playersStatistic.Select(x => GetPlayerOverallStatistic(x.Value)).ToList();
        }

        public async IAsyncEnumerable<PlayerMatchStatistic> GetMatchStatistic(PubgMatch match)
        {
            var myRoster = match.Rosters.First(x => x.Participants.Select(x => x.Stats.PlayerId).Contains(_playerId));

            var grouped = myRoster.Participants.GroupBy(x => x.Stats.PlayerId);

            foreach (var group in grouped)
            {
                var firstItemStats = group.First().Stats;
                var name = firstItemStats.Name;
                var kills = firstItemStats.Kills;
                var damage = firstItemStats.DamageDealt;
                var headShotsKills = firstItemStats.HeadshotKills;
                var revives = firstItemStats.Revives;
                var walkDistance = firstItemStats.WalkDistance;
                var vehiclesDistance = firstItemStats.RideDistance;
                var swimDistance = firstItemStats.SwimDistance;

                yield return new PlayerMatchStatistic(
                    firstItemStats.PlayerId,
                    name,
                    kills,
                    damage,
                    headShotsKills,
                    revives,
                    walkDistance,
                    vehiclesDistance,
                    swimDistance
                );
            }
        }
        
        private PlayerStatistic GetPlayerOverallStatistic(IList<PlayerMatchStatistic> matchStatistics)
        {
            var name = matchStatistics.First().Name;
            var kills = matchStatistics.Sum(x => x.Kills);
            var damage = matchStatistics.Sum(x => x.Damage);
            var headShotsKills = matchStatistics.Sum(x => x.HeadShotsKills);
            var revives = matchStatistics.Sum(x => x.Revives);
            var walkDistance = matchStatistics.Sum(x => x.WalkDistance);
            var vehiclesDistance = matchStatistics.Sum(x => x.VehiclesDistance);
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
