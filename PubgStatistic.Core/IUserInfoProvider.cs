namespace PubgStatistic.Contracts
{
    public interface IUserInfoProvider
    {
        string UserName { get; set; }
        string PubgApiKey { get; set; }
        string DiscordWebhookUrl { get; set; }
    }
}
