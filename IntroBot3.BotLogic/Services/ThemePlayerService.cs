using IntroBot3.BotLogic.Wrapper;
using Microsoft.Extensions.Logging;

namespace IntroBot3.BotLogic.Services;

public class Theme(ThemeType type, ulong userId, ulong channelId, ulong guildId, string basePath)
{
    public string FilePath { get; set; } = Path.Combine(basePath, $"{type.GetThemeTypeString()}-{userId}.opus");
    public ulong ChannelId { get; set; } = channelId;
    public ulong GuildId { get; set; } = guildId;

    public bool Exists()
    {
        return File.Exists(FilePath);
    }
}

public class ThemePlayerService(ILogger<ThemePlayerService> logger)
{
    private readonly Queue<Theme> _themeQueue = new();

    public async Task AddThemeToQueue(IDiscordClient client, Theme theme)
    {
        if (!theme.Exists())
        {
            return;
        }

        _themeQueue.Enqueue(theme);

        if (_themeQueue.Count == 1)
        {
            await ProcessQueue(client);
        }
    }

    private async Task ProcessQueue(IDiscordClient client)
    {
        IConnectionInfo? connectionInfo = null;

        while (_themeQueue.Count > 0)
        {
            try
            {
                var theme = _themeQueue.Peek();

                if (connectionInfo == null || connectionInfo.GetCurrentChannelId() != theme.ChannelId)
                {
                    if (connectionInfo != null)
                    {
                        await connectionInfo.CloseConnection();
                    }

                    connectionInfo = await client.ConnectToVoiceChat(theme.GuildId, theme.ChannelId);
                }

                logger.LogInformation("Playing theme: {ThemeFilePath} in Guild: {GuildId}, Channel: {ChannelId} Queue Length: {QueueLength}", theme.FilePath, theme.GuildId, theme.ChannelId, _themeQueue.Count);

                await connectionInfo.PlayFile(theme.FilePath);

                if (_themeQueue.Count == 1)
                {
                    await Task.Delay(5000);
                }

                if (_themeQueue.Count == 1)
                {
                    await connectionInfo.CloseConnection();

                    connectionInfo = null;

                    await client.LeaveVoiceChat(theme.GuildId);
                }

                _ = _themeQueue.Dequeue();
            }
            catch (Exception)
            {
                _ = _themeQueue.Dequeue();
                if (connectionInfo != null)
                {
                    await connectionInfo.CloseConnection();
                }
                throw;
            }
        }
    }
}