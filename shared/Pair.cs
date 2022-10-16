using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Shared;

public class Pair : IPEndPoint
{
    
    public File? File { get; set; }
    
    public int MTU { get; set; }
    
    public Pair(File file, long address, int port) : base(address, port)
    {
        this.File = file;
    }

    public Pair(File file, IPAddress address, int port) : base(address, port)
    {
        this.File = file;
    }
    
    public Pair(IMessageHandler handler, IPAddress address, int port) : base(address, port)
    {
        this.File = null;
        this.FindMTU(handler);
    }

    private void FindMTU(IMessageHandler handler)
    {
        int mtu = 1000;
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

        this.MTU = mtu;

    }

    public IMessage StartSync(IMessageHandler handler)
    {
        // client side
        Pair mySide = (Pair)handler.Self;
        return new MessageSyncInit(mySide, this, mySide.File!.Hash, mySide.File.ModifiedAt);
    }
}