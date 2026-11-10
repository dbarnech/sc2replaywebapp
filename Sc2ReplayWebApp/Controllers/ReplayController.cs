using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sc2ReplayWebApp.Data;
using Sc2ReplayWebApp.Models;
using Sc2ReplayWebApp.Services;
using static Sc2ReplayWebApp.Services.ReplayParserService;

namespace Sc2ReplayWebApp.Controllers;

[Authorize]
public class ReplayController : Controller
{
    private readonly IReplayParserService _parser;
    private readonly IBattleNetUserService _userService;

    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly string _uploadsFolder;
    private readonly ILogger<ReplayController> _logger;

    public ReplayController(
        IReplayParserService parser,
        IBattleNetUserService userService,
        AppDbContext db,
        IWebHostEnvironment env,
        ILogger<ReplayController> logger)
    {
        _parser = parser;
        _userService = userService;
        _db = db;
        _env = env;
        _logger = logger;

        // Ensure the uploads folder exists
        _uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
        Directory.CreateDirectory(_uploadsFolder);
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(List<IFormFile> files)
    {
        var skippedCount = 0;

        var currentUser = await _userService.GetOrCreateUser(User);

        foreach (var file in files)
        {
            if (file == null || file.Length == 0)
            {
                skippedCount++;
                continue;
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(_uploadsFolder, fileName);

            try
            {
                using (var stream = System.IO.File.Create(path))
                {
                    await file.CopyToAsync(stream);
                    await stream.FlushAsync();
                }

                using (System.IO.File.OpenRead(path))
                {
                }

                // Get the hash of the file to check for duplicates
                var replayHash = FileHashHelper.ComputeSha256(path);
                var exists = await _db.Replays
                    .AsNoTracking()
                    .AnyAsync(r => r.ReplayHash == replayHash);
                if (exists)
                {
                    _logger.LogWarning("Replay '{FileName}' has already been uploaded.", file.FileName);
                    skippedCount++;
                    continue;
                }

                var parsed = await _parser.ParseReplay(path);

                if (parsed.Warnings != null && parsed.Warnings.Count > 0)
                {
                    var warnings = string.Join(',', parsed.Warnings);
                    _logger.LogWarning("Replay '{FileName}' has uploaded with warnings {Warnings}.", file.FileName, warnings);
                }

                var replay = new ReplaySummary
                {
                    FileName = file.FileName,
                    ReplayHash = replayHash,
                    GameVersion = parsed.GameVersion,
                    BaseBuild = parsed.BaseBuild,
                    DataBuild = parsed.DataBuild,
                    DataVersion = parsed.DataVersion,
                    MapName = parsed.Title,
                    Winner = string.Join(",", parsed.Players.Where(p => p.Won).Select(p => p.Name)),
                    DurationSeconds = parsed.DurationSeconds,
                    UploadedAt = DateTime.UtcNow,
                    GameDate = parsed.GameDate,
                    DetailsHex = parsed.DetailsHex,
                    DetailsRaw = parsed.DetailsRaw,
                    BattleNetUserId = currentUser.Id,
                    User = currentUser,
                    Players = parsed
                        .Players
                        .Select(p => new PlayerSummary
                        {
                            Name = p.Name,
                            ActionsPerMinute = p.APM,
                            Race = p.Race,
                            Won = p.Won
                        })
                        .ToList()
                };

                _db.Replays.Add(replay);
            }
            catch (ReplayParsingException parsingEx)
            {
                _logger.LogError(parsingEx, "Error parsing replay {FileName}.", path);
                ModelState.AddModelError(string.Empty, $"An error occurred while parsing the replay {path}.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading replay {FileName}.", path);
                ModelState.AddModelError(string.Empty, $"An error occurred while uploading the replay. {path}");
            }
            finally
            {
                // Delete the temporary file after processing
                try
                {
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error deleting temporary replay file {FileName}.", path);
                }
            }
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = $"{files.Count} replay(s) processed, {files.Count - skippedCount} uploaded, {skippedCount} skipped.";

        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var replay = await _db.Replays
            .AsNoTracking()
            .Include(x => x.Players)
            .FirstAsync(x => x.Id == id);

        return View(replay);
    }

    public async Task<IActionResult> List(string search)
    {
        var currentUser = await _userService.GetOrCreateUser(User);

        var query = _db.Replays
            .AsNoTracking()
            .Include(r => r.Players)
            .Where(r =>
                r.User!.BattleNetId == currentUser.BattleNetId)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query
                .Where(r => r.MapName.ToLower().Contains(search.ToLower()));
        }

        var replays = await query
            .OrderByDescending(r => r.GameDate)
            .ToListAsync();

        var wins = replays.Count(r => r.Winner == currentUser.Sc2PlayerName);

        var model = new ReplayHistoryViewModel
        {
            TotalReplays = replays.Count,
            Wins = wins,
            Losses = replays.Count - wins,
            WinRate = replays.Count == 0
                ? 0
                : (double)wins / replays.Count * 100,
            Replays = replays
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var replay =
        await _db.Replays
            .FindAsync(id);

        if (replay != null)
        {
            _db.Replays.Remove(replay);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(List));
    }

    public async Task<IActionResult> Dashboard()
    {
        var currentUser = await _userService.GetOrCreateUser(User);
        var replays = await _db.Replays
                .Include(r => r.Players)
                .ToListAsync();

        var wins = replays.Count(r =>
                r.Winner == currentUser.Sc2PlayerName);

        var maps = replays
                .GroupBy(r => r.MapName)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .ToList();

        var model =
            new ReplayDashboardViewModel
            {
                TotalReplays = replays.Count,
                Wins = wins,
                Losses = replays.Count - wins,
                WinRate = replays.Count == 0
                        ? 0
                        : ((double)wins / replays.Count) * 100,
                Maps = [.. maps.Select(g => g.Key)],
                MapCounts = [.. maps.Select(g => g.Count())]
            };

        return View(model);
    }
}