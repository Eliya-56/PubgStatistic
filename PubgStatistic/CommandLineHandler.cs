using PubgStatistic.Contracts;
using PubgStatistic.Contracts.Interfaces;
using System.Text;

namespace PubgStatistic.ConsoleApp
{
    internal class CommandLineHandler : ICommandLineHandler
    {
        private readonly ILogger _logger;
        private readonly IStatisticManager _statisticManager;
        private readonly IUserInfoProvider _userInfoProvider;

        private CancellationTokenSource? _sessionCancellationTokenSource;

        public CommandLineHandler(
            ILogger logger,
            IStatisticManager statisticManager,
            IUserInfoProvider userInfoProvider)
        {
            _logger = logger;
            _statisticManager = statisticManager;
            _userInfoProvider = userInfoProvider;
        }

        public async Task HandleCommandAsync(string? input, CancellationToken ct = default)
        {
            if (input == null)
            {
                await _logger.LogErrorAsync("Command is empty");
                return;
            }

            var inputWords = input.Split(' ');
            switch (inputWords[0])
            {
                case Command.SendStatisticByDaysCommand:
                case Command.SendStatisticByMinutesCommand:
                case Command.SendStatisticByMonthsCommand:
                case Command.SendStatisticForLastNumberOfMatchesCommand:
                    await SendStatisticCommandAsync(inputWords, ct);
                    break;
                case Command.StartGameSessionCommand:
                    _sessionCancellationTokenSource = new CancellationTokenSource();
                    _statisticManager.StartNewGameSessionAsync(_sessionCancellationTokenSource.Token);
                    break;
                case Command.StopGameSessionCommand:
                    if (_sessionCancellationTokenSource is not null)
                    {
                        await _logger.LogMessageAsync("Stopping session");
                        _sessionCancellationTokenSource.Cancel();
                    }
                    else
                    {
                        await _logger.LogMessageAsync("Session hasn't been started");
                    }
                    break;
                case Command.SetUserNameCommand:
                    await SetUserNameAsync(inputWords);
                    break;
                case Command.SetPubgApiKeyCommand:
                    await SetPubgApiKeyAsync(inputWords);
                    break;
                case Command.SetDiscordWebhookUrl:
                    await SetDiscordWebhookAsync(inputWords);
                    break;
                case Command.HelpCommand:
                    await TypeHelp();
                    break;
                default:
                    await _logger.LogErrorAsync($"Command \"{input}\" is not defined");
                    break;
            }
        }

        private async Task TypeHelp()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Here is the list of commands: ");
            stringBuilder.AppendLine();
            stringBuilder.AppendLine($"{Command.SendStatisticByDaysCommand} \'value\' - send statistic for the last \'value\' days");
            stringBuilder.AppendLine($"{Command.SendStatisticByMinutesCommand} \'value\' - send statistic for the last \'value\' minutes");
            stringBuilder.AppendLine($"{Command.SendStatisticByMonthsCommand} \'value\' - send statistic for the last \'value\' month");
            stringBuilder.AppendLine($"{Command.StartGameSessionCommand} - start new game session from now, means that it will send and update statistic while you continue playing");
            stringBuilder.AppendLine($"{Command.SetUserNameCommand} \'username\' - set you pubg username");
            stringBuilder.AppendLine($"{Command.SetPubgApiKeyCommand} \'pubgApiKey\' - set you pubgApiKey");
            stringBuilder.AppendLine($"{Command.SetDiscordWebhookUrl} \'discordWebHook\' - set you discordWebHook");

            await _logger.LogMessageAsync(stringBuilder.ToString());
        }

        private async Task SetUserNameAsync(string[] inputWords)
        {
            if (inputWords.Length <= 1)
            {
                await _logger.LogErrorAsync($"Command \"{inputWords[0]}\" doesn't contain value argument");
                return;
            }

            _userInfoProvider.UserName = inputWords[1];
        }

        private async Task SetPubgApiKeyAsync(string[] inputWords)
        {
            if (inputWords.Length <= 1)
            {
                await _logger.LogErrorAsync($"Command \"{inputWords[0]}\" doesn't contain value argument");
                return;
            }

            _userInfoProvider.PubgApiKey = inputWords[1];
        }

        private async Task SetDiscordWebhookAsync(string[] inputWords)
        {
            if (inputWords.Length <= 1)
            {
                await _logger.LogErrorAsync($"Command \"{inputWords[0]}\" doesn't contain value argument");
                return;
            }

            _userInfoProvider.DiscordWebhookUrl = inputWords[1];
        }

        private async Task SendStatisticCommandAsync(string[] inputWords, CancellationToken ct = default)
        {
            if (inputWords.Length <= 1)
            {
                await _logger.LogErrorAsync($"Command \"{inputWords[0]}\" doesn't contain value argument");
                return;
            }

            var canParse = int.TryParse(inputWords[1], out var value);
            if (!canParse)
            {
                await _logger.LogErrorAsync($"Invalid value parameter \"{inputWords[1]}\"");
                return;
            }

            DateTimeOffset fromDate;
            switch (inputWords[0])
            {
                case Command.SendStatisticByDaysCommand:
                    fromDate = DateTimeOffset.UtcNow.AddDays(-value);
                    break;
                case Command.SendStatisticByMinutesCommand:
                    fromDate = DateTimeOffset.UtcNow.AddMinutes(-value);
                    break;
                case Command.SendStatisticByMonthsCommand:
                    fromDate = DateTimeOffset.UtcNow.AddMonths(-value);
                    break;
                case Command.SendStatisticForLastNumberOfMatchesCommand:
                    await _statisticManager.SendStatisticSnapshotAsync(value, ct);
                    return;
                default:
                    throw new Exception("Unexpected command");
            }

            await _statisticManager.SendStatisticSnapshotAsync(fromDate, ct);
        }
    }
}
