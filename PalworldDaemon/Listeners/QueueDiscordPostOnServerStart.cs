using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
using PalworldDaemon.Events;
using PalworldDaemon.Invocables;

namespace PalworldDaemon.Listeners;

public class QueueDiscordPostOnServerStart(IQueue queue) : IListener<ServerStarted>
{
    public Task HandleAsync(ServerStarted broadcasted)
    {
        queue.QueueInvocableWithPayload<PostToDiscord, string>("The Palworld server has started!");
        return Task.CompletedTask;
    }
}