namespace PalworldDaemon;

class PalworldServerController(ILogger<PalworldServerController> logger, IConfiguration configuration, IPalworldRCONConnection rconConnection, IDiscordClient discordClient, IPalworldServerExecutable palworldServerExecutable) : IPalworldServerController
{
    private readonly TimeSpan _restartTimeSpan = configuration.GetValue("RestartCycleTime", TimeSpan.FromHours(4));
    
    public async Task RunPalworldServerAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting Palworld Server");
        Task executableTask = palworldServerExecutable.RunAsync(cancellationToken);

        await discordClient.SendMessageAsync("The Palworld server has started!", cancellationToken).ConfigureAwait(false);
        
        logger.LogInformation("Waiting for {restartTimeSpan} to restart", _restartTimeSpan);
        Task delayTask = Task.Delay(_restartTimeSpan.Subtract(TimeSpan.FromMinutes(5)), cancellationToken);
        
        await Task.WhenAny(executableTask, delayTask).ConfigureAwait(false);
        
        // Check if the executable finished before the delay task
        // This means the executable crashed or was closed so there is no where to send commands
        if (!delayTask.IsCompleted)
        {
            logger.LogInformation("Executable closed before restart delay");
            await discordClient.SendMessageAsync("Palworld server crashed", cancellationToken).ConfigureAwait(false);
            return;
        }
        
        logger.LogDebug("Sending 5 minute warning to discord");
        await discordClient.SendMessageAsync("Server restarting in 5 minutes", cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Sending 5 minute warning to RCON");
        await rconConnection.BroadcastAsync("Server restarting in 5 minutes", cancellationToken).ConfigureAwait(false);

        logger.LogDebug("Waiting for 4 minutes before sending shutdown message");
        await Task.Delay(TimeSpan.FromMinutes(4), cancellationToken).ConfigureAwait(false);
            
        logger.LogDebug("Sending shutdown command and 1 minute warning message to RCON");
        await rconConnection.ShutdownAsync(TimeSpan.FromMinutes(1), "Server Restart In 1min LOGOUT NOW!!!", cancellationToken)
            .ConfigureAwait(false);

        await palworldServerExecutable.WaitForExitAsync(cancellationToken).ConfigureAwait(false);
    }
}