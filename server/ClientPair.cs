using System.Net;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class ClientPair : Pair
{
    public ClientPair(IMessageHandler handler, long address, int port) : base(handler, address, port)
    {
    }

    public ClientPair(IMessageHandler handler, IPAddress address, int port) : base(handler, address, port)
    {
    }

    public override void Handle(IMessageHandler handler, IMessage message)
    {
        List<IMessage> responses = new List<IMessage>() { new MessageCharlyResponse(handler.Self, this) };

        foreach (IPEndPoint pair in handler.Pairs)
        {
            if(pair.Address.ToString() == this.Address.ToString() && pair.Port == this.Port) continue;
            responses.Add(new MessageDiscover(handler.Self, this, pair));
            responses.Add(new MessageDiscover(handler.Self, pair, this));
        }


        foreach (var response in responses)
        {
            handler.Send(response, false);
        }
    }
}