using IntroBot3.BotLogic.Settings;
using IntroBot3.BotLogic.Wrapper;
using Microsoft.Extensions.Options;

namespace IntroBot3.BotLogic.Services;

public class StartThemeService(ThemePlayerService themePlayerService, IOptions<ThemeCacheSettings> options)
{
    private readonly Dictionary<ulong, ulong> userChannelMap = [];
    private readonly string themePath = options.Value.Path;
    private readonly ThemePlayerService themePlayerService = themePlayerService;

    public async Task HandleChannelMove(ulong userId, ulong? channelId, ulong guildId, IDiscordClient client)
    {
        if (channelId.HasValue)
        {
            userChannelMap[userId] = channelId.Value;
            await themePlayerService.AddThemeToQueue(client, new Theme(ThemeType.Intro, userId, channelId.Value, guildId, themePath));
        }
        else
        {
            if (userChannelMap.TryGetValue(userId, out var leftChannelId))
            {
                _ = userChannelMap.Remove(userId);
                await themePlayerService.AddThemeToQueue(client, new Theme(ThemeType.Outro, userId, leftChannelId, guildId, themePath));
            }
        }
    }
}