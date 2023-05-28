using PubgStatistic.Contracts.Records;

namespace PubgStatistic.Contracts.Interfaces
{
    public interface IPubgManager
    {
        string ApiKey { set; }

        IAsyncEnumerable<PlayerStatistic> GetStatisticFromDate(
            string playerName, 
            DateTimeOffset fromDate,
            CancellationToken ct = default);

        IAsyncEnumerable<PlayerStatistic> GetStatisticForNumberOfMatches(
            string playerName, 
            int numberOfMatches,
            CancellationToken ct = default);
    }
}
