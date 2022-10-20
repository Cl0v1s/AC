using System.Net.Sockets;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class Client
{
    private static List<Village> Villages = new List<Village>();

    private TcpClient _client;
    private CancellationTokenSource _cancellationTokenSource;
    private Village? _village;

    public Client(TcpClient client)
    {
        this._client = client;

        this._cancellationTokenSource = new CancellationTokenSource();
        this.Receive(this._cancellationTokenSource.Token);
    }

    private async void Receive(CancellationToken token)
    {
        Stream stream = this._client.GetStream();
        while (true)
        {
            if (this._client.Available > 0)
            {
                int i;
                byte[] data = new byte[this._client.Available];
                while ((i = stream.Read(data)) != 0)
                {
                   Message msg = Message.Parse(data); 
                   this.Handle(msg);
                }
            }
            await Task.Delay(500, token);
        }
    }

    public void Send(Message message)
    {
        Stream stream = this._client.GetStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        bw.Close();
    }

    private void Handle(Message message)
    {
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
                // if there were only a peer connected before us, we ask them to sync again
                // TODO: NOT WHAT WE WANT
                if(this._village.Clients.Count == 1) this._village.Clients[0].Send(new MessagePull());
                this._village.Clients.Add(this);
            }
            if (state.Hash != this._village.Hash && state.ModifiedAt >= this._village.ModifiedAt)
            {
                this.Send(new MessagePull());
            }
        } else if (message is MessagePush push)
        {
            if (this._village == null) return;
            // someone tried to push their version
            string hash = Message.ComputeHash(push.Content);
            if (hash != this._village.Hash && push.ModifiedAt >= this._village.ModifiedAt)
            {
                this._village.Hash = hash;
                this._village.ModifiedAt = push.ModifiedAt;
                // if we have at least a peer connected, we dont store the save on the server
                if (this._village.Clients.Count > 1)
                {
                    this._village.Newest = this;
                }
                else
                {
                    this._village.File = push.Content;
                }
                // we say to every clients to make a pull 
                this._village.Clients.ForEach((c) =>
                {
                    if (c == this) return;
                    c.Send(new MessagePull());
                });
            }
        }
        
    }
}