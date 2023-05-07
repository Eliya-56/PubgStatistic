namespace PubgStatistic
{
    public record PlayerMatchStatistic(
        string PlayerId,
        string Name,
        int Kills,
        double Damage,
        int HeadShotsKills,
        int Revives,
        double WalkDistance,
        double VehiclesDistance,
        double SwimDistance
    );
}
