using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class ClientPair : Pair
{
    public Shared.File? File { get; set; }

    private int Mtu { get; set; }

    private CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    private bool Syncing { get; set; }
    private byte[][]? SyncParts { get; set; }
    
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

    public void NotifyLocalStateToThisPair(IMessageHandler handler)
    {
        ClientPair? self = handler.Self as ClientPair;
        handler.Send(new MessageSyncState(self!, this, self!.File!.Hash, self.File.ModifiedAt, false), true);
    }

    private void FindMtu(IMessageHandler handler)
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

        this.Mtu = mtu;

    }
    
    /// <summary>
    /// Start sync process with a foreign pair
    /// </summary>
    /// <param name="handler"></param>
    /// <param name="state"></param>
    /// <param name="token"></param>
    private async void Sync(IMessageHandler handler, MessageSyncState state, CancellationToken token)
    {
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
                // needResponse is false since it's handled here
                handler.Send(new MessageSyncRequest(handler.Self, this, this.Mtu, left.ToArray()), false);
            }
        }

        // place the different parts into the same byte array
        byte[] content = new byte[this.SyncParts!.Length * this.Mtu];
        for (int i = 0; i < this.SyncParts.Length; i++)
        {
            this.SyncParts[i].CopyTo(content, i * this.Mtu);
        }
        // since the resulting byte array length is not exactly correct we have to remove
        // NULL characters
        int u = content.Length - 1;
        while (content[u] == '\0')
        {
            u--;
        }
        Array.Resize(ref content, u + 1);
        
        this.SyncParts = null;
        this.Syncing = false;
        
        // something during transfer gone wrong
        if (Shared.File.CalculateHash(content) != state.Hash)
        {
            Console.WriteLine("Error during transfer. Hashs doesnt match.");
            return;
        }
        this.File = new Shared.File(state.Hash, state.ModifiedAt);
        this.File.Content = content;
        this.File!.Save(Config.Instance.SaveFile);
    }

    /// <summary>
    /// Handle messages. Remember than "this" here refers to the peer endpoint and "self" to this client
    /// </summary>
    /// <param name="handler">Message handler</param>
    /// <param name="message">Message to handle</param>
    public override void Handle(IMessageHandler handler, Message message)
    {
        ClientPair? self = handler.Self as ClientPair;
        
        // use class
        if (message is MessageSyncState compare)
        {
            Console.WriteLine("Hash (us/them): " + self!.File!.Hash + " vs " + compare.Hash);
            Console.WriteLine("ModifiedAt (us/them): " + self.File.ModifiedAt + " vs " + compare.ModifiedAt);

            // if our file is newer or the hash is the same or we are already syncing, do nothing but ack
            if (this.Syncing || self.File.Hash == compare.Hash || self.File.ModifiedAt > compare.ModifiedAt)
            {
                handler.Send(new Message(MessageTypes.Ok, self, this), false);
                return;
            };
            this._cancellationTokenSource = new CancellationTokenSource();
            this.Sync(handler, compare, this._cancellationTokenSource.Token);
            // needResponse is false since it's handled here
            handler.Send(new MessageSyncRequest(self, this, this.Mtu), false);
        } else if (message is MessageSyncRequest request)
        {
            byte[][] parts = self!.File!.Split(request.Mtu);
            Message[] responses;
            if (request.PartsToSend == null)
            {
                responses = new Message[parts.Length];
                for (int i = 0; i < parts.Length; i++)
                {
                    responses[i] = new MessageSyncResponse(self, this, i, parts[i], parts.Length);
                }
            }
            else
            {
                responses = new Message[request.PartsToSend.Length];
                for (int i = 0; i < request.PartsToSend.Length; i++)
                {
                    int index = (int)request.PartsToSend[i];
                    responses[i] = new MessageSyncResponse(self, this, index, parts[index], parts.Length);
                }
            }
            foreach (var response in responses)
            {
                handler.Send(response, false);
            }
        } else if (message is MessageSyncResponse response)
        {
            this.SyncParts ??= new byte[response.Length][];
            this.SyncParts[response.Index] = response.Content;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (this.SyncParts.Any(x => x == null) == false)
            {
                this._cancellationTokenSource.Cancel();
            }
        } else if (message is MessageBye)
        {
            handler.Pairs.Remove(this);
            Console.WriteLine(this.Address +":"+this.Port+" disconnected");
        }
    }
}