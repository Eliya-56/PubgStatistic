using PubgStatistic.Core.Interfaces;

namespace PubgStatistic.ConsoleApp
{
    internal class ConsoleLogger : ILogger
    {
        public Task LogMessageAsync(string message)
        {
            Console.WriteLine($"[{DateTime.Now:hh:mm:ss}] - {message}");
            return Task.CompletedTask;
        }

        public async Task LogErrorAsync(string message)
        {
            var foreground = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            await LogMessageAsync($" Error: {message}");
            Console.ForegroundColor = foreground;
        }
    }
}
