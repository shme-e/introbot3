namespace IntroBot3.BotLogic.Wrapper;

public interface IConnectionInfo
{
    Task CloseConnection();
    ulong GetCurrentChannelId();
    Task PlayFile(string path);
}