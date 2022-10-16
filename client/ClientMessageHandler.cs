using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;
using System.Net.NetworkInformation;
namespace AnimalCrossing.Client;

public class ClientMessageHandler : IMessageHandler
{
    public UdpClient Client { get; }
    public IPEndPoint Self { get; set; }
    
    private Pair Server { get; }
    
    public List<IPEndPoint> Pairs { get; set; }

    public ClientMessageHandler(IPEndPoint server)
    {
        this.Pairs = new List<IPEndPoint>();
        this.Self = new ClientPair(this, new AnimalCrossing.Shared.File(Config.Instance.SaveFile), IPAddress.Any, 0);
        this.Client = new UdpClient();
        this.Client.DontFragment = true;
        this.Server = new ServerPair(this, server.Address, server.Port);
    }

    public Pair FindPair(IPEndPoint sender)
    {
        if (sender.Address.ToString() == this.Server.Address.ToString() && sender.Port == this.Server.Port)
        {
            return this.Server;
        }
        ClientPair? pair =
            this.Pairs.Find(x => x.Address.ToString() == sender.Address.ToString() && x.Port == sender.Port) as ClientPair;
        if (pair == null)
        {
            pair = new ClientPair(this, sender.Address, sender.Port);
            this.Pairs.Add(pair);
        }

        return pair;
    }
    
    public void Receive()
    {
        IPEndPoint raw = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = this.Client.Receive(ref raw);

        Pair sender = this.FindPair(raw);
        
        IMessage? message = IMessage.Parse(data);
        if (message == null)
        {
            Console.WriteLine("Unknown message from "+ sender.Address + ":" + sender.Port);
            return;
        }
        Console.WriteLine("Incoming " + message.Type + " from " + sender.Address + ":" + sender.Port);

        IMessage[] responses = sender.Handle(this, message);
        foreach (IMessage response in responses)
        {
            this.Send(response);
        }
        /*
        IMessage[] responses = message.Act(this, sender);
        foreach (IMessage response in responses)
        {
            this.Send(response);
        }
        */
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