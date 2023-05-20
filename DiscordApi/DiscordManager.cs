using Discord;
using Discord.Webhook;
using PubgStatistic.Core;
using PubgStatistic.Core.Records;

namespace DiscordApi
{
    public class DiscordManager
    {
        private readonly string _webhookUrl;

        public DiscordManager(string webhookUrl)
        {
            _webhookUrl = webhookUrl;
        }

        public async Task<ulong> SendStatistic(IList<PlayerStatistic> stats, ulong? messageID = null)
        {
            stats = stats.OrderByDescending(x => x.Damage).ToList();
            var statsString = stats.FormatStatisticToTable();
            var message = new EmbedBuilder()
                .WithTitle("Статистика игровой сессии")
                .WithDescription($"```\n{stats.FormatStatisticToTable()}\n")
                .Build();

            using var client = new DiscordWebhookClient(_webhookUrl);

            if (messageID == null)
            {
                return await client.SendMessageAsync(embeds: new[] { message });
            }

            await client.ModifyMessageAsync(messageID.Value, x => x.Embeds = new[] { message });
            return messageID.Value;
        }
    }
}
