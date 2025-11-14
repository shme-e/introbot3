using IntroBot3.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntroBot3.Services;

public class YtDlpService(ExecutableService executableService, IOptions<ThemeCacheSettings> options, ILogger<YtDlpService> logger)
{
    private readonly ExecutableService executableService = executableService;
    private readonly string themeCachePath = options.Value.Path;
    private readonly ILogger<YtDlpService> logger = logger;

    public async Task DownloadFromUrl(string url, string range, string fileName)
    {
        await executableService.EnsureLibrariesExistAsync();
        await ExecutableService.CreateDirectoryIfNotExists(themeCachePath);

        var filePath = Path.Combine(themeCachePath, fileName);

        logger.LogInformation("Starting download from yt-dlp... for {url} in range {range} to {filePath}", url, range, filePath);
        await ExecutableService.Run("yt-dlp", $"-P theme-cache -o {filePath} -x --audio-format opus -q {url} --download-section \"*{range}\" --max-filesize 100M --force-overwrites");
        logger.LogInformation("Download completed: {filePath}", filePath);
    }
}