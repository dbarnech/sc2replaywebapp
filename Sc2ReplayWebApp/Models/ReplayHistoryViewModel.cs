namespace Sc2ReplayWebApp.Models;

public class ReplayHistoryViewModel
{
    public int TotalReplays { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public double WinRate { get; set; }

    public List<ReplaySummary> Replays { get; set; } = [];
}