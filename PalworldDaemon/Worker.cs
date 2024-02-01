namespace PalworldDaemon;

public class Worker(ILogger<Worker> logger, IPalworldServerController palworldServerController, IDiscordClient discordClient) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await palworldServerController.RunPalworldServerAsync(stoppingToken)
                .ConfigureAwait(false);
            
            logger.LogInformation("Palworld server stopped, restarting");
            
            logger.LogDebug("Sending restart message to discord");
            await discordClient.SendMessageAsync("Palworld server stopped, restarting...", stoppingToken).ConfigureAwait(false);
        }
    }
}