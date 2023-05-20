using System.Text;
using PubgStatistic.Contracts.Records;

namespace PubgStatistic.Core
{
    public static class Formatter
    {
        public static string FormatStatisticToTable(this IEnumerable<PlayerStatistic> stats)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine(" Имя |-Игр-|-Уб-|-Сред-|-Урон-|-Сред-|-Голова-|-Воскр-");
            foreach (var stat in stats)
            {
                builder.Append($" {stat.Name.PadLeft(3, '-')[..3]} |");
                builder.Append($" {stat.NumberOfMatches.ToString("0.##").PadLeft(5, '-')}|");
                builder.Append($" {stat.Kills.ToString().PadLeft(4, '-')}|");
                builder.Append($" {stat.KillsPerMatch.ToString("0.##").PadLeft(6, '-')}|");
                builder.Append($" {stat.Damage.ToString("0.##").PadLeft(7, '-')}|");
                builder.Append($" {stat.DamagePerMatch.ToString("0.##").PadLeft(7, '-')}|");
                builder.Append($" {stat.HeadShotsKills.ToString().PadLeft(8, '-')}|");
                builder.Append($" {stat.Revives.ToString().PadLeft(7, '-')}");
                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
