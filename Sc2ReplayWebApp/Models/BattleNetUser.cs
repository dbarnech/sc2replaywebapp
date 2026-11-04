using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Sc2ReplayWebApp.Models;

[Index(nameof(BattleNetId), IsUnique = true)]
public class BattleNetUser
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string BattleNetId { get; set; } = "";

    [Required]
    [MaxLength(100)]
    public string BattleTag { get; set; } = "";

    public DateTime FirstLogin { get; set; }

    public DateTime LastLogin { get; set; }

    public string Region { get; set; } = "us";

    public string Sc2PlayerName { get; set; } = "";

    public string Sc2ProfileId { get; set; } = "";

    public int Sc2RegionId { get; set; } = 1;

    public int Sc2RealmId { get; set; } = 1;

    public ICollection<ReplaySummary> Replays { get; set; }
        = new List<ReplaySummary>();
}
