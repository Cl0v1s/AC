using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class ClientUser : IOther
{
    private List<IPEndPoint> _clients;
    public UdpClient Client { get; }
    public IPEndPoint Self { get; set; }

    public ClientUser(IPEndPoint self, UdpClient client)
    {
        this.Client = client;
        this._clients = new List<IPEndPoint>();
        this.Self = self;
    }

    public void Receive(IPEndPoint sender, byte[] data)
    {
        IMessage message = IMessage.Parse(data);
        Console.WriteLine("Incoming " + message.Type + " from " + sender.Address + ":" + sender.Port);
        message.Act(this, sender);
        Thread.Sleep(5000);
        if (message is MessageCharlyRequest)
        {
            this._clients.ForEach((c) =>
            {
                this.Send( c, new MessageTango(sender));
                Thread.Sleep(1000);
                this.Send(sender, new MessageTango(c));
            });                        
            this._clients.Add(sender);
        }
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