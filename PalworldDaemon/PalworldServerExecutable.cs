using System.Diagnostics;

namespace PalworldDaemon;

class PalworldServerExecutable(ILogger<PalworldServerExecutable> logger, string palworldServerExePath) : IPalworldServerExecutable
{
    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Process? serverProcess = Process.GetProcessesByName("PalServer-Win64-Test-Cmd").FirstOrDefault();
        if (serverProcess is null)
        {
            logger.LogInformation("Starting a new server process");
            serverProcess = new Process();

            var processStartInfo = new ProcessStartInfo(palworldServerExePath)
            {
                CreateNoWindow = true,
                UseShellExecute = false
            };

            serverProcess.StartInfo = processStartInfo;

            logger.LogDebug("Starting Palworld server process");
            bool startedNew = serverProcess.Start();
            logger.LogDebug("Started new process: {newProcess}", startedNew);
        }
        else
        {
            logger.LogInformation("Found an existing server process");
        }

        logger.LogDebug("Waiting for server process to exit");
        await WaitForExitAsync(serverProcess, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("Server process exited");
    }

    public Task WaitForExitAsync(CancellationToken cancellationToken = default)
    {
        return WaitForExitAsync(Process.GetProcessesByName("PalServer-Win64-Test-Cmd").FirstOrDefault(), cancellationToken);
    }

    private static Task WaitForExitAsync(Process? process, CancellationToken cancellationToken)
    {
        return process?.WaitForExitAsync(cancellationToken) ?? Task.CompletedTask;
    }
}