using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class ServerMessageHandler : IMessageHandler
{
    public UdpClient Client { get; }
    public IPEndPoint Self { get; set; }
    
    public List<IPEndPoint> Pairs { get; set; }

    public ServerMessageHandler(IPEndPoint self, UdpClient client)
    {
        this.Pairs = new List<IPEndPoint>();
        this.Client = client;
        this.Self = self;
        
        AppDomain.CurrentDomain.ProcessExit += new EventHandler (this.OnProcessExit);
        Console.CancelKeyPress += this.OnProcessExit;
    }
    
    /// <summary>
    /// Send a bye message to server before quitting
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    private void OnProcessExit(object? obj, EventArgs args)
    {
        Console.WriteLine("Quitting....");
        this.Pairs.ForEach((pair) =>
        {
            this.Send(new MessageBye(this.Self, pair), false);
        });
    }
    
    private ClientPair FindPair(IPEndPoint sender)
    {
        ClientPair? pair =
            this.Pairs.Find(x => x.Address.ToString() == sender.Address.ToString() && x.Port == sender.Port) as ClientPair;
        if (pair == null)
        {
            pair = new ClientPair(this, sender.Address, sender.Port);
            this.Pairs.Add(pair);
        }
        return pair;
    }

    public void Receive(IPEndPoint sender, byte[] data)
    {
        ClientPair client = this.FindPair(sender);        
        Message? message = Message.Parse(data);
        if (message == null)
        {
            Console.WriteLine("Unknown message from "+ sender.Address + ":" + sender.Port);
            return;
        }
        Console.WriteLine("Incoming " + message.Type + " from " + sender.Address + ":" + sender.Port);
        client.Handle(this, message);
    }
    
    // we dont use needResponse here it's never used
    public void Send(Message message, bool needResponse)
    {
        Console.WriteLine("Sending " + message.Type + " to " + message.To.Address + ":" + message.To.Port);
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        byte[] data = stream.ToArray();
        this.Client.Send(data, data.Length, message.To);
        bw.Close();
        stream.Close();
    }

}