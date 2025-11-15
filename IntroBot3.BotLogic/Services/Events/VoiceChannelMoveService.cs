using IntroBot3.BotLogic.Wrapper;

namespace IntroBot3.BotLogic.Services.Events;

public class UserVoiceChannelMoveService(StartThemeService startThemeService)
{
    public async Task OnUserVoiceChannelMove(IUser user, ulong? channelId, ulong guildId, IDiscordClient client)
    {
        if (user.IsBot())
        {
            return;
        }

        await startThemeService.HandleChannelMove(user.Id(), channelId, guildId, client);
    }
}