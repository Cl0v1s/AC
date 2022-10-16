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
    
    public IMessage[] Act(IMessageHandler handler, IPEndPoint raw)
    {
        Pair sender = (Pair)raw;
        // client side dest
        Console.WriteLine("Other Hash: " + this.Hash + " AT " + this.ModifiedAt);
        sender.File = new File(this.Hash, this.ModifiedAt);
        Pair? thisSide = handler.Self as Pair;

        // The two files are the same we stop there
        if (this.Hash == thisSide!.File!.Hash) return new IMessage[] { };
        
        // else we have an older version than peer
        if (this.ModifiedAt >= thisSide.File.ModifiedAt)
            return sender.StartSync(handler);
        
        // else the other is outdated. None of our business for now
        
        return new IMessage[] { };
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