namespace PalworldDaemon;

internal interface IPalworldServerExecutable
{
    Task RunAsync(CancellationToken cancellationToken = default);
    Task WaitForExitAsync(CancellationToken cancellationToken = default);
}