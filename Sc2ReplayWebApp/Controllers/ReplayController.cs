using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sc2ReplayWebApp.Data;
using Sc2ReplayWebApp.Models;
using Sc2ReplayWebApp.Services;

namespace Sc2ReplayWebApp.Controllers;

public class ReplayController : Controller
{
    private readonly ReplayParserService _parser;

    private readonly AppDbContext _db;

    private readonly IWebHostEnvironment _env;

    public ReplayController(
        ReplayParserService parser,
        AppDbContext db,
        IWebHostEnvironment env)
    {
        _parser = parser;
        _db = db;
        _env = env;
    }

    public IActionResult Upload()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
            return View();

        var folder = Path.Combine(_env.ContentRootPath, "uploads");

        Directory.CreateDirectory(folder);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var path = Path.Combine(folder, fileName);

        Console.WriteLine("Saving replay");

        using (var stream = System.IO.File.Create(path))
        {
            await file.CopyToAsync(stream);
            await stream.FlushAsync();
        }

        Console.WriteLine("Replay file closed");

        Console.WriteLine("Starting parser");

        using (System.IO.File.OpenRead(path))
        {
        }

        var parsed = await _parser.ParseReplay(path);

        var replay = new ReplaySummary
        {
            FileName = file.FileName,
            MapName = parsed.MapName,
            Winner = parsed.Winner,
            DurationSeconds = parsed.DurationSeconds,
            UploadedAt = DateTime.UtcNow,
            Players = parsed.Players
        };

        _db.Replays.Add(replay);

        await _db.SaveChangesAsync();

        return RedirectToAction(nameof(Details),
            new { id = replay.Id });
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
}