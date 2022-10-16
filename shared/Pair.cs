using System.Net;

namespace AnimalCrossing.Shared;

public abstract class Pair : IPEndPoint
{
    protected Pair(IMessageHandler handler, long address, int port) : base(address, port)
    {
    }

    protected Pair(IMessageHandler handler, IPAddress address, int port) : base(address, port)
    {
    }
    
    public abstract IMessage[] Handle(IMessageHandler handler, IMessage message);
}