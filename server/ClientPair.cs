using System.Net;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class ClientPair : Pair
{
    private string Password { get; set; }
    
    public ClientPair(IMessageHandler handler, long address, int port) : base(handler, address, port)
    {
    }

    public ClientPair(IMessageHandler handler, IPAddress address, int port) : base(handler, address, port)
    {
    }

    public override void Handle(IMessageHandler handler, Message message)
    {
        if (message is MessageCharly charly)
        {
            this.Password = charly.Password;
            List<Message> responses = new List<Message>() { new MessageIdentity(handler.Self, this) };

            foreach (IPEndPoint raw in handler.Pairs)
            {
                ClientPair pair = (raw as ClientPair)!;
                if (pair.Address.ToString() == this.Address.ToString() && pair.Port == this.Port || pair.Password != this.Password ) continue;
                responses.Add(new MessageDiscover(handler.Self, this, pair));
                responses.Add(new MessageDiscover(handler.Self, pair, this));
            }

            foreach (var response in responses)
            {
                handler.Send(response, false);
            }
        } else if (message is MessageBye)
        {
            handler.Pairs.Remove(this);
            Console.WriteLine(this.Address +":"+this.Port+" disconnected");
        }
    }
}