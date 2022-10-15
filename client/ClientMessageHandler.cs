using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;
using System.Net.NetworkInformation;
namespace AnimalCrossing.Client;

public class ClientMessageHandler : IMessageHandler
{
    public UdpClient Client { get; }
    public IPEndPoint? Self { get; set; }
    public List<IPEndPoint> Pairs { get; set; }

    public ClientMessageHandler()
    {
        this.Pairs = new List<IPEndPoint>();
        this.Self = null;
        this.Client = new UdpClient();
    }

    public void Receive()
    {
        IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = this.Client.Receive(ref sender);
        
        IMessage message = IMessage.Parse(data);
        Console.WriteLine("Incoming " + message.Type + " from " + sender.Address + ":" + sender.Port);
        IMessage[] responses = message.Act(this, sender);
        foreach (IMessage response in responses)
        {
            this.Send(response);
        }
    }
    
    public void Send(IMessage message)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        byte[] data = stream.ToArray();
        Console.WriteLine("Sending " + message.Type + " to " + message.To.Address + ":" + message.To.Port);
        this.Client.Send(data, data.Length, message.To);
        bw.Close();
        stream.Close();
    }


}