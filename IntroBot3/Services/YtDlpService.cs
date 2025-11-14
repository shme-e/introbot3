using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace IntroBot3.Services;

public class YtDlpService(ExecutableService executableService)
{
    // join to directory of executable
    private readonly string folderPath = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "yt-dlp-files";
    private readonly string exePath = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar + "yt-dlp-files" + Path.DirectorySeparatorChar + "yt-dlp";
    private readonly ExecutableService executableService = executableService;

    public async Task<Result> DownloadFromUrl(string url, string range, string fileName)
    {
        var ytDlpExists = await executableService.EnsureLibrariesExistAsync();

        return Result.Success;
        // if (ytDlpExists == Result.Fail)
        // {
        //     return Result.Fail;
        // }

        // var ytDlpResult = await Run(exePath, $" -P theme-cache -o {fileName} -x --audio-format opus -q {url} --download-section \"*{range}\" --max-filesize 100M --force-overwrites");
        // return ytDlpResult;
    }
}