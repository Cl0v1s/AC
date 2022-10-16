using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class ClientPair : Pair
{
    public Shared.File? File { get; set; }
    
    public int Mtu { get; set; }
    
    public bool Syncing { get; set; }
    public byte[][]? SyncParts { get; set; }
    
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
        this.InitRemote(handler);
    }

    private void InitRemote(IMessageHandler handler)
    {
        ClientPair? self = handler.Self as ClientPair;
        handler.Send(new MessageSyncCompare(self!, this, self!.File!.Hash, self.File.ModifiedAt));
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
        ClientPair? self = handler.Self as ClientPair;
        if (message is MessageSyncCompare compare)
        {
            Console.WriteLine("Hash (us/them): " + self!.File!.Hash + " vs " + compare.Hash);
            Console.WriteLine("ModifiedAt (us/them): " + self.File.ModifiedAt + " vs " + compare.ModifiedAt);

            if (self.File.Hash == compare.Hash || self.File.ModifiedAt > compare.ModifiedAt) return new IMessage[] { };
            
            // need to sync
            this.Syncing = true;
            return new IMessage[] { new MessageSyncRequest(self, this, this.Mtu) };
        } else if (message is MessageSyncRequest request)
        {
            byte[][] parts = self!.File!.Split(request.MTU);
            IMessage[] responses;
            if (request.PartsToSend == null)
            {
                responses = new IMessage[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    responses[i] = new MessageSyncResponse(self, this, i, parts[i], parts.Length);
                }
            }
            else
            {
                responses = new IMessage[request.PartsToSend.Length];
                for (int i = 0; i < request.PartsToSend.Length; i++)
                {
                    int index = (int)request.PartsToSend[i];
                    responses[i] = new MessageSyncResponse(self, this, index, parts[index], parts.Length);
                }
            }

            return responses;
        }
        
        return new IMessage[] { };
    }
}