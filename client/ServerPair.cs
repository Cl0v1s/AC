using System.Net;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class ServerPair : Pair
{
    public ServerPair(IMessageHandler handler, long address, int port) : base(handler, address, port)
    {
        this.Init(handler);
    }

    public ServerPair(IMessageHandler handler, IPAddress address, int port) : base(handler, address, port)
    {
        this.Init(handler);
    }

    private void Init(IMessageHandler handler)
    { 
        handler.Send(new MessageCharlyRequest(this, "bonjour"));   
    }

    public override IMessage[] Handle(IMessageHandler handler, IMessage message)
    {
        if (message is MessageCharlyResponse)
        {
            handler.Self.Address = message.To.Address;
            handler.Self.Port = message.To.Port;
            Console.WriteLine("This client is " + handler.Self.Address + ":" + handler.Self.Port);
        }

        return new IMessage[] { };
    }
}