namespace IntroBot3.BotLogic.Wrapper;

public interface IUser
{
    bool IsBot();
    ulong Id();
}