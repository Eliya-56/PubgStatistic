using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordApi;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.Contracts.Records;
using PubgStatistic.PubgApi;

namespace PubgStatistic.ApplicationLogic
{
    public class StatisticManager
    {
        private readonly IPubgManager _pubgManager;
        private readonly IDiscordManager _discordManager;
        private readonly ILogger _logger;

        public TimeSpan SessionTimeout { get; set; } = TimeSpan.FromHours(2);

        public StatisticManager(
            IPubgManager pubgManager,
            IDiscordManager discordManager,
            ILogger logger)
        {
            _pubgManager = pubgManager;
            _discordManager = discordManager;
            _logger = logger;
        }

        public async Task StartNewGameSessionAsync(CancellationToken ct = default)
        {
            var smallRequestIntervalSeconds = 60;
            var largeRequestIntervalSeconds = 1200;
            var waitTimeSeconds = 0;

            await _logger.LogMessageAsync("Start new game session");
            var startDateTime = DateTimeOffset.UtcNow;
            ulong? messageId = null;
            IList<PlayerStatistic>? lastStatistics = null;

            try
            {
                do
                {
                    await _logger.LogMessageAsync("Try get statistic from pubg server");
                    var newStatistic = await _pubgManager.GetStatisticFromDate(startDateTime).ToListAsync(ct);

                    if (StatisticChanged(lastStatistics, newStatistic))
                    {
                        await _logger.LogMessageAsync("Statistic changed. Try to send statistic to discord");
                        messageId = await UpdateStatisticInDiscord(newStatistic, messageId);
                        lastStatistics = newStatistic;

                        await _logger.LogMessageAsync($"Wait for {largeRequestIntervalSeconds} second");
                        await Task.Delay(TimeSpan.FromSeconds(largeRequestIntervalSeconds), ct);
                    }
                    else
                    {
                        await _logger.LogMessageAsync($"Wait for {smallRequestIntervalSeconds} second");
                        await Task.Delay(TimeSpan.FromSeconds(smallRequestIntervalSeconds), ct);
                    }
                } while (ct.IsCancellationRequested || SessionTimeout < TimeSpan.FromSeconds(waitTimeSeconds));
            }
            catch (Exception ex)
            {
                await _logger.LogErrorAsync(ex.Message);
                throw;
            }
            finally
            {
                await _logger.LogMessageAsync("Session stopped");
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
