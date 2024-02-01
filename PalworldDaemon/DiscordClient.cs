using System.Net.Http.Json;

namespace PalworldDaemon;

internal class DiscordClient(ILogger<DiscordClient> logger, HttpClient httpClient) : IDiscordClient
{
    public async Task SendMessageAsync(string message, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Discord message: {message}", message);
        await httpClient.PostAsync(httpClient.BaseAddress, JsonContent.Create(new
        {
            content = message
        }), cancellationToken);
    }
}