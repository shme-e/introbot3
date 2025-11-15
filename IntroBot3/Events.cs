using IntroBot3.Services;
using NetCord.Gateway;
using NetCord.Hosting.Gateway;

namespace IntroBot3;

public class ReadyHandler(ExecutableService executableService) : IReadyGatewayHandler
{
    private readonly ExecutableService executableService = executableService;

    public async ValueTask HandleAsync(ReadyEventArgs args)
    {
        await executableService.EnsureLibrariesExistAsync();
    }
}

public class UserVoiceStateUpdateHandler(StartThemeService startThemeService, GatewayClient client) : IVoiceStateUpdateGatewayHandler
{
    public async ValueTask HandleAsync(VoiceState args)
    {
        if (args.User == null)
        {
            return;
        }

        if (args.User.IsBot == true)
        {
            return;
        }

        await startThemeService.HandleChannelMove(args.UserId, args.ChannelId, args.GuildId, client);
    }
}