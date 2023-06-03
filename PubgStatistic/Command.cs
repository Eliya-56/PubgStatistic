namespace PubgStatistic.ConsoleApp
{
    internal static class Command
    {
        public const string SendStatisticByMinutesCommand = "minutes";
        public const string SendStatisticByDaysCommand = "days";
        public const string SendStatisticByMonthsCommand = "month";

        public const string SendStatisticForLastNumberOfMatchesCommand = "matches";

        public const string SetUserNameCommand = "name";
        public const string SetPubgApiKeyCommand = "pubg-key";
        public const string SetDiscordWebhookUrl = "discord-url";

        public const string StartGameSessionCommand = "start";
        public const string StopGameSessionCommand = "stop";

        public const string HelpCommand = "help";
    }
}
