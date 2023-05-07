// See https://aka.ms/new-console-template for more information

using PubgStatistic;

const string myPlayerId = "account.054f6584d44048099cd32724bcb01a5d";
const string discordWebhookUrl = "https://discord.com/api/webhooks/1102266019197751307/VaGsSXpfVJEWv5QM0fYWOrRmsdRdSyOY3uUqiJRP611JqB30wldbZ8RHmwietE3ywTh7";
const string pubgApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxZGQ4OGNiMC1jOWIzLTAxM2ItZmJlOC0yZTE3NmZkNTJmNDkiLCJpc3MiOiJnYW1lbG9ja2VyIiwiaWF0IjoxNjgyODc5NDc2LCJwdWIiOiJibHVlaG9sZSIsInRpdGxlIjoicHViZyIsImFwcCI6Im15b3duc3RhdGlzdGljIn0.ezzzYfQGTswZb9x7rZym7GMUZqTPMF8rtdN5B3VG-qw";

var pubgManager = new PubgManager(pubgApiKey, myPlayerId, DateTimeOffset.UtcNow.AddDays(-2));

ulong? messageId = null;
var discordManager = new DiscordManager(discordWebhookUrl);

LogMessage("Started");
LogError("Test error");
while (true)
{
    var input = Console.ReadLine();
    if (input == "send")
    {
        try
        {
            LogMessage("Getting statistic");
            var matches = pubgManager.GetMatches();
            var stats = await pubgManager.GetStatistic(matches);

            LogMessage("Send statistic to discord");
            messageId = await discordManager.SendStatistic(stats, messageId);

            LogMessage($"Statistic sent with messageId: {messageId}");
        }
        catch (Exception ex)
        {
            LogError(ex.Message);
        }
    }

    if (input == "exit")
    {
        LogMessage("Exit program");
        break;
    }
}

void LogMessage(string message)
{
    Console.WriteLine($"[{DateTime.Now : hh:mm:ss}] - {message}");
}

void LogError(string message)
{
    var foreground = Console.ForegroundColor;
    Console.ForegroundColor = ConsoleColor.Red;
    LogMessage($" Error: {message}");
    Console.ForegroundColor = foreground;
}
