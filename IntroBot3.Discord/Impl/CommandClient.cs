using IntroBot3.BotLogic.Wrapper;
using NetCord.Rest;
using NetCord.Services.ApplicationCommands;

namespace IntroBot3.Discord.Impl;

public class CommandClient(ApplicationCommandModule<ApplicationCommandContext> applicationCommandModule) : ICommandClient
{
    public async Task SendInitialResponse(string s)
    {
        _ = await applicationCommandModule.RespondAsync(InteractionCallback.Message(s));
    }

    public async Task UpdateResponse(string s)
    {
        _ = await applicationCommandModule.ModifyResponseAsync(m => m.Content = s);
    }
}