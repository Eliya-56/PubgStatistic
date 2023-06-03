using PubgStatistic.Contracts;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.Contracts.Records;

namespace PubgStatistic.ApplicationLogic
{
    public class StatisticManager : IStatisticManager
    {
        private readonly IPubgManager _pubgManager;
        private readonly IDiscordManager _discordManager;
        private readonly ILogger _logger;
        private readonly IUserInfoProvider _userInfoProvider;

        private bool _sessionIsInProgress = false;

        public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(2);

        public StatisticManager(
            IPubgManager pubgManager,
            IDiscordManager discordManager,
            ILogger logger,
            IUserInfoProvider userInfoProvider)
        {
            _pubgManager = pubgManager;
            _discordManager = discordManager;
            _logger = logger;
            _userInfoProvider = userInfoProvider;
        }

        public async Task SendStatisticSnapshotAsync(int numberOfGames, CancellationToken ct = default)
        {
            await _logger.LogMessageAsync($"Start send statistic snapshot task for last {numberOfGames} games");

            _discordManager.WebhookUrl = _userInfoProvider.DiscordWebhookUrl;
            _pubgManager.ApiKey = _userInfoProvider.PubgApiKey;

            await _logger.LogMessageAsync("Getting statistic from pubg server");
            var stats = await _pubgManager.GetStatisticForNumberOfMatches(_userInfoProvider.UserName, numberOfGames, ct).ToListAsync(ct);

            await _logger.LogMessageAsync($"Send statistic to discord");
            await _discordManager.SendPubgStatisticAsync(stats);
        }

        public async Task SendStatisticSnapshotAsync(DateTimeOffset startDate, CancellationToken ct = default)
        {
            await _logger.LogMessageAsync($"Start send statistic snapshot task from date {startDate:f}");

            _discordManager.WebhookUrl = _userInfoProvider.DiscordWebhookUrl;
            _pubgManager.ApiKey = _userInfoProvider.PubgApiKey;


            await _logger.LogMessageAsync($"Getting statistic from pubg server");
            var stats = await _pubgManager.GetStatisticFromDate(_userInfoProvider.UserName, startDate, ct).ToListAsync(ct);

            await _logger.LogMessageAsync($"Send statistic to discord");
            await _discordManager.SendPubgStatisticAsync(stats);
        }

        public async Task StartNewGameSessionAsync(CancellationToken ct = default)
        {
            if (_sessionIsInProgress)
            {
                await _logger.LogErrorAsync("Session is already in progress");
                return;
            }

            _discordManager.WebhookUrl = _userInfoProvider.DiscordWebhookUrl;
            _pubgManager.ApiKey = _userInfoProvider.PubgApiKey;

            var smallRequestIntervalSeconds = 60;
            var largeRequestIntervalSeconds = 1200;
            var waitTimeSeconds = 0;

            await _logger.LogMessageAsync("Start new game session");
            _sessionIsInProgress = true;
            var startDateTime = DateTimeOffset.UtcNow;
            ulong? messageId = null;
            IList<PlayerStatistic>? lastStatistics = null;

            try
            {
                do
                {
                    if (ct.IsCancellationRequested)
                    {
                        break;
                    }

                    await _logger.LogMessageAsync("Try get statistic from pubg server");
                    var newStatistic = await _pubgManager.GetStatisticFromDate(_userInfoProvider.UserName, startDateTime, ct).ToListAsync(ct);

                    if (StatisticChanged(lastStatistics, newStatistic))
                    {
                        await _logger.LogMessageAsync("Statistic changed. Try to send statistic to discord");
                        messageId = await UpdateStatisticInDiscord(newStatistic, messageId);
                        lastStatistics = newStatistic;

                        await _logger.LogMessageAsync($"Wait for {largeRequestIntervalSeconds} second");
                        await Task.Delay(TimeSpan.FromSeconds(largeRequestIntervalSeconds), ct);
                        waitTimeSeconds += largeRequestIntervalSeconds;
                    }
                    else
                    {
                        await _logger.LogMessageAsync($"Wait for {smallRequestIntervalSeconds} second");
                        await Task.Delay(TimeSpan.FromSeconds(smallRequestIntervalSeconds), ct);
                        waitTimeSeconds += smallRequestIntervalSeconds;
                    }
                } while (!ct.IsCancellationRequested && SessionTimeout > TimeSpan.FromSeconds(waitTimeSeconds));
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
                throw;
            }
            finally
            {
                await _logger.LogMessageAsync("Session stopped");
                _sessionIsInProgress = false;
            }
        }

        private async Task<ulong> UpdateStatisticInDiscord(IList<PlayerStatistic> statistic, ulong? messageId)
        {
            if (messageId == null)
            {
                return await _discordManager.SendPubgStatisticAsync(statistic);
            }

            return await _discordManager.ModifyPubgStatisticAsync(statistic, messageId.Value);

        }

        private bool StatisticChanged(IList<PlayerStatistic>? oldStatistic, IList<PlayerStatistic> newStatistic)
        {
            if (oldStatistic == null)
            {
                return newStatistic.Count != 0;
            }

            if (oldStatistic.Count != newStatistic.Count)
            {
                return false;
            }


            foreach (var @new in newStatistic)
            {
                var old = oldStatistic.FirstOrDefault(x => x.Name == @new.Name);
                if (old == null)
                {
                    return false;
                }

                if (old.NumberOfMatches != @new.NumberOfMatches)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
