
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetCord.Hosting;
using NetCord.Hosting.Gateway;
using NetCord.Hosting.Services;
using NetCord.Hosting.Services.ApplicationCommands;
using dotenv.net;

DotEnv.Load();

var builder = Host.CreateApplicationBuilder(args);

builder.Services
    .AddDiscordGateway()
    .AddApplicationCommands()
    .AddOptions<IDiscordOptions>();

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

host.AddModules(typeof(Commands).Assembly);

await host.RunAsync();
