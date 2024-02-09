using System.Diagnostics;

namespace PalworldDaemon;

public interface IPalworldServerExecutable
{
    void EnsureRunning();
    bool TryGetPalServerProcess(out Process? process);
}