using System.Net;

namespace AnimalCrossing.Shared;

public class MessageDiscover : Message
{
    public IPEndPoint ReplyTo { get; set; }
    
    public MessageDiscover() {}

    public MessageDiscover(IPEndPoint from, IPEndPoint to, IPEndPoint replyTo): base(MessageTypes.Discover, from, to)
    {
        this.ReplyTo = replyTo;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        Message.SerializeIpEndpoint(bw, this.ReplyTo);
    }

    public override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.ReplyTo = Message.DeserializeIpEndpoint(br);
    }
}