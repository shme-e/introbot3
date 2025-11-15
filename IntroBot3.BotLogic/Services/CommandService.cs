using IntroBot3.BotLogic.Wrapper;

namespace IntroBot3.BotLogic.Services;

public class CommandService(YtDlpService ytDlpService)
{
    public static async Task OnPingCommand(ICommandClient commandClient)
    {
        await commandClient.SendInitialResponse("Pong!");
    }

    public async Task OnThemeCommand(ThemeType themeType, Uri uri, string start, string end, ulong userId, ICommandClient commandClient)
    {
        var range = $"{start}-{end}";
        await commandClient.SendInitialResponse($"downloading <{uri}> between {range}");

        try
        {
            var fileName = $"{themeType.GetThemeTypeString()}-{userId}.opus";

            await ytDlpService.DownloadFromUrl(uri, range, fileName);

            await commandClient.UpdateResponse($"done <{uri}> between {range}");
        }
        catch (Exception)
        {
            await commandClient.UpdateResponse("error");
            throw;
        }
    }
}
