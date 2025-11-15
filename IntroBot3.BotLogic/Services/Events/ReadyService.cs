namespace IntroBot3.BotLogic.Services.Events;

public class ReadyService(ExecutableService executableService)
{
    public async Task OnReady()
    {
        await executableService.EnsureLibrariesExistAsync();
    }
}