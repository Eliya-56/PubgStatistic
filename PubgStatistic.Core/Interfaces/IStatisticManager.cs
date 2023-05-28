using PubgStatistic.Contracts.Records;

namespace PubgStatistic.Contracts.Interfaces
{
    public interface IStatisticManager
    {
        Task StartNewGameSessionAsync(
            RequestProperties requestProperties,
            CancellationToken ct = default);

        Task SendStatisticSnapshotAsync(
            RequestProperties requestProperties,
            int numberOfGames,
            CancellationToken ct = default);
        Task SendStatisticSnapshotAsync(
            RequestProperties requestProperties,
            DateTimeOffset startDate,
            CancellationToken ct = default);
    }
}
