// See https://aka.ms/new-console-template for more information

using DiscordApi;
using PubgStatistic.ConsoleApp;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.PubgApi;

const string myPlayerId = "account.054f6584d44048099cd32724bcb01a5d";
const string discordWebhookUrl = "https://discord.com/api/webhooks/1102266019197751307/VaGsSXpfVJEWv5QM0fYWOrRmsdRdSyOY3uUqiJRP611JqB30wldbZ8RHmwietE3ywTh7";
const string pubgApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxZGQ4OGNiMC1jOWIzLTAxM2ItZmJlOC0yZTE3NmZkNTJmNDkiLCJpc3MiOiJnYW1lbG9ja2VyIiwiaWF0IjoxNjgyODc5NDc2LCJwdWIiOiJibHVlaG9sZSIsInRpdGxlIjoicHViZyIsImFwcCI6Im15b3duc3RhdGlzdGljIn0.ezzzYfQGTswZb9x7rZym7GMUZqTPMF8rtdN5B3VG-qw";

var fromDate = DateTimeOffset.UtcNow.AddMonths(-1);
IPubgManager pubgManager = new PubgManager(pubgApiKey, myPlayerId);

ulong? messageId = null;
IDiscordManager discordManager = new DiscordManager(discordWebhookUrl);

ILogger logger = new ConsoleLogger();

await logger.LogMessageAsync("Started");
await logger.LogErrorAsync("Test error");
while (true)
{
    var input = Console.ReadLine();
    if (input == "send")
    {
        try
        {
            await logger.LogMessageAsync("Getting statistic");
            var stats = await pubgManager.GetStatisticFromDate(fromDate).ToListAsync();

            await logger.LogMessageAsync("Send statistic to discord");
            if (messageId == null)
            {
                messageId = await discordManager.SendPubgStatisticAsync(stats);
            }
            else
            {
                await discordManager.ModifyPubgStatisticAsync(stats, messageId.Value);
            }

            await logger.LogMessageAsync($"Statistic sent with messageId: {messageId}");
        }
        catch (Exception ex)
        {
            await logger.LogErrorAsync(ex.Message);
        }
    }

    if (input == "exit")
    {
        await logger.LogMessageAsync("Exit program");
        break;
    }
}
