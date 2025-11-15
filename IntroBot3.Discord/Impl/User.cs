using IntroBot3.BotLogic.Wrapper;
using NetCord;

namespace IntroBot3.Discord.Impl;

public class User(GuildUser user) : IUser
{
    public ulong Id()
    {
        return user.Id;
    }

    public bool IsBot()
    {
        return user.IsBot;
    }
}