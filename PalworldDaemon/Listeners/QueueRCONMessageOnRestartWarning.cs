using Coravel.Events.Interfaces;
using Coravel.Queuing.Interfaces;
using Humanizer;
using PalworldDaemon.Events;
using PalworldDaemon.Invocables;

namespace PalworldDaemon.Listeners;

public class QueueRCONMessageOnShutdownWarning(IQueue queue) : IListener<ShutdownWarning>
{
    public Task HandleAsync(ShutdownWarning broadcasted)
    {
        queue.QueueInvocableWithPayload<SendRCONMessage, string>(
            $"Server restarting in {broadcasted.ShutdownTime.Humanize()}");
        return Task.CompletedTask;
    }
}