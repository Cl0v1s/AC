using System.Net;

namespace AnimalCrossing.Shared;

public class MessageDiscover : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint? To { get; set; }

    public MessageDiscover() {}

    public MessageDiscover(IPEndPoint from, IPEndPoint to, IPEndPoint replyTo)
    {
        // server side
        this.Type = MessageTypes.Discover;
        this.ReplyTo = replyTo;
        this.From = from;
        this.To = to;
    }

    /*
    public IMessage[] Act(IMessageHandler client, IPEndPoint sender)
    {
        // client side
        Pair thisSide = (Pair)client.Self;
        Pair pair = new Pair(client, this.ReplyTo.Address, this.ReplyTo.Port);
        client.Pairs.Add(pair);
        return new IMessage[] { new MessageSyncCompare(thisSide, pair, thisSide.File!.Hash, thisSide.File.ModifiedAt)};
    }
    */

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
    }

    public void Deserialize(BinaryReader bw)
    {
        IMessage.Deserialize(bw, this);
    }
}