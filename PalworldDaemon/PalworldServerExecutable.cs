using System.Diagnostics;

namespace PalworldDaemon;

class PalworldServerExecutable(
    ILogger<PalworldServerExecutable> logger,
    string palworldServerExePath,
    string? launchArgs) : IPalworldServerExecutable
{
    public void EnsureRunning()
    {
        if (TryGetPalServerProcess(out _))
            return;
        
        logger.LogInformation("Starting a new server process");
        StartPalServerProcess();
    }

    public bool TryGetPalServerProcess(out Process? process)
    {
        process = Process.GetProcessesByName("PalServer").FirstOrDefault();
        return process is not null;
    }

    private void StartPalServerProcess()
    {
        var serverProcess = new Process();

        var processStartInfo = new ProcessStartInfo(palworldServerExePath)
        {
            CreateNoWindow = true,
            UseShellExecute = false,
            Arguments = launchArgs ?? ""
        };

        serverProcess.StartInfo = processStartInfo;

        logger.LogDebug("Starting Palworld server process");
        bool startedNew = serverProcess.Start();
        logger.LogDebug("Started new process: {newProcess}", startedNew);
    }
}