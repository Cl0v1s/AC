using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;
using System.Net.NetworkInformation;
namespace AnimalCrossing.Client;

public class ClientMessageHandler : IMessageHandler
{
    public UdpClient Client { get; }
    public IPEndPoint Self { get; set; }

    public ClientMessageHandler()
    {
        this.Self = new IPEndPoint(IPAddress.Any, 0);
        this.Client = new UdpClient();
    }

    public void Receive()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = this.Client.Receive(ref sender);
        
        IMessage message = IMessage.Parse(data);
        Console.WriteLine("Incoming " + message.Type + " from " + sender.Address + ":" + sender.Port);
        IMessage? response = message.Act(this, sender);
        if(response != null) this.Send(message.ReplyTo ?? sender, response);
    }
    
    public void Send(IPEndPoint endpoint, IMessage message)
    {
        if (this.Self.Address.ToString() != IPAddress.Any.ToString())
        {
            message.ReplyTo = this.Self;
        }
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        byte[] data = stream.ToArray();
        Console.WriteLine("Sending " + message.Type + " to " + endpoint.Address + ":" + endpoint.Port);
        this.Client.Send(data, data.Length, endpoint);
        bw.Close();
        stream.Close();
    }


}