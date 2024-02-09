using Coravel.Invocable;
using Coravel.Queuing.Interfaces;

namespace PalworldDaemon.Invocables;

public class PostToDiscord(IDiscordClient discordClient) : ICancellableTask, IInvocable, IInvocableWithPayload<string>
{
    public CancellationToken Token { get; set; }

    public string Payload { get; set; } = null!;
    
    public async Task Invoke()
    {
        await discordClient.SendMessageAsync(Payload, Token)
            .ConfigureAwait(false);
    }
}