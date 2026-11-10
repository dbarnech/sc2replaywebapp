namespace Sc2ReplayWebApp.Models;

public class ReplayDashboardViewModel
{
    public int TotalReplays { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public double WinRate { get; set; }

    public List<string> Maps { get; set; } = [];

    public List<int> MapCounts { get; set; } = [];
}
