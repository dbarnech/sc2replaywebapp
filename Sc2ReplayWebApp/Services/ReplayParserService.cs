using System.Diagnostics;
using System.Text.Json;
using Sc2ReplayWebApp.Models;

namespace Sc2ReplayWebApp.Services;

public class ReplayParserService
{
    private readonly IWebHostEnvironment _env;

    public ReplayParserService(IWebHostEnvironment env)
    {
        _env = env;
    }

    public async Task<ReplayParseResult> ParseReplay(
        string replayPath)
    {
        if (!File.Exists(replayPath))
        {
            throw new FileNotFoundException(replayPath);
        }

        var parserFile = Path.Combine(
            _env.ContentRootPath,
            "Python",
            "parse_replay.py");

        var start = new ProcessStartInfo
        {
            FileName = "py",
            Arguments = $"-3.11 \"{parserFile}\" \"{replayPath}\"",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var process = Process.Start(start)!;

        string json =
            await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            string error =
                await process.StandardError.ReadToEndAsync();
            throw new Exception(
                $"Replay parser failed with exit code {process.ExitCode}: {error}");
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new Exception("Replay parser returned empty output.");
        }

        return JsonSerializer.Deserialize<ReplayParseResult>(
                   json,
                   new JsonSerializerOptions
                   {
                       PropertyNameCaseInsensitive = true
                   })!;
    }
}