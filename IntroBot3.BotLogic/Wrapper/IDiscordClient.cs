namespace IntroBot3.BotLogic.Wrapper;

public interface IDiscordClient
{
    Task<IConnectionInfo> ConnectToVoiceChat(ulong guildId, ulong channelId);
    Task LeaveVoiceChat(ulong guildId);
}