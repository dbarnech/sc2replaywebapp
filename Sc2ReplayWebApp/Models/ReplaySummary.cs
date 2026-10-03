namespace Sc2ReplayWebApp.Models;

public class ReplaySummary
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string MapName { get; set; } = null!;

    public string Winner { get; set; } = null!;

    public int DurationSeconds { get; set; }

    public int RawDuration { get; set; }

    public DateTime UploadedAt { get; set; }

    public List<PlayerSummary> Players { get; set; } = [];
}