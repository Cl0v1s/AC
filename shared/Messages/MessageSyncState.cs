using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncState : IMessage
{
    private static readonly DateTime Epoch = new DateTime (1970, 01, 01);
    
    public MessageTypes Type { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }
    
    public string Hash { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public bool Playing { get; set; }
    
    
    public MessageSyncState() {}

    public MessageSyncState(IPEndPoint from, IPEndPoint to, string hash, DateTime modifiedAt, bool playing)
    {
        this.Type = MessageTypes.SyncCompare;
        // client side init
        this.From = from;
        this.To = to;
        this.Hash = hash;
        this.ModifiedAt = modifiedAt;
        this.Playing = playing;
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        bw.Write(this.Hash);
        bw.Write((this.ModifiedAt - MessageSyncState.Epoch).TotalSeconds);
        bw.Write(this.Playing);
    }

    public void Deserialize(BinaryReader br)
    {
        IMessage.Deserialize(br, this);
        this.Hash = br.ReadString();
        this.ModifiedAt = Epoch.AddSeconds(br.ReadDouble());
        this.Playing = br.ReadBoolean();
    }
}