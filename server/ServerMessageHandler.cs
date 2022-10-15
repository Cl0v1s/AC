using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class ServerMessageHandler : IMessageHandler
{
    private List<IPEndPoint> _clients;
    public UdpClient Client { get; }
    public IPEndPoint Self { get; set; }

    public ServerMessageHandler(IPEndPoint self, UdpClient client)
    {
        this.Client = client;
        this._clients = new List<IPEndPoint>();
        this.Self = self;
    }

    public void Receive(IPEndPoint sender, byte[] data)
    {
        IMessage message = IMessage.Parse(data);
        Console.WriteLine("Incoming " + message.Type + " from " + sender.Address + ":" + sender.Port);
        IMessage? response = message.Act(this, sender);
        if(response != null) this.Send(response.ReplyTo ?? sender, response);
    }
    
    public void Send(IPEndPoint endpoint, IMessage message)
    {
        Console.WriteLine("Sending " + message.Type + " to " + endpoint.Address + ":" + endpoint.Port);
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        byte[] data = stream.ToArray();
        this.Client.Send(data, data.Length, endpoint);
        bw.Close();
        stream.Close();
    }

}