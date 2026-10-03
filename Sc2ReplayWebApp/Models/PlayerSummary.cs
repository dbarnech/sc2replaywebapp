namespace Sc2ReplayWebApp.Models;

public class PlayerSummary
{
    public int Id { get; set; }

    public string Name { get; set; } = "";

    public string Race { get; set; } = "";
    
    public double ActionsPerMinute { get; set; }

    public bool Won { get; set; }

    public int ReplaySummaryId { get; set; }
}