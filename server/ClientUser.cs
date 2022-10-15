using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class ClientUser : IOther
{
    private readonly IPEndPoint _endPoint;
    public UdpClient Client { get; }
    public Village? Village { get; set; }

    public ClientUser(UdpClient client, IPAddress address, int port)
    {
        this.Client = client;
        this._endPoint = new IPEndPoint(address, port);
    }

    public void Receive(byte[] data)
    {
        IMessage message = IMessage.Parse(data);
        message?.Act(this);
    }
    
    public void Send(IMessage message)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        byte[] data = stream.ToArray();
        this.Client.Send(data, data.Length, this._endPoint);
        bw.Close();
        stream.Close();
    }
}