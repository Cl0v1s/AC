using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System.Linq;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class Server
{
    private readonly TcpClient _socket;
    private readonly AppViewModel _context;
    
    public Server(AppViewModel context)
    {
        this._context = context;
        this._socket = new TcpClient(Config.Instance.ServerAddress, Config.Instance.ServerPort);
        
        this.Send(new MessageState(Village.Instance.Password, Village.Instance.ModifiedAt, Village.Instance.Hash));
        Village.Instance.FileChanged += this.OnVillageChanged;
        
    }

    ~Server()
    {
        Village.Instance.FileChanged -= this.OnVillageChanged;
    }

    private void OnVillageChanged()
    {
        this.Send(new MessageState(Village.Instance.Password, Village.Instance.ModifiedAt, Village.Instance.Hash));
    }

    private Task CheckIfEmulatorRunning(CancellationToken token)
    {
        return Task.Run(async () =>
        {
            try
            {
                while (token.IsCancellationRequested == false)
                {
                    bool processExists = Process.GetProcesses()
                        .Any(p => p.ProcessName.ToUpper().Contains(Config.Instance.Emulator.ToUpper()));
                    if (processExists) this.Send(new MessagePlaying());
                    await Task.Delay(1000, token);
                }
            }
            catch (OperationCanceledException e)
            {
                Console.WriteLine("CheckIfEmulatorRunning aborted");
            }
        }, token);
    }

    public Task Start(CancellationToken token)
    {
        Task running = this.CheckIfEmulatorRunning(token);
        Task listen = this.Listen(token);

        return Task.WhenAll(running, listen);
    }

    private Task Listen(CancellationToken token)
    {
        return Task.Run(() =>
        {
            Stream stream = this._socket.GetStream();
            byte[] length = new byte[4];
            int i = 0;
            int r;
            while (token.IsCancellationRequested == false && (r = stream.ReadByte()) != -1)
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
        }, token);
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
            // we are out of date
            this._context.State = "<Syncing...>";
            this.Send(new MessagePull());
        } else if (message is MessagePull)
        {
            byte[] content = await Village.Instance.GetContent();
            this.Send(new MessagePush(content, Village.Instance.ModifiedAt));
        } else if (message is MessagePush push)
        {
            // received a version
            this._context.State = "<Connected...>";
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