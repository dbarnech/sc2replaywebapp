namespace Sc2ReplayWebApp.Models;

public class ReplayParseResult
{
    public string MapName { get; set; } = "";

    public string Winner { get; set; } = "";

    public int DurationSeconds { get; set; }

    public List<PlayerSummary> Players { get; set; } = [];
}