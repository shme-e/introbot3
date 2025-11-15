using IntroBot3.BotLogic.Services.Events;
using IntroBot3.Discord.Impl;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace IntroBot3.Discord;

public class ReadyHandler(ReadyService readyService) : IReadyGatewayHandler
{
    public async ValueTask HandleAsync(ReadyEventArgs arg)
    {
        await readyService.OnReady();
    }
}

public class UserVoiceStateUpdateHandler(UserVoiceChannelMoveService userVoiceChannelMoveService, GatewayClient client) : IVoiceStateUpdateGatewayHandler
{
    public async ValueTask HandleAsync(VoiceState arg)
    {
        if (arg.User == null)
        {
            return;
        }

        await userVoiceChannelMoveService.OnUserVoiceChannelMove(new User(arg.User), arg.ChannelId, arg.GuildId, new DiscordClient(client));
    }
}