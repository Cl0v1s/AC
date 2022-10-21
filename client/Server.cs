using System.Net;
using System.Net.Sockets;
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
            while (true)
            {
                if(this._socket.Connected == false) continue;
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
        });
    }

    private void Send(Message message)
    {
        Stream stream = this._socket.GetStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        bw.Close();
    }

    private void Handle(Message message)
    {
        Console.WriteLine("Handled " + message.Type);
    }
}