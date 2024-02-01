namespace PalworldDaemon;

public interface IPalworldServerController
{
    Task RunPalworldServerAsync(CancellationToken cancellationToken = default);
}