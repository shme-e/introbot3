using System.Diagnostics;
using Microsoft.Extensions.Logging;
using NetCord.Gateway;
using NetCord.Gateway.Voice;
using NetCord.Logging;

namespace IntroBot3.Services;

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

    public async Task AddThemeToQueue(GatewayClient client, Theme theme)
    {
        if (!theme.Exists()) return;

        _themeQueue.Enqueue(theme);
        
        if (_themeQueue.Count == 1)
        {
            await ProcessQueue(client);
        }
    }

    private async Task ProcessQueue(GatewayClient client)
    {
        ulong? _currentChannelId = null;
        OpusEncodeStream? _stream = null;
        VoiceClient? _voiceClient = null;

        while (_themeQueue.Count > 0)
        {
            try
            {
                var theme = _themeQueue.Peek();

                if (_stream == null || _voiceClient == null || _currentChannelId == null || _currentChannelId != theme.ChannelId)
                {
                    if (_voiceClient != null)
                    {
                        await _voiceClient.CloseAsync();
                        _voiceClient.Dispose();
                    }

                    _stream?.Dispose();

                    _voiceClient = await client.JoinVoiceChannelAsync(theme.GuildId, theme.ChannelId, new VoiceClientConfiguration
                    {
                        Logger = new ConsoleLogger()
                    });
                    _currentChannelId = theme.ChannelId;

                    await Task.Delay(250);
                    await _voiceClient.StartAsync();
                    await Task.Delay(250);
                    await _voiceClient.EnterSpeakingStateAsync(new SpeakingProperties(SpeakingFlags.Microphone));

                    var outStream = _voiceClient.CreateOutputStream();

                    _stream = new OpusEncodeStream(outStream, PcmFormat.Short, VoiceChannels.Stereo, OpusApplication.Audio);
                }

                ProcessStartInfo startInfo = new("ffmpeg")
                {
                    RedirectStandardOutput = true,
                };
                var arguments = startInfo.ArgumentList;

                // Specify the input
                arguments.Add("-i");
                arguments.Add(theme.FilePath);

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

                logger.LogInformation("Playing theme: {ThemeFilePath} in Guild: {GuildId}, Channel: {ChannelId}", theme.FilePath, theme.GuildId, theme.ChannelId);

                await ffmpeg.StandardOutput.BaseStream.CopyToAsync(_stream);

                await _stream.FlushAsync();

                if (_themeQueue.Count == 1)
                {
                    await Task.Delay(5000);
                }

                if (_themeQueue.Count == 1)
                {
                    await _voiceClient.CloseAsync();
                    _voiceClient.Dispose();
                    _voiceClient = null;

                    _stream.Dispose();
                    _stream = null;

                    _currentChannelId = null;

                    await client.UpdateVoiceStateAsync(new VoiceStateProperties(theme.GuildId, null));
                }
            } catch (Exception)
            {
                _themeQueue.Dequeue();
                _voiceClient?.Dispose();
                _stream?.Dispose();
                throw;
            }
        }
    }
}