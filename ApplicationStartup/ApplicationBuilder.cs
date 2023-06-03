using Autofac;
using DiscordApi;
using PubgStatistic.ApplicationLogic;
using PubgStatistic.Contracts;
using PubgStatistic.Contracts.Interfaces;
using PubgStatistic.PubgApi;

namespace ApplicationStartup
{
    public class ApplicationBuilder
    {
        private readonly ContainerBuilder _containerBuilder;
        private readonly IList<ILogger> _loggers;
        private readonly IUserInfoProvider _userInfoProvider;

        public ApplicationBuilder()
        {
            _loggers = new List<ILogger>();
            _containerBuilder = new ContainerBuilder();
            _userInfoProvider = new UserInfoProvider();

            _containerBuilder.RegisterType<PubgManager>().As<IPubgManager>();
            _containerBuilder.RegisterType<DiscordManager>().As<IDiscordManager>();
            _containerBuilder.RegisterType<StatisticManager>().As<IStatisticManager>();
        }

        public ApplicationBuilder AddUserName(string userName)
        {
            _userInfoProvider.UserName = userName;
            return this;
        }

        public ApplicationBuilder AddPubgApiKey(string pubgApiKey)
        {
            _userInfoProvider.PubgApiKey = pubgApiKey;
            return this;
        }

        public ApplicationBuilder AddDiscordWebhookUrl(string discordWebhookUrl)
        {
            _userInfoProvider.DiscordWebhookUrl = discordWebhookUrl;
            return this;
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
            _containerBuilder.Register(x => _userInfoProvider).As<IUserInfoProvider>().SingleInstance();
            return _containerBuilder.Build();
        }
    }
}