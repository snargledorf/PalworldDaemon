using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
using PalworldDaemon.Events;
using PalworldDaemon.Invocables;

namespace PalworldDaemon.Listeners;

public class QueueDiscordPostOnServerRestarting(IQueue queue) : IListener<ServerRestarting>
{
    public Task HandleAsync(ServerRestarting broadcasted)
    {
        queue.QueueInvocableWithPayload<PostToDiscord, string>("Palworld server restarting...");
        return Task.CompletedTask;
    }
}