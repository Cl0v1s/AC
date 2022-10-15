using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncInit : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint To { get; set; }
    
    public string Hash { get; }
    
    public DateTime ModifiedAt { get; }
    
    public MessageSyncInit() {}

    public MessageSyncInit(IPEndPoint from, IPEndPoint to, string hash, DateTime modifiedAt)
    {
        // client side
        this.From = from;
        this.To = to;
        this.Hash = hash;
        this.ModifiedAt = modifiedAt;
    }
    
    public IMessage[] Act(IMessageHandler client, IPEndPoint sender)
    {
        // client side
        Pair? pair =
            client.Pairs.Find(x => x.Address.ToString() == sender.Address.ToString() && x.Port == sender.Port) as Pair;
        if (pair == null) throw new KeyNotFoundException("Pair must exists at this point.");
        pair.HandleSyncInit(client, this);

        return new IMessage[] { };
    }

    public void Serialize(BinaryWriter bw)
    {
        throw new NotImplementedException();
    }

    public void Deserialize(BinaryReader bw)
    {
        throw new NotImplementedException();
    }
}