using System.Diagnostics;
using IntroBot3.BotLogic.Wrapper;
using NetCord.Gateway.Voice;

namespace IntroBot3.Discord.Impl;

public class ConnectionInfo(ulong currentChannelId, OpusEncodeStream stream, VoiceClient voiceClient) : IConnectionInfo
{
    public async Task CloseConnection()
    {
        await voiceClient.CloseAsync();
        voiceClient.Dispose();
        await stream.DisposeAsync();
    }

    public ulong GetCurrentChannelId()
    {
        return currentChannelId;
    }

    public async Task PlayFile(string path)
    {
        ProcessStartInfo startInfo = new("ffmpeg")
        {
            RedirectStandardOutput = true,
        };
        var arguments = startInfo.ArgumentList;

        // Specify the input
        arguments.Add("-i");
        arguments.Add(path);

        // Set the logging level to quiet mode
        arguments.Add("-loglevel");
        arguments.Add("-8");

        // Set the number of audio channels to 2 (stereo)
        arguments.Add("-ac");
        arguments.Add("2");

        // Set the output format to 16-bit signed little-endian
        arguments.Add("-f");
        arguments.Add("s16le");

        // Set the audio sampling rate to 48 kHz
        arguments.Add("-ar");
        arguments.Add("48000");

        // Direct the output to stdout
        arguments.Add("pipe:1");

        // Start the FFmpeg process
        var ffmpeg = Process.Start(startInfo)!;

        await ffmpeg.StandardOutput.BaseStream.CopyToAsync(stream);

        await stream.FlushAsync();
    }
}