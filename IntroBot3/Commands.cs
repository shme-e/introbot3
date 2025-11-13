using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

public class Commands : ApplicationCommandModule<ApplicationCommandContext>
{
    [SlashCommand("ping", "Replies with Pong!")]
    public async Task PingAsync()
    {
        await RespondAsync(InteractionCallback.Message("Pong!"));

        await Task.Delay(5000);

        await ModifyResponseAsync(m => m.Content = "well...");
    }
}