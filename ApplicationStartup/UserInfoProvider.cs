using PubgStatistic.Contracts;

namespace PubgStatistic.ApplicationLogic
{
    internal class UserInfoProvider : IUserInfoProvider
    {
        private string? _userName;
        private string? _pubgApiKey;
        private string? _discordWebhookUrl;

        public string UserName
        {
            get
            {
                if (_userName is null)
                {
                    throw new NullReferenceException($"{nameof(UserName)} is not set");
                }

                return _userName;
            }
            set => _userName = value;
        }

        public string PubgApiKey
        {
            get
            {
                if (_pubgApiKey is null)
                {
                    throw new NullReferenceException($"{nameof(PubgApiKey)} is not set");
                }

                return _pubgApiKey;
            }
            set => _pubgApiKey = value;
        }

        public string DiscordWebhookUrl
        {
            get
            {
                if (_discordWebhookUrl is null)
                {
                    throw new NullReferenceException($"{nameof(DiscordWebhookUrl)} is not set");
                }

                return _discordWebhookUrl;
            }
            set => _discordWebhookUrl = value;
        }
    }
}
