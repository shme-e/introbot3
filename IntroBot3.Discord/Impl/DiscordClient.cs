using IntroBot3.BotLogic.Wrapper;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;

namespace IntroBot3.Discord.Impl;

public class DiscordClient(GatewayClient client) : IDiscordClient
{
    public async Task<IConnectionInfo> ConnectToVoiceChat(ulong guildId, ulong channelId)
    {
        var voiceClient = await client.JoinVoiceChannelAsync(guildId, channelId, new VoiceClientConfiguration
        {
            Logger = new ConsoleLogger()
        });
        var currentChannelId = channelId;

        await Task.Delay(250);
        await voiceClient.StartAsync();
        await Task.Delay(250);
        await voiceClient.EnterSpeakingStateAsync(new SpeakingProperties(SpeakingFlags.Microphone));

        var outStream = voiceClient.CreateOutputStream();

        var stream = new OpusEncodeStream(outStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);

        return new ConnectionInfo(currentChannelId, stream, voiceClient);
    }

    public async Task LeaveVoiceChat(ulong guildId)
    {
        await client.UpdateVoiceStateAsync(new VoiceStateProperties(guildId, null));
    }
}