namespace PubgStatistic.Contracts.Interfaces
{
    public interface ILogger
    {
        Task LogMessageAsync(string message);
        Task LogErrorAsync(string message);
    }
}
