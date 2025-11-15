using IntroBot3.BotLogic;
using IntroBot3.BotLogic.Services;
using IntroBot3.Discord.Impl;
using NetCord.Services.ApplicationCommands;

namespace IntroBot3.Discord;

public class CommandModule(CommandService commandService) : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping", "Replies with Pong!")]
    public async Task PingAsync()
    {
        await CommandService.OnPingCommand(new CommandClient(this));
    }

    [SlashCommand("set_intro", "set a track to play on join")]
    public async Task SetIntroAsync(
        [SlashCommandParameter(Description = "youtube url")] string link,
        [SlashCommandParameter(Description = "format: mm:ss")] string start,
        [SlashCommandParameter(Description = "format: mm:ss or inf")] string end
    )
    {
        await SetThemeAsync(ThemeType.Intro, new Uri(link), start, end);
    }

    [SlashCommand("set_outro", "set a track to play on leave")]
    public async Task SetOutroAsync(
        [SlashCommandParameter(Description = "youtube url")] string link,
        [SlashCommandParameter(Description = "format: mm:ss")] string start,
        [SlashCommandParameter(Description = "format: mm:ss or inf")] string end
    )
    {
        await SetThemeAsync(ThemeType.Outro, new Uri(link), start, end);
    }

    private async Task SetThemeAsync(ThemeType themeType, Uri uri, string start, string end)
    {
        await commandService.OnThemeCommand(
            themeType: themeType,
            uri: uri,
            start: start,
            end: end,
            userId: Context.User.Id,
            new CommandClient(this)
        );
    }
}
