namespace IntroBot3.BotLogic.Wrapper;

public interface ICommandClient
{
    Task SendInitialResponse(string s);

    Task UpdateResponse(string s);
}