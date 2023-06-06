using System.Drawing;
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

        public static Bitmap FormatStatisticToImage(this IEnumerable<PlayerStatistic> stats)
        {
            var playerStatistics = stats.ToArray();

            // Define image dimensions
            int imageWidth = 960;
            int imageHeight = playerStatistics.Count() * 40 + 60; // 40px per player, 60px for headers
            
            // Create new image with defined dimensions
            Bitmap bmp = new Bitmap(imageWidth, imageHeight);

            // Get Graphics object from image
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Define fonts and brushes
                Font headerFont = new Font("Roboto", 12, FontStyle.Bold);
                Font dataFont = new Font("Roboto", 12);
                Brush blackBrush = new SolidBrush(Color.Black);

                // Define cell dimensions and offsets
                int cellWidth = imageWidth / 8; // We have 8 columns
                int cellHeight = 40;
                int xOffset = 0;
                int yOffset = 0;

                // Header data
                string[] headers = { "Name", "Matches", "Kills", "K/M", "Damage", "D/M", "HS Kills", "Revives" };

                // Draw table header
                for (int j = 0; j < headers.Length; j++)
                {
                    // Draw header cell
                    Rectangle cell = new Rectangle(
                        xOffset + j * cellWidth,
                        yOffset,
                        cellWidth,
                        cellHeight);
                    g.FillRectangle(new SolidBrush(Color.LightGoldenrodYellow), cell);
                    g.DrawRectangle(Pens.Black, cell);

                    // Draw header text
                    g.DrawString(
                        headers[j],
                        headerFont,
                        blackBrush,
                        cell,
                        new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                }

                yOffset += cellHeight; // Update offset for data rows

                // Draw table data
                foreach (var player in playerStatistics)
                {
                    string[] playerData = {
                        player.Name,
                        player.NumberOfMatches.ToString(), player.Kills.ToString(),
                        player.KillsPerMatch.ToString("F2"),
                        player.Damage.ToString("F2"),
                        player.DamagePerMatch.ToString("F2"),
                        player.HeadShotsKills.ToString(),
                        player.Revives.ToString()
                    };

                    for (int j = 0; j < playerData.Length; j++)
                    {
                        // Draw data cell
                        Rectangle cell = new Rectangle(
                            xOffset + j * cellWidth,
                            yOffset,
                            cellWidth,
                            cellHeight);
                        g.FillRectangle(new SolidBrush(Color.White), cell);
                        g.DrawRectangle(Pens.Black, cell);

                        // Draw data text
                        g.DrawString(
                            playerData[j],
                            dataFont,
                            blackBrush,
                            cell,
                            new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center });
                    }

                    yOffset += cellHeight; // Move to next row
                }
            }

            return bmp;

            // Save the image
            //bmp.Save("PlayerStatistics.png", System.Drawing.Imaging.ImageFormat.Png);
        }
    }
}
