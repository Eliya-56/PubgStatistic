using PubgStatistic.Core.Records;

namespace DiscordApi
{
    public interface IDiscordManager
    {
        Task<ulong> SendPubgStatisticAsync(IList<PlayerStatistic> stats);
        Task<ulong> ModifyPubgStatisticAsync(IList<PlayerStatistic> stats, ulong messageId);
    }
}
