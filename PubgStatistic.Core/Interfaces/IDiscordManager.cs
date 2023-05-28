using PubgStatistic.Contracts.Records;

namespace PubgStatistic.Contracts.Interfaces
{
    public interface IDiscordManager
    {
        string WebhookUrl { set; }
        Task<ulong> SendPubgStatisticAsync(IList<PlayerStatistic> stats);
        Task<ulong> ModifyPubgStatisticAsync(IList<PlayerStatistic> stats, ulong messageId);
    }
}
