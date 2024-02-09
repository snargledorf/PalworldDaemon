using Coravel;
using Coravel.Events.Interfaces;
using PalworldDaemon;
using PalworldDaemon.Events;
using PalworldDaemon.Invocables;
using PalworldDaemon.Listeners;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

int rconPort = builder.Configuration.GetValue("RCONPort", 25575);
var adminPassword = builder.Configuration.GetValue<string>("AdminPassword");

if (string.IsNullOrWhiteSpace(adminPassword))
    throw new InvalidOperationException("AdminPassword is required");

var palworldServerExePath = builder.Configuration.GetValue<string>("PalworldServerExe");
if (string.IsNullOrEmpty(palworldServerExePath))
    throw new InvalidOperationException("Server exe is required");

builder.Services.AddTransient<IPalworldServerExecutable, PalworldServerExecutable>(sp =>
    new PalworldServerExecutable(sp.GetRequiredService<ILogger<PalworldServerExecutable>>(), palworldServerExePath));

builder.Services.AddScoped<IPalworldRCONConnection>(sp =>
    new PalworldRconConnection(rconPort, adminPassword, sp.GetRequiredService<ILogger<PalworldRconConnection>>()));

var discordWebhookUrl = builder.Configuration.GetValue<string>("DiscordWebHookUrl");
if (!string.IsNullOrWhiteSpace(discordWebhookUrl))
{
    builder.Services.AddHttpClient<IDiscordClient, DiscordClient>(config =>
    {
        config.BaseAddress = new Uri(discordWebhookUrl);
    });
}
else
{
    builder.Services.AddSingleton<IDiscordClient, DummyDiscordClient>();
}

builder.Services.AddTransient<ShutdownPalworldServerJob>();

builder.Services.AddQueue();
builder.Services.AddTransient<PostToDiscord>();
builder.Services.AddTransient<SendRCONMessage>();

builder.Services.AddEvents();
builder.Services.AddTransient<QueueDiscordPostOnServerStart>();
builder.Services.AddTransient<QueueDiscordPostOnServerStop>();
builder.Services.AddTransient<QueueDiscordPostOnServerRestarting>();
builder.Services.AddTransient<QueueDiscordPostOnDaemonStopped>();
builder.Services.AddTransient<QueueDiscordPostOnShutdownWarning>();
builder.Services.AddTransient<QueueRCONMessageOnShutdownWarning>();

builder.Services.AddScheduler();
string cronSchedule = builder.Configuration.GetValue<string>("RestartCronSchedule") ?? throw new ArgumentNullException("RestartCronSchedule");

builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();

host.Services.UseScheduler(scheduler =>
{
    scheduler.Schedule<ShutdownPalworldServerJob>()
        .Cron(cronSchedule)
        .Zoned(TimeZoneInfo.Local);
});

IEventRegistration eventRegistration = host.Services.ConfigureEvents();

eventRegistration
    .Register<ServerStarted>()
    .Subscribe<QueueDiscordPostOnServerStart>();

eventRegistration
    .Register<ServerStopped>()
    .Subscribe<QueueDiscordPostOnServerStop>();

eventRegistration
    .Register<ServerRestarting>()
    .Subscribe<QueueDiscordPostOnServerRestarting>();

eventRegistration
    .Register<DaemonStopped>()
    .Subscribe<QueueDiscordPostOnDaemonStopped>();

eventRegistration
    .Register<ShutdownWarning>()
    .Subscribe<QueueDiscordPostOnShutdownWarning>()
    .Subscribe<QueueRCONMessageOnShutdownWarning>();

host.Run();