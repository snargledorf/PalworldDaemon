namespace PalworldDaemon;

public interface IPalworldRCONConnection : IDisposable
{
    Task BroadcastAsync(string message, CancellationToken cancellationToken = default);
    Task ShutdownAsync(TimeSpan shutdownInTimeSpan, string message, CancellationToken cancellationToken = default);
}