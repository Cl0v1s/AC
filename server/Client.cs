using System.Net.Sockets;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class Client
{
    private static readonly List<Village> Villages = new List<Village>();

    private delegate void OnPushReceiveHandler(Client sender, MessagePush push);

    private event OnPushReceiveHandler? OnPushReceived;
    private readonly TcpClient _socket;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Village? _village;

    public Client(TcpClient socket)
    {
        this._socket = socket;

        this._cancellationTokenSource = new CancellationTokenSource();
        this.Receive(this._cancellationTokenSource.Token);
    }

    private async void Receive(CancellationToken token)
    {
        Stream stream = this._socket.GetStream();
        int i;
        byte[] data = new byte[255];
        while ((i = stream.Read(data, 0, 255)) != 0)
        {
            Console.WriteLine("Received " + data.Length);
            Message msg = Message.Parse(data);
            this.Handle(msg);
        }
    }

    public void Send(Message message)
    {
        Stream stream = this._socket.GetStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        bw.Close();
    }

    private void Handle(Message message)
    {
        Console.WriteLine("Handling " + message.Type);
        if (message is MessageState state)
        {
            // someone joined the village
            if (this._village == null)
            {
                Village? village = Villages.Find(x => x.Password == state.Password);
                if (village == null)
                {
                    village = new Village(state.Password);
                    Villages.Add(village);
                }
                this._village = village;
                this._village.Clients.Add(this);
            }
            if (state.Hash != this._village.Hash && state.ModifiedAt >= this._village.ModifiedAt)
            {
                // we ask the new comer to send their version
                this.Send(new MessagePull());
            }
            else
            {
                // we send them the current village state
                this.Send(new MessageState(this._village.Password, this._village.ModifiedAt, this._village.Hash));
            }
        } else if (message is MessagePush push)
        {
            if (this._village == null) return;
            // we trigger event for eventual listeners
            this.OnPushReceived?.Invoke(this, push);
            // someone tried to push their version
            string hash = Message.ComputeHash(push.Content);
            if (hash != this._village.Hash && push.ModifiedAt >= this._village.ModifiedAt)
            {
                // the new version is newer than the the village one
                this._village.Hash = hash;
                this._village.ModifiedAt = push.ModifiedAt;
                if (this._village.Clients.Count > 1)
                {
                    // if we have at least a peer connected, we dont store the save on the server
                    // we instead mark this client as the one having the newest version
                    this._village.Newest = this;
                }
                else
                {
                    // else we store the version being sent until their is a newcomer
                    this._village.File = push.Content;
                }

                // we say to every clients to make a pull 
                this._village.Clients.ForEach((c) =>
                {
                    if (c == this) return;
                    c.Send(new MessageState(this._village.Password, this._village.ModifiedAt, this._village.Hash));
                });
            }
        } else if (message is MessagePull pull)
        {
            if (this._village == null) return;
            if (this._village.File != null)
            {
                // the version is stored in memory
                this.Send(new MessagePush(this._village.File));
            }
            else if(this._village.Newest != null)
            {
                // the version is not stored on server 
                void Handler(Client sender, MessagePush messagePush)
                {
                    // we transfer the received message push
                    this.Send(messagePush);
                    this._village.Newest.OnPushReceived -= Handler;
                }

                this._village.Newest.OnPushReceived += Handler;
                this._village.Newest.Send(new MessagePull());
            }
        }
    }
}