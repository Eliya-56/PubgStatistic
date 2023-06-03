// See https://aka.ms/new-console-template for more information

using ApplicationStartup;
using Autofac;
using PubgStatistic.ConsoleApp;
using PubgStatistic.Contracts.Interfaces;

//const string myPlayerId = "account.054f6584d44048099cd32724bcb01a5d";
const string playerName = "PlayerEliya";
const string discordWebhookUrl = "https://discord.com/api/webhooks/1102266019197751307/VaGsSXpfVJEWv5QM0fYWOrRmsdRdSyOY3uUqiJRP611JqB30wldbZ8RHmwietE3ywTh7";
const string pubgApiKey = "eyJ0eXAiOiJKV1QiLCJhbGciOiJIUzI1NiJ9.eyJqdGkiOiIxZGQ4OGNiMC1jOWIzLTAxM2ItZmJlOC0yZTE3NmZkNTJmNDkiLCJpc3MiOiJnYW1lbG9ja2VyIiwiaWF0IjoxNjgyODc5NDc2LCJwdWIiOiJibHVlaG9sZSIsInRpdGxlIjoicHViZyIsImFwcCI6Im15b3duc3RhdGlzdGljIn0.ezzzYfQGTswZb9x7rZym7GMUZqTPMF8rtdN5B3VG-qw";

var container = new ApplicationBuilder()
    .AddUserName(playerName)
    .AddPubgApiKey(pubgApiKey)
    .AddDiscordWebhookUrl(discordWebhookUrl)
    .AddLogger(new ConsoleLogger())
    .AddRegistration(x => x.RegisterType<CommandLineHandler>().As<ICommandLineHandler>())
    .Build();
await using var scope = container.BeginLifetimeScope();

var commandHandler = scope.Resolve<ICommandLineHandler>();
ILogger logger = scope.Resolve<ILogger>();

await logger.LogMessageAsync("Started");
await logger.LogErrorAsync("Test error");
while (true)
{
    await logger.LogMessageAsync("Waiting for command");
    var input = Console.ReadLine();

    if (input == "exit")
    {
        await logger.LogMessageAsync("Exit program");
        break;
    }

    await commandHandler.HandleCommandAsync(input);
}
