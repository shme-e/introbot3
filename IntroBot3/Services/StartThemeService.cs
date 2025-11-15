using IntroBot3.Settings;
using Microsoft.Extensions.Options;
using NetCord;
using NetCord.Gateway;

namespace IntroBot3.Services;

public class StartThemeService(ThemePlayerService themePlayerService, IOptions<ThemeCacheSettings> options)
{
    private readonly Dictionary<ulong, ulong> userChannelMap = new();
    private readonly string themePath = options.Value.Path;
    private readonly ThemePlayerService themePlayerService = themePlayerService;

    public async Task HandleChannelMove(ulong userId, ulong? channelId, ulong guildId, GatewayClient client)
    {
        if (channelId.HasValue)
        {
            // if this is a new join or a move to a diff channel
            // must be careful since this event triggers on eg unmuting
            if (!userChannelMap.TryGetValue(userId, out ulong value) || value != channelId.Value)
            {
                value = channelId.Value;
                userChannelMap[userId] = value;
                await themePlayerService.AddThemeToQueue(client, new Theme(ThemeType.Intro, userId, channelId.Value, guildId, themePath));
            }
        }
        else
        {
            if (userChannelMap.TryGetValue(userId, out var leftChannelId))
            {
                userChannelMap.Remove(userId);
                await themePlayerService.AddThemeToQueue(client, new Theme(ThemeType.Outro, userId, leftChannelId, guildId, themePath));
            }
        }
    }
}