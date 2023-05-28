// See https://aka.ms/new-console-template for more information

using ApplicationStartup;
using Autofac;
using DiscordApi;
using PubgStatistic.ConsoleApp;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.Contracts.Records;
using PubgStatistic.PubgApi;

//const string myPlayerId = "account.054f6584d44048099cd32724bcb01a5d";
const string playerName = "PlayerEliya";
const string discordWebhookUrl = "https://discord.com/api/webhooks/1102266019197751307/VaGsSXpfVJEWv5QM0fYWOrRmsdRdSyOY3uUqiJRP611JqB30wldbZ8RHmwietE3ywTh7";
const string pubgApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxZGQ4OGNiMC1jOWIzLTAxM2ItZmJlOC0yZTE3NmZkNTJmNDkiLCJpc3MiOiJnYW1lbG9ja2VyIiwiaWF0IjoxNjgyODc5NDc2LCJwdWIiOiJibHVlaG9sZSIsInRpdGxlIjoicHViZyIsImFwcCI6Im15b3duc3RhdGlzdGljIn0.ezzzYfQGTswZb9x7rZym7GMUZqTPMF8rtdN5B3VG-qw";

var fromDate = DateTimeOffset.UtcNow.AddMonths(-1);

RequestProperties requestProperties = new RequestProperties(playerName, discordWebhookUrl, pubgApiKey);

var container = ApplicationBuilder.BuildAsync(containerBuilder =>
{
    containerBuilder.RegisterType<ConsoleLogger>().As<ILogger>();
});
await using var scope = container.BeginLifetimeScope();
var statisticManager = scope.Resolve<IStatisticManager>();
ILogger logger = scope.Resolve<ILogger>();

await logger.LogMessageAsync("Started");
await logger.LogErrorAsync("Test error");
while (true)
{
    await logger.LogMessageAsync("Waiting for command");
    var input = Console.ReadLine();
    if (input == "send")
    {
        try
        {
            await statisticManager.SendStatisticSnapshotAsync(requestProperties, fromDate);
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
