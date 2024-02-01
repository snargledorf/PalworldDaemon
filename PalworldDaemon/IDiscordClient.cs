namespace PalworldDaemon;

public interface IDiscordClient
{
    Task SendMessageAsync(string message, CancellationToken cancellationToken = default);
}