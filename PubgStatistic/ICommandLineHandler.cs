namespace PubgStatistic.ConsoleApp
{
    internal interface ICommandLineHandler
    {
        Task HandleCommandAsync(string? input, CancellationToken ct = default);
    }
}
