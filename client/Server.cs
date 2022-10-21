using System.Net;
using System.Net.Sockets;
using System.Text;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class Server
{
    private TcpClient _socket;
    private CancellationTokenSource _cancellationTokenSource;
    
    public Server()
    {
        this._socket = new TcpClient(Config.Instance.ServerAddress, Config.Instance.ServerPort);
        this._cancellationTokenSource = new CancellationTokenSource();
        
        this.Send(new MessageState(Config.Instance.Password, DateTime.Now, "TEST"));
    }

    public Task Start()
    {
        return Task.Run(() =>
        {
            Stream stream = this._socket.GetStream();
            byte[] length = new byte[4];
            int i = 0;
            int r;
            while ((r = stream.ReadByte()) != -1)
            {
                length[i] = (byte)r;
                i++;
                if (i < length.Length) continue;
                byte[] data = new byte[BitConverter.ToInt32(length)];
                Console.Write("Receiving " + data.Length + " bytes...");
                i = 0;
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
        Console.Write("Sending " + data.Length + " bytes...");
        stream.Write(data);
        Console.WriteLine("Done");
    }

    private void Handle(Message message)
    {
        Console.WriteLine("Handled " + message.Type);
    }
}