using System.Diagnostics;
using System.Runtime.ExceptionServices;
using Coravel.Events.Interfaces;
using PalworldDaemon.Events;

namespace PalworldDaemon;

public class Worker(ILogger<Worker> logger, IDispatcher dispatcher, IPalworldServerExecutable palworldServerExecutable) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                logger.LogInformation("Starting Palworld Server");
                palworldServerExecutable.EnsureRunning();

                if (palworldServerExecutable.TryGetPalServerProcess(out Process? serverProcess))
                {
                    await dispatcher.Broadcast(new ServerStarted());
                    await serverProcess!.WaitForExitAsync(stoppingToken).ConfigureAwait(false);
                    await dispatcher.Broadcast(new ServerStopped());
                }
                else
                {
                    logger.LogDebug("Palworld server process not found");
                }

                if (stoppingToken.IsCancellationRequested)
                    continue;

                logger.LogInformation("Palworld server stopped, restarting");
                await dispatcher.Broadcast(new ServerRestarting());
            }
        }
        finally
        {
            await dispatcher.Broadcast(new DaemonStopped());
        }
    }
}