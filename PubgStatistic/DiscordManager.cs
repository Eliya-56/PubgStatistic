using Discord;
using Discord.Webhook;
using System.Text;

namespace PubgStatistic
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
            var statsString = FormatStatistic(stats);
            var message = new EmbedBuilder()
                .WithTitle("Статистика игровой сессии")
                .WithDescription($"```\n{FormatStatistic(stats)}\n")
                .Build();

            using var client = new DiscordWebhookClient(_webhookUrl);

            if (messageID == null)
            {
                return await client.SendMessageAsync(embeds: new[] { message });
            }

            await client.ModifyMessageAsync(messageID.Value, x => x.Embeds = new[] { message });
            return messageID.Value;
        }

        private static string FormatStatistic(IList<PlayerStatistic> stats)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("------Имя-------|-Игр-|-Уб-|-Сред-|-Урон-|-Сред-|-Голова-|-Воскр-");
            foreach (var stat in stats)
            {
                builder.Append($"{stat.Name.PadLeft(16, '-')}|");
                builder.Append($" {stat.NumberOfMatches.ToString("0.##").PadLeft(5, '-')}|");
                builder.Append($" {stat.Kills.ToString().PadLeft(4, '-')}|");
                builder.Append($" {stat.KillsPerMatch.ToString("0.##").PadLeft(6, '-')}|");
                builder.Append($" {stat.Damage.ToString("0.##").PadLeft(6, '-')}|");
                builder.Append($" {stat.DamagePerMatch.ToString("0.##").PadLeft(6, '-')}|");
                builder.Append($" {stat.HeadShotsKills.ToString().PadLeft(8, '-')}|");
                builder.Append($" {stat.Revives.ToString().PadLeft(7, '-')}");
                builder.AppendLine();
            }

            return builder.ToString();
        }

        private static string FormatStatisticМ2(IList<PlayerStatistic> stats)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("------Имя-------|-Игр-|-Уб-|-Сред-|-Урон-|-Сред-|-Голова-|-Воскр-");
            foreach (var stat in stats)
            {
                builder.Append($"{stat.Name.PadLeft(16, '-')}|");
                builder.Append($" {stat.NumberOfMatches.ToString("0.##").PadLeft(5, '-')}|");
                builder.Append($" {stat.Kills.ToString().PadLeft(4, '-')}|");
                builder.Append($" {stat.KillsPerMatch.ToString("0.##").PadLeft(6, '-')}|");
                builder.Append($" {stat.Damage.ToString("0.##").PadLeft(6, '-')}|");
                builder.Append($" {stat.DamagePerMatch.ToString("0.##").PadLeft(6, '-')}|");
                builder.Append($" {stat.HeadShotsKills.ToString().PadLeft(8, '-')}|");
                builder.Append($" {stat.Revives.ToString().PadLeft(7, '-')}");
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
