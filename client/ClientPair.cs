using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class ClientPair : Pair
{
    public Shared.File? File { get; set; }
    
    public int Mtu { get; set; }

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    
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

    private async void Sync(IMessageHandler handler, MessageSyncCompare compare, CancellationToken token)
    {
        this.File = new Shared.File(compare.Hash, compare.ModifiedAt);
        this.Syncing = true;
        while (token.IsCancellationRequested == false)
        {
            await Task.Delay(3000);
            if (this.SyncParts == null) continue;
            List<byte> left = new List<byte>();
            for (int i = 0; i < this.SyncParts.Length; i++)
            {
                if (this.SyncParts[i].Length == 0)
                {
                    left.Add((byte)i);
                }
            }
            if (left.Count > 0)
            {
                handler.Send(new MessageSyncRequest(handler.Self, this, this.Mtu, left.ToArray()));
            }
        }
        
        Console.WriteLine("All parts are here");

        this.File.Content = new byte[this.SyncParts!.Length * this.Mtu];
        for (int i = 0; i < this.SyncParts.Length; i++)
        {
            this.SyncParts[i].CopyTo(this.File.Content, i * this.Mtu);
        }

        Console.WriteLine(System.Text.Encoding.ASCII.GetString(this.File.Content));

        this.SyncParts = null;
        this.Syncing = false;
    }

    public override IMessage[] Handle(IMessageHandler handler, IMessage message)
    {
        ClientPair? self = handler.Self as ClientPair;
        if (message is MessageSyncCompare compare)
        {
            Console.WriteLine("Hash (us/them): " + self!.File!.Hash + " vs " + compare.Hash);
            Console.WriteLine("ModifiedAt (us/them): " + self.File.ModifiedAt + " vs " + compare.ModifiedAt);

            if (self.File.Hash == compare.Hash || self.File.ModifiedAt > compare.ModifiedAt) return new IMessage[] { };
            
            this.Sync(handler, compare, this._cancellationTokenSource.Token);
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
        } else if (message is MessageSyncResponse response)
        {
            if (this.Syncing == false) return new IMessage[] { };
            this.SyncParts ??= new byte[response.Length][];
            this.SyncParts[response.Index] = response.Content;
            if(this.SyncParts.Any(x => x == null) == false) this._cancellationTokenSource.Cancel();
        }
        
        return new IMessage[] { };
    }
}