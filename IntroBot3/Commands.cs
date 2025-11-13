using IntroBot3.Services;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IntroBot3;

public class Commands(YtDlpService ytDlpService) : ApplicationCommandModule<ApplicationCommandContext>
{
    private readonly YtDlpService _ytDlpService = ytDlpService;

    [SlashCommand("ping", "Replies with Pong!")]
    public async Task PingAsync()
    {
        await RespondAsync(InteractionCallback.Message("Pong!"));

        await _ytDlpService.DownloadFromUrl("https://www.youtube.com/watch?v=nud2TQNahaU", "00:01:00-00:02:00", "test.opus");

        await ModifyResponseAsync(m => m.Content = "well...");
    }
}