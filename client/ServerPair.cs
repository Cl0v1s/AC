using System.Net;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

/// <summary>
/// Represent the server to which this client is connected
/// </summary>
public class ServerPair : Pair
{
    public ServerPair(IMessageHandler handler, long address, int port) : base(handler, address, port)
    {
    }

    public ServerPair(IMessageHandler handler, IPAddress address, int port) : base(handler, address, port)
    {
    }
    
    public void UpdateState(IMessageHandler handler)
    { 
        ClientPair? self = handler.Self as ClientPair;
        handler.Send(new MessageSyncState(self!, this, Config.Instance.Password, self!.File!.Hash, self.File.ModifiedAt, false), false);
    }

    /// <summary>
    /// Handle message coming from the server 
    /// </summary>
    /// <param name="handler">handler receiving the message</param>
    /// <param name="message">message received</param>
    public override void Handle(IMessageHandler handler, IMessage message)
    {
        if (message is MessageIdentity)
        {
            handler.Self.Address = message.To.Address;
            handler.Self.Port = message.To.Port;
            Console.WriteLine("This client is " + handler.Self.Address + ":" + handler.Self.Port);
        } else if (message is MessageDiscover)
        {
            Pair? clientPair = (handler as ClientMessageHandler)!.FindPair(message.ReplyTo!);
            if (clientPair == null)
            {
                clientPair = new ClientPair(handler, message.ReplyTo!.Address, message.ReplyTo.Port);
                handler.Pairs.Add(clientPair);
                Console.WriteLine("This client discovered " + clientPair.Address + ":" + clientPair.Port);
            }
        } else if (message is MessageBye)
        {
            Console.WriteLine("Server disconnected.");
        }
    }
}