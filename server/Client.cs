using System.Diagnostics;
using System.Net.Sockets;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class Client
{
    private static readonly List<Village> Villages = new List<Village>();

    public delegate void OnPushReceiveHandler(Client sender, MessagePush push);

    public event OnPushReceiveHandler? OnPushReceived;
    private readonly TcpClient _socket;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private Village? _village;

    public Client(TcpClient socket)
    {
        this._socket = socket;
        this._cancellationTokenSource = new CancellationTokenSource();
    }

    ~Client()
    {
        this._socket.GetStream().Dispose();
        this._socket.Dispose();
        this._cancellationTokenSource.Dispose();
    }

    public async Task Start()
    {
        await this.Receive();
        this._village?.RemoveClient(this);
    }

    private Task Receive()
    {
        return Task.Run(() =>
        {
            while (this._cancellationTokenSource.IsCancellationRequested == false && this._socket.Connected)
            {
                Stream stream = this._socket.GetStream();
                stream.ReadTimeout = 5000;
                byte[] length = new byte[4];
                int i = 0;
                try
                {
                    int r;
                    while ((r = stream.ReadByte()) != -1)
                    {
                        length[i] = (byte)r;
                        i++;
                        if (i < length.Length) continue;
                        byte[] data = new byte[BitConverter.ToInt32(length)];
                        // Console.Write("Receiving " + data.Length + " bytes...");
                        length = new byte[4];
                        i = 0;
                        if (data.Length == 0)
                        {
                            // Console.WriteLine("Pass");
                            continue;
                        }

                        while (i < data.Length && (r = stream.ReadByte()) != -1)
                        {
                            data[i] = (byte)r;
                            i++;
                        }

                        i = 0;
                        // Console.WriteLine("Done");
                        Message? msg = Message.Parse(data);
                        if (msg == null) continue;
                        this.Handle(msg);
                    }
                }
                catch (IOException e)
                {
                    Debug.WriteLine("Error occured: ");
                    Debug.WriteLine(e);
                    // Read timeout is normal
                }

                // keep alive, after each readTimeout to see if client is still there
                // updates socket.Connected
                try
                {
                    this._socket.Client.Send(BitConverter.GetBytes((Int32)0));
                }
                catch (SocketException e)
                {
                    // Cant send, client probably disconnected
                    Debug.WriteLine("Error occured: ");
                    Debug.WriteLine(e);
                }
            }
        });
    }

    public void Send(Message message)
    {
        MemoryStream memory = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(memory);
        message.Serialize(bw);

        NetworkStream stream = this._socket.GetStream();
        byte[] data = memory.ToArray();
        stream.Write(BitConverter.GetBytes((Int32)data.Length));
        stream.Write(data);
        Console.WriteLine("Sent " + message.Type);
    }

    private async void Handle(Message message)
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
                this._village.AddClient(this, state.Hash, state.ModifiedAt);
            }
            this.Send(new MessageState(this._village.Password, this._village.ModifiedAt, this._village.Hash));
        } else if (message is MessagePush push)
        {
            // we trigger event for eventual listeners
            this.OnPushReceived?.Invoke(this, push);
        } else if (message is MessagePull pull)
        {
            if (this._village == null) return;
            // the version is stored in memory
            byte[] content = await this._village.GetContent();
            this.Send(new MessagePush(content, this._village.ModifiedAt));
        }
    }
}