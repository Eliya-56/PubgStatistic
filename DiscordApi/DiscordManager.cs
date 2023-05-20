using Discord;
using Discord.Webhook;
using PubgStatistic.Contracts.Records;
using PubgStatistic.Core;

namespace DiscordApi
{
    public class DiscordManager : IDiscordManager
    {
        private readonly string _webhookUrl;

        public DiscordManager(string webhookUrl)
        {
            _webhookUrl = webhookUrl;
        }

        public async Task<ulong> SendPubgStatisticAsync(IList<PlayerStatistic> stats)
        {
            var message = BuildStatisticMessage(stats);
            using var client = new DiscordWebhookClient(_webhookUrl);
            return await client.SendMessageAsync(embeds: new[] { message });
        }

        public async Task<ulong> ModifyPubgStatisticAsync(IList<PlayerStatistic> stats, ulong messageId)
        {
            var message = BuildStatisticMessage(stats);
            using var client = new DiscordWebhookClient(_webhookUrl);
            await client.ModifyMessageAsync(messageId, x => x.Embeds = new[] { message });
            return messageId;
        }

        private Embed BuildStatisticMessage(IList<PlayerStatistic> stats)
        {
            stats = stats.OrderByDescending(x => x.Damage).ToList();
            var statsString = stats.FormatStatisticToTable();
            return new EmbedBuilder()
                .WithTitle("Статистика игровой сессии")
                .WithDescription($"```\n{statsString}\n")
                .Build();
        }
    }
}
