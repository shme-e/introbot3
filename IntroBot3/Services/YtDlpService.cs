using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace IntroBot3.Services;

public class YtDlpService(HttpClientService httpClientService, ILogger<YtDlpService> logger)
{
    // join to directory of executable
    private readonly string folderPath = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "yt-dlp-files";
    private readonly string exePath = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "yt-dlp-files" + Path.DirectorySeparatorChar + "yt-dlp";
    private readonly HttpClientService _httpClientService = httpClientService;

    public async Task<Result> DownloadFromUrl(string url, string range, string fileName)
    {
        var ytDlpExists = await EnsureYtDlpExists();
        if (ytDlpExists == Result.Fail)
        {
            return Result.Fail;
        }

        var ytDlpResult = await Run(exePath, $" -P theme-cache -o {fileName} -x --audio-format opus -q {url} --download-section \"*{range}\" --max-filesize 100M --force-overwrites");
        return ytDlpResult;
    }

    private async Task<Result> EnsureYtDlpExists()
    {
        var folderResult = await EnsureProgramDirectoryExists();
        if (folderResult == Result.Fail)
        {
            return Result.Fail;
        }

        if (!File.Exists(exePath))
        {
            logger.LogInformation("yt-dlp not found, downloading...");
            var result = await _httpClientService.DownloadFileAsync("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp", exePath);
            if (result == Result.Fail)
            {
                logger.LogError("Failed to download yt-dlp.");
                return Result.Fail;
            }
            logger.LogInformation("yt-dlp downloaded successfully.");

            // Make executable
            logger.LogInformation("Making yt-dlp executable...");
            var chmodResult = await Run("chmod", $"+x {exePath}");
            if (chmodResult == Result.Fail)
            {
                logger.LogError("Failed to make yt-dlp executable.");
                return Result.Fail;
            }
            logger.LogInformation("yt-dlp is now executable.");
        }
        return Result.Success;
    }

    private async Task<Result> EnsureProgramDirectoryExists()
    {
        if (!Directory.Exists(folderPath))
        {
            logger.LogInformation("Program directory not found, creating...");
            Directory.CreateDirectory(folderPath);
            await Task.Delay(500);

            if (!Directory.Exists(folderPath))
            {
                logger.LogError("Failed to create program directory.");
                return Result.Fail;
            }

            logger.LogInformation("Program directory created.");
        }
        return Result.Success;
    }

    private async Task<Result> Run(string command, string arguments)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            UseShellExecute = false,
            CreateNoWindow = true,
            WindowStyle = ProcessWindowStyle.Hidden
        };

        using var process = Process.Start(startInfo);

        if (process == null)
        {
            return Result.Fail;
        }

        await process.WaitForExitAsync();
        return process.ExitCode == 0 ? Result.Success : Result.Fail;
    }
}