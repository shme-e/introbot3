using IntroBot3.Services;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IntroBot3;

public enum ThemeType
{
    Intro,
    Outro,
}

public static class ThemeTypeExtensions
{
    public static string GetThemeTypeString(this ThemeType themeType) => themeType switch
    {
        ThemeType.Intro => "intro",
        ThemeType.Outro => "outro",
        _ => throw new ArgumentOutOfRangeException(nameof(themeType), themeType, null)
    };
}

public class Commands(YtDlpService ytDlpService) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly YtDlpService _ytDlpService = ytDlpService;

    [SlashCommand("ping", "Replies with Pong!")]
    public async Task PingAsync()
    {
        await RespondAsync(InteractionCallback.Message("Pong!"));
    }

    [SlashCommand("set_intro", "set a track to play on join")]
    public async Task SetIntroAsync(
        [SlashCommandParameter(Description = "youtube url")] string url,
        [SlashCommandParameter(Description = "format: mm:ss")] string start,
        [SlashCommandParameter(Description = "format: mm:ss or inf")] string end
    )
    {
        await SetThemeAsync(ThemeType.Intro, url, start, end);
    }

    [SlashCommand("set_outro", "set a track to play on leave")]
    public async Task SetOutroAsync(
        [SlashCommandParameter(Description = "youtube url")] string url,
        [SlashCommandParameter(Description = "format: mm:ss")] string start,
        [SlashCommandParameter(Description = "format: mm:ss or inf")] string end
    )
    {
        await SetThemeAsync(ThemeType.Outro, url, start, end);
    }

    private async Task SetThemeAsync(ThemeType themeType, string url, string start, string end)
    {
        var range = $"{start}-{end}";
        await RespondAsync(InteractionCallback.Message($"downloading <{url}> between {range}"));

        try
        {
            var fileName = $"{themeType.GetThemeTypeString()}-{Context.User.Id}.opus";

            await _ytDlpService.DownloadFromUrl(url, range, fileName);

            await ModifyResponseAsync(m => m.Content = $"done <{url}> between {range}");
        } catch (Exception)
        {
            await ModifyResponseAsync(m => m.Content = "error");
            throw;
        }
    }
}