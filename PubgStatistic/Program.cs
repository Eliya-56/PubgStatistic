// See https://aka.ms/new-console-template for more information

using ApplicationStartup;
using Autofac;
using PubgStatistic.ConsoleApp;
using PubgStatistic.Contracts.Interfaces;

//const string myPlayerId = "account.054f6584d44048099cd32724bcb01a5d";

var container = new ApplicationBuilder()
    .AddUserInfoFromFile("config.yaml")
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
