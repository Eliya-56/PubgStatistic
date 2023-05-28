using Autofac;
using DiscordApi;
using PubgStatistic.ApplicationLogic;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.PubgApi;

namespace ApplicationStartup
{
    public class ApplicationBuilder
    {
        public static IContainer BuildAsync(Action<ContainerBuilder> registerAction)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterType<PubgManager>().As<IPubgManager>();
            containerBuilder.RegisterType<DiscordManager>().As<IDiscordManager>();
            containerBuilder.RegisterType<StatisticManager>().As<IStatisticManager>();
            
            registerAction?.Invoke(containerBuilder);
            return containerBuilder.Build();
        }
    }
}