using Coravel.Invocable;
using Coravel.Queuing.Interfaces;

namespace PalworldDaemon.Invocables;

public class SendRCONMessage(IPalworldRCONConnection rconConnection) : ICancellableTask, IInvocable, IInvocableWithPayload<string>
{
    public CancellationToken Token { get; set; }
    
    public Task Invoke()
    {
        return rconConnection.BroadcastAsync(Payload, Token);
    }

    public string Payload { get; set; }
}