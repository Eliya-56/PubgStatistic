using Discord.Webhook;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.Contracts.Records;
using PubgStatistic.Core;

namespace DiscordApi
{
    public class DiscordManager : IDiscordManager
    {
        private const string ImagesDirectory = "statistics-images";

        private string? _webhookUrl;

        public string WebhookUrl
        {
            private get
            {
                if (_webhookUrl == null)
                {
                    throw new NullReferenceException($"{nameof(WebhookUrl)} is not set");
                }

                return _webhookUrl;
            }
            set => _webhookUrl = value;
        }

        public async Task<ulong> SendPubgStatisticAsync(IList<PlayerStatistic> stats)
        {
            Directory.CreateDirectory(ImagesDirectory);
            try
            {
                var filePath = BuildStatisticMessage(stats);
                using var client = new DiscordWebhookClient(WebhookUrl);
                return await client.SendFileAsync(filePath, "Статистика игровой сессии");
            }
            finally
            {
                Directory.Delete(ImagesDirectory, true);
            }
        }

        public async Task<ulong> ModifyPubgStatisticAsync(IList<PlayerStatistic> stats, ulong messageId)
        {
            using var client = new DiscordWebhookClient(WebhookUrl);
            await client.DeleteMessageAsync(messageId);
            return await SendPubgStatisticAsync(stats);
        }

        private string BuildStatisticMessage(IList<PlayerStatistic> stats)
        {
            stats = stats.OrderByDescending(x => x.Damage).ToList();

            var fileName = $"PlayerStatistics{Guid.NewGuid()}.png";
            var statsBitmap = stats.FormatStatisticToImage();
            var filePath = $"{ImagesDirectory}\\{fileName}";
            statsBitmap.Save($"{filePath}", System.Drawing.Imaging.ImageFormat.Png);

            return filePath;
        }
    }
}
