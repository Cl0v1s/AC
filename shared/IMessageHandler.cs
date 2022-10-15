using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Shared;

public interface IMessageHandler
{
    UdpClient Client { get;  }
    
    IPEndPoint? Self { get; set; }
    
    List<IPEndPoint> Pairs { get; set; }
    
    void Send(IMessage message);
}