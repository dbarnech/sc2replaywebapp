using System.Diagnostics;
using System.Text.Json;
using Sc2ReplayWebApp.Models;

namespace Sc2ReplayWebApp.Services;

public class ReplayParserService
{
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ReplayParserService> _logger;
    private static readonly JsonSerializerOptions DefaultJsonSerializerOptions = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true,
    };

    public ReplayParserService(
        IWebHostEnvironment env,
        ILogger<ReplayParserService> logger)
    {
        _env = env;
        _logger = logger;
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

        _logger.LogInformation("Executing parser script {Script}", parserFile);

        using var process = Process.Start(start)!;

        var json = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        _logger.LogInformation("Parser completed with exit code {ExitCode} and Content='{Json}'", process.ExitCode, json);

        if (process.ExitCode != 0)
        {
            var error = await process.StandardError.ReadToEndAsync();

            _logger.LogError(
                "Python parser failed. Error: {Error}",
                error);
            throw new Exception(
                $"Replay parser failed with exit code {process.ExitCode}: {error}");
        }

        if (string.IsNullOrWhiteSpace(json))
        {
            throw new Exception("Replay parser returned empty output.");
        }

        _logger.LogDebug(json);

        return JsonSerializer
            .Deserialize<ReplayParseResult>(json, DefaultJsonSerializerOptions)!;
    }
}