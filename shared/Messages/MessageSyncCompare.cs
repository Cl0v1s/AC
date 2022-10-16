using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncCompare : IMessage
{
    private static readonly DateTime Epoch = new DateTime (1970, 01, 01);
    
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint To { get; set; }
    
    public string Hash { get; set; }
    
    public DateTime ModifiedAt { get; set; }
    
    public MessageSyncCompare() {}

    public MessageSyncCompare(IPEndPoint from, IPEndPoint to, string hash, DateTime modifiedAt)
    {
        this.Type = MessageTypes.SyncCompare;
        // client side init
        this.From = from;
        this.To = to;
        this.Hash = hash;
        this.ModifiedAt = modifiedAt;
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        bw.Write(this.Hash);
        bw.Write((this.ModifiedAt - MessageSyncCompare.Epoch).TotalSeconds);
    }

    public void Deserialize(BinaryReader br)
    {
        IMessage.Deserialize(br, this);
        this.Hash = br.ReadString();
        this.ModifiedAt = Epoch.AddSeconds(br.ReadDouble());
    }
}