using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Client;

public class Pair : IOther
{
    public UdpClient Client { get; }
    public Village? Village { get; set; }

    public Pair(Village village, IPAddress address, int port)
    {
        this.Village = village;
        
        this.Client = new UdpClient();
        this.Client.Connect(address, port);

        this.Client.BeginReceive(this.Receive, new object());
    }

    private void Receive(IAsyncResult ar)
    {
        IPEndPoint? ep = null;
        byte[] data = this.Client.EndReceive(ar, ref ep);
        
        IMessage? message = IMessage.Parse(data);
        message?.Act(this);

        this.Client.BeginReceive(this.Receive, new object());
    }

    public void Send(IMessage message)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        byte[] data = stream.ToArray();
        this.Client.Send(data, data.Length);
        bw.Close();
        stream.Close();
    }
}