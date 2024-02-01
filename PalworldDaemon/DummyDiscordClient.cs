using PalworldDaemon;

public class DummyDiscordClient(ILogger<DummyDiscordClient> logger) : IDiscordClient
{
    public Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Discord message: {message}", message);
        return Task.CompletedTask;
    }
}