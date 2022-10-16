using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncInit : IMessage
{
    private static readonly DateTime EPOCH = new DateTime (1970, 01, 01);

    
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint To { get; set; }
    
    public string Hash { get; set; }
    
    public DateTime ModifiedAt { get; set; }
    
    public MessageSyncInit() {}

    public MessageSyncInit(IPEndPoint from, IPEndPoint to, string hash, DateTime modifiedAt)
    {
        this.Type = MessageTypes.SyncInit;
        // client side init
        this.From = from;
        this.To = to;
        this.Hash = hash;
        this.ModifiedAt = modifiedAt;
    }
    
    public IMessage[] Act(IMessageHandler client, IPEndPoint sender)
    {
        // client side dest
        Console.WriteLine("Other Hash: " + this.Hash + " AT " + this.ModifiedAt);
        return new IMessage[] { };
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        bw.Write(this.Hash);
        bw.Write((this.ModifiedAt - MessageSyncInit.EPOCH).TotalSeconds);
    }

    public void Deserialize(BinaryReader br)
    {
        IMessage.Deserialize(br, this);
        this.Hash = br.ReadString();
        this.ModifiedAt = EPOCH.AddSeconds(br.ReadDouble());
    }
}