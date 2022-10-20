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

    private void Send(Message message)
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
            }
            if (state.Hash != this._village.Hash && state.ModifiedAt >= this._village.ModifiedAt)
            {
                this.Send(new MessagePull());
            }
        } else if (message is MessagePull pull)
        {
            // someone asked to pull the best version 
        }
        
    }
}