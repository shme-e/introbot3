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