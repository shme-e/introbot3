using System.Diagnostics;
using IntroBot3.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntroBot3.Services;

public class ExecutableService
{
    private readonly string folderPath;
    public readonly string YtDlpPath;
    public readonly string FfmpegPath;
    private readonly ILogger<ExecutableService> logger;
    private readonly HttpClientService httpClientService;

    public ExecutableService(IOptions<ExecutablesSettings> options, ILogger<ExecutableService> logger, HttpClientService httpClientService)
    {
        folderPath = options.Value.DownloadPath;
        YtDlpPath = Path.Combine(folderPath, "yt-dlp");
        FfmpegPath = Path.Combine(folderPath, "ffmpeg");
        this.logger = logger;
        this.httpClientService = httpClientService;
    }

    public async Task EnsureLibrariesExistAsync()
    {
        await EnsureProgramDirectoryExists();
        await EnsureYtDlpExists();
        ExportLibFolderToPath();
    }

    private async Task EnsureYtDlpExists()
    {
        if (!File.Exists(YtDlpPath))
        {
            logger.LogInformation("yt-dlp not found, downloading...");
            await httpClientService.DownloadFileAsync("https://github.com/yt-dlp/yt-dlp/releases/latest/download/yt-dlp", YtDlpPath);
            logger.LogInformation("yt-dlp downloaded successfully.");

            // Make executable
            logger.LogInformation("Making yt-dlp executable...");
            await Run("chmod", $"+x {YtDlpPath}");
            logger.LogInformation("yt-dlp is now executable.");
        }
    }
    private async Task EnsureProgramDirectoryExists()
    {
        await CreateDirectoryIfNotExists(folderPath);
    }

    private void ExportLibFolderToPath()
    {
        var currentPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;
        if (!currentPath.Split(Path.PathSeparator).Contains(folderPath))
        {
            var newPath = $"{currentPath}{Path.PathSeparator}{folderPath}";
            Environment.SetEnvironmentVariable("PATH", newPath);
        }
    }

    public static async Task CreateDirectoryIfNotExists(string path)
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            await Task.Delay(500);

            if (!Directory.Exists(path))
            {
                throw new Exception($"Failed to create directory at {path}.");
            }
        }
    }

    public static async Task Run(string command, string arguments)
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
            throw new Exception($"Failed to start process {command} {arguments}.");
        }

        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new Exception($"Process {command} {arguments} exited with code {process.ExitCode}.");
        }
    }
}