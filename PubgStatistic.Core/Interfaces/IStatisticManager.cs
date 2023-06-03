namespace PubgStatistic.Contracts.Interfaces
{
    public interface IStatisticManager
    {
        Task StartNewGameSessionAsync(CancellationToken ct = default);
        Task SendStatisticSnapshotAsync(int numberOfGames, CancellationToken ct = default);
        Task SendStatisticSnapshotAsync(DateTimeOffset startDate, CancellationToken ct = default);
    }
}
