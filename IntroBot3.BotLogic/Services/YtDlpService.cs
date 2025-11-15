using IntroBot3.BotLogic.Settings;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace IntroBot3.BotLogic.Services;

public class YtDlpService(ExecutableService executableService, IOptions<ThemeCacheSettings> options, ILogger<YtDlpService> logger)
{
    private readonly ExecutableService executableService = executableService;
    private readonly string themeCachePath = options.Value.Path;
    private readonly ILogger<YtDlpService> logger = logger;

    public async Task DownloadFromUrl(Uri uri, string range, string fileName)
    {
        await executableService.EnsureLibrariesExistAsync();
        await ExecutableService.CreateDirectoryIfNotExists(themeCachePath);

        var filePath = Path.Combine(themeCachePath, fileName);

        logger.LogInformation("Starting download from yt-dlp... for {Url} in range {Range} to {FilePath}", uri, range, filePath);
        await ExecutableService.Run("yt-dlp", $"-P theme-cache -o {filePath} -x --audio-format opus -q {uri} --download-section \"*{range}\" --max-filesize 100M --force-overwrites");
        logger.LogInformation("Download completed: {FilePath}", filePath);
    }
}