using System.Text.Json.Serialization;

namespace Sc2ReplayWebApp.Models;

public class ReplayParseResult
{
    public string Title { get; set; } = null!;

    public string GameVersion { get; set; } = null!;

    public string DataBuild { get; set; } = null!;

    public string DataVersion { get; set; } = null!;

    public string BaseBuild { get; set; } = null!;

    public int Duration { get; set; }

    // DurationSeconds = Duration / 1.4
    public int DurationSeconds { get; set; }

    public List<PlayerParseResult> Players { get; set; } = [];
}

/// <summary>
/// Players parsing from SC2Replay metadata
/// {
///   "PlayerID": 1,
///   "APM": 93.000000,
///   "Result": "Loss",         // "Win"
///   "SelectedRace": "Terr",   // "Prot" // "Zerg"
///   "AssignedRace": "Terr"
/// }
/// </summary>
public class PlayerParseResult
{
    public int PlayerID { get; set; }

    public string Name { get; set; } = null!;

    public double APM { get; set; }

    public string Result { get; set; } = null!;

    public string SelectedRace { get; set; } = null!;

    public string AssignedRace { get; set; } = null!;

    public bool Won { get; set; }
}