using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Shared;

public class Pair : IPEndPoint
{
    public File? File { get; set; }
    
    public int Mtu { get; set; }
    
    public Task Syncing { get; set; }
    
    public Pair(File file, long address, int port) : base(address, port)
    {
        this.File = file;
        this.Syncing = Task.CompletedTask;
    }

    public Pair(File file, IPAddress address, int port) : base(address, port)
    {
        this.File = file;
        this.Syncing = Task.CompletedTask;
    }
    
    public Pair(IMessageHandler handler, IPAddress address, int port) : base(address, port)
    {
        this.File = null;
        this.Syncing = Task.CompletedTask;
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

    public IMessage[] StartSync(IMessageHandler handler)
    {
        // client side
        Pair mySide = (Pair)handler.Self;
        
        
        // and send first request
        return new IMessage[] { new MessageSyncRequest(mySide, this, this.Mtu) };
    }
}