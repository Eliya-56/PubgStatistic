using PubgStatistic.Contracts.Records;

namespace PubgStatistic.PubgApi
{
    public interface IPubgManager
    {
        public IAsyncEnumerable<PlayerStatistic> GetStatisticFromDate(DateTimeOffset fromDate);
        public IAsyncEnumerable<PlayerStatistic> GetStatisticForNumberOfMatches(int numberOfMatches);
    }
}
