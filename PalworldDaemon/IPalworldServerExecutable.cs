using System.Diagnostics;

namespace PalworldDaemon;

internal interface IPalworldServerExecutable
{
    void EnsureRunning();
    bool TryGetPalServerProcess(out Process? process);
}