namespace PubgStatistic.Contracts.Records
{
    public readonly record struct RequestProperties(
        string PlayerName,
        string DiscordWebhookUrl,
        string PubgApiKey);
}
