using System.Net;

namespace AnimalCrossing.Shared;

public class Pair : IPEndPoint
{
    private bool Synchronized { get; } = false;
    
    public Pair(long address, int port) : base(address, port)
    {
    }

    public Pair(IPAddress address, int port) : base(address, port)
    {
    }

    public void StartSync()
    {
        
    }
}