using System.ComponentModel.DataAnnotations.Schema;

namespace Sc2ReplayWebApp.Models;

public class ReplaySummary
{
    public int Id { get; set; }

    public string FileName { get; set; } = null!;

    public string GameVersion { get; set; } = null!;

    public string DataBuild { get; set; } = null!;

    public string DataVersion { get; set; } = null!;

    public string BaseBuild { get; set; } = null!;

    public string MapName { get; set; } = null!;

    public string Winner { get; set; } = null!;

    public int DurationSeconds { get; set; }

    public int RawDuration { get; set; }

    [NotMapped]
    public string DurationDisplay => TimeSpan.FromSeconds(DurationSeconds).ToString(@"hh\:mm\:ss");

    public DateTime UploadedAt { get; set; }

    public List<PlayerSummary> Players { get; set; } = [];
}