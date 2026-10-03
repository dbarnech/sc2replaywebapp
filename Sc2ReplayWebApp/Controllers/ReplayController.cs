using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Sc2ReplayWebApp.Data;
using Sc2ReplayWebApp.Models;
using Sc2ReplayWebApp.Services;

namespace Sc2ReplayWebApp.Controllers;

public class ReplayController : Controller
{
    private readonly ReplayParserService _parser;
    private readonly AppDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ReplayController> _logger;

    public ReplayController(
        ReplayParserService parser,
        AppDbContext db,
        IWebHostEnvironment env,
        ILogger<ReplayController> logger)
    {
        _parser = parser;
        _db = db;
        _env = env;
        _logger = logger;
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return View();

            var folder = Path.Combine(_env.ContentRootPath, "uploads");

            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var path = Path.Combine(folder, fileName);

            using (var stream = System.IO.File.Create(path))
            {
                await file.CopyToAsync(stream);
                await stream.FlushAsync();
            }

            using (System.IO.File.OpenRead(path))
            {
            }

            var parsed = await _parser.ParseReplay(path);

            //_logger.LogDebug(parsed.MetadataJson);

            var replay = new ReplaySummary
            {
                FileName = file.FileName,
                GameVersion = parsed.GameVersion,
                BaseBuild = parsed.BaseBuild,
                DataBuild = parsed.DataBuild,
                DataVersion = parsed.DataVersion,
                MapName = parsed.Title,
                Winner = string.Join(",", parsed.Players.Where(p => p.Won).Select(p => p.Name)),
                DurationSeconds = parsed.DurationSeconds,
                UploadedAt = DateTime.UtcNow,
                Players = parsed
                    .Players
                    .Select(p => new PlayerSummary
                    {
                        Name = p.Name,
                        ActionsPerMinute = p.APM,
                        Race = p.AssignedRace,
                        Won = p.Won
                    })
                    .ToList()
            };

            _db.Replays.Add(replay);

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Details),
                new { id = replay.Id });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading replay");
            ModelState.AddModelError(string.Empty, "An error occurred while uploading the replay.");
            return View();
        }
    }

    public async Task<IActionResult> Details(int id)
    {
        var replay = await _db.Replays
            .Include(x => x.Players)
            .FirstAsync(x => x.Id == id);

        return View(replay);
    }

    public async Task<IActionResult> List()
    {
        return View(
            await _db.Replays
                .OrderByDescending(x => x.UploadedAt)
                .ToListAsync());
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var replay = await _db.Replays
            .Include(x => x.Players)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (replay != null)
        {
            _db.Remove(replay);
            await _db.SaveChangesAsync();
        }

        return RedirectToAction(nameof(List));
    }
}