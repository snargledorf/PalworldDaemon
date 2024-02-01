using PalworldDaemon;

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

builder.Services.AddSingleton<IPalworldRCONConnection>(sp =>
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

builder.Services.AddSingleton<IPalworldServerController, PalworldServerController>();

builder.Services.AddHostedService<Worker>();

IHost host = builder.Build();
host.Run();