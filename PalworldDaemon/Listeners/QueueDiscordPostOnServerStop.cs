using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
using PalworldDaemon.Events;
using PalworldDaemon.Invocables;

namespace PalworldDaemon.Listeners;

public class QueueDiscordPostOnServerStop(IQueue queue) : IListener<ServerStopped>
{
    public Task HandleAsync(ServerStopped broadcasted)
    {
        queue.QueueInvocableWithPayload<PostToDiscord, string>("Palworld server stopped!");
        return Task.CompletedTask;
    }
}