using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;
using System.Net.NetworkInformation;
namespace AnimalCrossing.Client;

public class Pair : IOther
{
    private static int GetAvailablePort(int startingPort)
    {
        IPEndPoint[] endPoints;
        List<int> portArray = new List<int>();

        IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();

        //getting active connections
        TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
        portArray.AddRange(from n in connections
            where n.LocalEndPoint.Port >= startingPort
            select n.LocalEndPoint.Port);

        //getting active tcp listners - WCF service listening in tcp
        endPoints = properties.GetActiveTcpListeners();
        portArray.AddRange(from n in endPoints
            where n.Port >= startingPort
            select n.Port);

        //getting active udp listeners
        endPoints = properties.GetActiveUdpListeners();
        portArray.AddRange(from n in endPoints
            where n.Port >= startingPort
            select n.Port);

        portArray.Sort();

        for (int i = startingPort; i < UInt16.MaxValue; i++)
            if (!portArray.Contains(i))
                return i;

        return 0;
    }
    
    public UdpClient Client { get; }
    public IPEndPoint Self { get; set; }

    public Pair()
    {
        this.Self = new IPEndPoint(IPAddress.Any, 5242);
        this.Client = new UdpClient();
    }

    public void Receive()
    {
        IPEndPoint? ep = null;
        byte[] data = this.Client.Receive(ref ep);
        
        
        IMessage message = IMessage.Parse(data);
        Console.WriteLine("Incoming " + message.Type + " from " + ep?.Address + ":" + ep?.Port);
        message.Act(this, ep);
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