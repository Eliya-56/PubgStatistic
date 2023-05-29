using Autofac;
using DiscordApi;
using PubgStatistic.ApplicationLogic;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.PubgApi;

namespace ApplicationStartup
{
    public class ApplicationBuilder
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly IList<ILogger> _loggers;

        public ApplicationBuilder()
        {
            _loggers = new List<ILogger>();
            _containerBuilder = new ContainerBuilder();

            _containerBuilder.RegisterType<PubgManager>().As<IPubgManager>();
            _containerBuilder.RegisterType<DiscordManager>().As<IDiscordManager>();
            _containerBuilder.RegisterType<StatisticManager>().As<IStatisticManager>();
        }

        public ApplicationBuilder AddRegistration(Action<ContainerBuilder> registerAction)
        {
            registerAction?.Invoke(_containerBuilder);
            return this;
        }

        public ApplicationBuilder AddLogger(ILogger logger)
        {
            _loggers.Add(logger);
            return this;
        }

        public IContainer Build()
        {
            _containerBuilder.Register(x => new AggregateLogger(_loggers)).As<ILogger>();
            return _containerBuilder.Build();
        }
    }
}