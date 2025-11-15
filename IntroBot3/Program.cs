
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using Microsoft.Extensions.Configuration;
using IntroBot3.Discord;
using IntroBot3.BotLogic.Services;
using IntroBot3.BotLogic.Settings;
using IntroBot3.BotLogic.Services.Events;

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddGatewayHandlers(typeof(ReadyHandler).Assembly)
    .AddApplicationCommands()
    .AddSingleton<HttpClientService>()
    .AddScoped<ExecutableService>()
    .AddScoped<YtDlpService>()
    .AddSingleton<StartThemeService>()
    .AddSingleton<ThemePlayerService>()
    .AddScoped<ReadyService>()
    .AddScoped<UserVoiceChannelMoveService>()
    .Configure<ExecutablesSettings>(builder.Configuration.GetSection("Executables"))
    .Configure<ThemeCacheSettings>(builder.Configuration.GetSection("ThemeCache"))
    .AddOptions<IDiscordOptions>();

builder.Configuration.AddUserSecrets<Program>();

using IHost host = builder.Build();
var lifetime = host.Services.GetRequiredService<IHostApplicationLifetime>();

lifetime.ApplicationStarted.Register(() =>
{
    Console.WriteLine("Introbot online!");
});
lifetime.ApplicationStopping.Register(() =>
{
    Console.WriteLine("Introbot closing...");
    Console.WriteLine("Stopping end");
});

host.AddModules(typeof(CommandModule).Assembly);

await host.RunAsync().ConfigureAwait(false);
