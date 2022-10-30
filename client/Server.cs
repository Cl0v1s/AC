using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class Server
{
    private readonly TcpClient _socket;
    private readonly CancellationTokenSource _cancellationTokenSource;
    
    public Server()
    {
        this._socket = new TcpClient(Config.Instance.ServerAddress, Config.Instance.ServerPort);
        this._cancellationTokenSource = new CancellationTokenSource();
        
        this.Send(new MessageState(Village.Instance.Password, Village.Instance.ModifiedAt, Village.Instance.Hash));
        Village.Instance.FileChanged += this.OnVillageChanged;

        AppDomain.CurrentDomain.ProcessExit += new EventHandler(this.OnExit);
        Console.CancelKeyPress += new ConsoleCancelEventHandler(this.OnExit);
    }

    ~Server()
    {
        Village.Instance.FileChanged -= this.OnVillageChanged;
    }

    public void Stop()
    {
        this.OnExit(null, EventArgs.Empty);
    }

    private void OnExit(object? obj, EventArgs args)
    {
        this._cancellationTokenSource.Cancel();
        this._socket.Close();
    }

    private void OnVillageChanged()
    {
        this.Send(new MessageState(Village.Instance.Password, Village.Instance.ModifiedAt, Village.Instance.Hash));
    }

    public Task Start()
    {
        return Task.Run(() =>
        {
            Stream stream = this._socket.GetStream();
            byte[] length = new byte[4];
            int i = 0;
            int r;
            while (this._cancellationTokenSource.IsCancellationRequested == false && (r = stream.ReadByte()) != -1)
            {
                length[i] = (byte)r;
                i++;
                if (i < length.Length) continue;
                byte[] data = new byte[BitConverter.ToInt32(length)];
                Console.Write("Receiving " + data.Length + " bytes...");
                length = new byte[4];
                i = 0;
                if (data.Length == 0)
                {
                    Console.WriteLine("Pass");
                    continue;
                };
                while (i < data.Length && (r = stream.ReadByte()) != -1)
                {
                    data[i] = (byte)r;
                    i++;
                }
                i = 0;
                Console.WriteLine("Done");
                Message? msg = Message.Parse(data);
                if(msg == null) continue;
                this.Handle(msg);
            }
        });
    }

    private void Send(Message message)
    {
        MemoryStream memory = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(memory);
        message.Serialize(bw);

        Stream stream = this._socket.GetStream();
        byte[] data = memory.ToArray();
        stream.Write(BitConverter.GetBytes((Int32)data.Length));
        stream.Write(data);
        Console.WriteLine("Sent " + message.Type);
    }

    private async void Handle(Message message)
    {
        Console.WriteLine("Handled " + message.Type);
        if (message is MessageState state)
        {
            if (state.Hash == Village.Instance.Hash || state.ModifiedAt < Village.Instance.ModifiedAt) return;
            this.Send(new MessagePull());
        } else if (message is MessagePull)
        {
            byte[] content = await Village.Instance.GetContent();
            this.Send(new MessagePush(content, Village.Instance.ModifiedAt));
        } else if (message is MessagePush push)
        {
            string hash = Message.ComputeHash(push.Content);
            if (hash != Village.Instance.Hash && push.ModifiedAt < Village.Instance.ModifiedAt)
            {
                // we are newer than received, so we answer with our village
                this.Send(new MessageState(Village.Instance.Password, Village.Instance.ModifiedAt, Village.Instance.Hash));
                return;
            }
            // else we are out of date so we replace local village
            Village.Instance.Save(push.Content);
            
            Console.WriteLine(Village.Instance.Hash + " -> " + Village.Instance.ModifiedAt + "");
        }
    }
}