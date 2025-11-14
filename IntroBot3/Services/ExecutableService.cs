using System.Diagnostics;
using IntroBot3.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntroBot3.Services;

public class ExecutableService
{
    private readonly string folderPath;
    private readonly string tempFolderPath;
    private readonly string ytDlpPath;
    private readonly string ffmpegPath;
    private readonly ILogger<ExecutableService> logger;
    private readonly HttpClientService httpClientService;

    public ExecutableService(IOptions<ExecutablesSettings> options, ILogger<ExecutableService> logger, HttpClientService httpClientService)
    {
        folderPath = options.Value.DownloadPath;
        tempFolderPath = Path.Combine(folderPath, "temp");
        ytDlpPath = Path.Combine(folderPath, "yt-dlp");
        ffmpegPath = Path.Combine(folderPath, "ffmpeg");
        this.logger = logger;
        this.httpClientService = httpClientService;
    }

    public async Task<Result> EnsureLibrariesExistAsync()
    {
        var folderResult = await EnsureProgramDirectoryExists();
        if (folderResult == Result.Fail)
        {
            return Result.Fail;
        }
        var ytDlpResult = await EnsureYtDlpExists();
        if (ytDlpResult == Result.Fail)
        {
            return Result.Fail;
        }
        var ffmpegResult = await EnsureFfmpegExists();
        if (ffmpegResult == Result.Fail)
        {
            return Result.Fail;
        }
        return Result.Success;
    }

    private async Task<Result> EnsureYtDlpExists()
    {
        if (!File.Exists(ytDlpPath))
        {
            logger.LogInformation("yt-dlp not found, downloading...");
            var result = await httpClientService.DownloadFileAsync("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp", ytDlpPath);
            if (result == Result.Fail)
            {
                logger.LogError("Failed to download yt-dlp.");
                return Result.Fail;
            }
            logger.LogInformation("yt-dlp downloaded successfully.");

            // Make executable
            logger.LogInformation("Making yt-dlp executable...");
            var chmodResult = await Run("chmod", $"+x {ytDlpPath}");
            if (chmodResult == Result.Fail)
            {
                logger.LogError("Failed to make yt-dlp executable.");
                return Result.Fail;
            }
            logger.LogInformation("yt-dlp is now executable.");
        }
        return Result.Success;
    }

    private async Task<Result> EnsureFfmpegExists()
    {
        if (!File.Exists(ffmpegPath))
        {
            var tempFolderResult = await EnsureTempDirectoryExists();
            if (tempFolderResult == Result.Fail)
            {
                return Result.Fail;
            }

            logger.LogInformation("ffmpeg not found, downloading...");
            var result = await httpClientService.DownloadFileAsync("https://github.com/BtbN/FFmpeg-Builds/releases/latest/download/ffmpeg-master-latest-linux64-gpl.tar.xz", $"{folderPath}/temp/ffmpeg.tar.xz");
            if (result == Result.Fail)
            {
                logger.LogError("Failed to download ffmpeg.");
                return Result.Fail;
            }
            logger.LogInformation("ffmpeg downloaded successfully.");

            // Extract ffmpeg
            logger.LogInformation("Extracting ffmpeg...");
            var tarResult = await Run("tar", $"-xf {folderPath}/temp/ffmpeg.tar.xz -C {folderPath}/temp --strip-components=1");
            if (tarResult == Result.Fail)
            {
                logger.LogError("Failed to extract ffmpeg.");
                return Result.Fail;
            }

            // Move ffmpeg binary to target path
            File.Move($"{folderPath}/temp/bin/ffmpeg", ffmpegPath);
            logger.LogInformation("ffmpeg extracted successfully.");

            // Make executable
            logger.LogInformation("Making ffmpeg executable...");
            var chmodResult = await Run("chmod", $"+x {ffmpegPath}");
            if (chmodResult == Result.Fail)
            {
                logger.LogError("Failed to make ffmpeg executable.");
                return Result.Fail;
            }
            logger.LogInformation("ffmpeg is now executable.");

            // Clean up temp files
            Directory.Delete(tempFolderPath, true);
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

    private async Task<Result> EnsureTempDirectoryExists()
    {
        if (!Directory.Exists(tempFolderPath))
        {
            logger.LogInformation("Temp directory not found, creating...");
            Directory.CreateDirectory(tempFolderPath);
            await Task.Delay(500);

            if (!Directory.Exists(tempFolderPath))
            {
                logger.LogError("Failed to create temp directory.");
                return Result.Fail;
            }

            logger.LogInformation("Temp directory created.");
        }
        return Result.Success;
    }

    private static async Task<Result> Run(string command, string arguments)
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