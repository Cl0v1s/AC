using System.Net.Sockets;

namespace AnimalCrossing.Shared;

public interface IOther
{
    UdpClient Client { get;  }
    
    Village? Village { get; set; }

    void Send(IMessage message);
}