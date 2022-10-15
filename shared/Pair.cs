using System.Net;

namespace AnimalCrossing.Shared;

public class Pair : IPEndPoint
{
    private bool Remote { get;  }
    
    private bool Synchronized { get; } = false;
    
    public string Hash { get; private set;  }
    
    public DateTime ModifiedAt { get; private set; }
    
    public Pair(bool remote, long address, int port) : base(address, port)
    {
        this.Remote = remote;
        if(!remote) this.LoadSave();
    }

    public Pair(bool remote, IPAddress address, int port) : base(address, port)
    {
        this.Remote = remote;
        if(!remote) this.LoadSave();
    }

    private void LoadSave()
    {
        
    }

    public void StartSync(IMessageHandler handler)
    {
        
    }

    public void HandleSyncInit(IMessageHandler handler, MessageSyncInit message)
    {
        this.Hash = message.Hash;
        this.ModifiedAt = message.ModifiedAt;
    }
}