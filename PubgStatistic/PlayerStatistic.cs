namespace PubgStatistic
{
    public record PlayerStatistic(
        string Name,
        int NumberOfMatches,
        int Kills,
        double KillsPerMatch,
        double Damage,
        double DamagePerMatch,
        int HeadShotsKills,
        double HeadShotsKillsPerMatch,
        int Revives,
        double RevivesPerMatch,
        double WalkDistance,
        double WalkDistancePerMatch,
        double VehiclesDistance,
        double VehiclesPerMatch,
        double SwimDistance, 
        double SwimDistancePerMatch
    );
    
}
