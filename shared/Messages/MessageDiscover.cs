using System.Net;

namespace AnimalCrossing.Shared;

public class MessageDiscover : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint ReplyTo { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }

    public MessageDiscover() {}

    public MessageDiscover(IPEndPoint from, IPEndPoint to, IPEndPoint replyTo)
    {
        // server side
        this.Type = MessageTypes.Discover;
        this.ReplyTo = replyTo;
        this.From = from;
        this.To = to;
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        IMessage.SerializeIpEndpoint(bw, this.ReplyTo);
    }

    public void Deserialize(BinaryReader bw)
    {
        IMessage.Deserialize(bw, this);
        this.ReplyTo = IMessage.DeserializeIpEndpoint(bw);
    }
}