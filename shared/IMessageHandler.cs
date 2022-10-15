using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Shared;

public interface IMessageHandler
{
    UdpClient Client { get;  }
    
    IPEndPoint Self { get; set; }
    
    void Send(IPEndPoint endpoint, IMessage message);
}