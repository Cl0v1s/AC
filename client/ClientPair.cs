using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class ClientPair : Pair
{
    public Shared.File? File { get; set; }
    
    public int Mtu { get; set; }
    
    public ClientPair(IMessageHandler handler, Shared.File file, long address, int port) : base(handler, address, port)
    {
        this.File = file;
    }

    public ClientPair(IMessageHandler handler, Shared.File file, IPAddress address, int port) : base(handler, address, port)
    {
        this.File = file;
    }
    
    public ClientPair(IMessageHandler handler, IPAddress address, int port) : base(handler, address, port)
    {
        this.File = null;
        this.FindMtu(handler);
    }

    private void FindMtu(IMessageHandler handler)
    {
        int mtu = 10;
        bool found = false;
        do
        {
            byte[] data = new byte[mtu];
            try
            {
                handler.Client.Send(data, mtu, this);
                found = true;
                Console.WriteLine("MTU found " + mtu);
            }
            catch (SocketException e)
            {
                if (e.SocketErrorCode == SocketError.MessageSize)
                {
                    found = false;
                    mtu -= 10;
                }
                else
                {
                    throw e;
                }
            }
        } while (found == false);

        this.Mtu = mtu;

    }

    public override IMessage[] Handle(IMessageHandler handler, IMessage message)
    {
        return new IMessage[] { };
    }
}