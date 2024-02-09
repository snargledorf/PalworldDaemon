using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
using PalworldDaemon.Events;
using PalworldDaemon.Invocables;

namespace PalworldDaemon.Listeners;

public class QueueDiscordPostOnDaemonStopped(IQueue queue) : IListener<DaemonStopped>
{
    public Task HandleAsync(DaemonStopped broadcasted)
    {
        queue.QueueInvocableWithPayload<PostToDiscord, string>("Palworld server daemon stopped!");
        return Task.CompletedTask;
    }
}