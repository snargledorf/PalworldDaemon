using Coravel.Events.Interfaces;

namespace PalworldDaemon.Events;

public class ShutdownWarning : IEvent
{
    public TimeSpan ShutdownTime { get; init; }
}