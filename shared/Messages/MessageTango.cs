using System.Net;

namespace AnimalCrossing.Shared;

public class MessageTango : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }

    public MessageTango() {}

    public MessageTango(IPEndPoint replyTo)
    {
        this.Type = MessageTypes.Tango;
        this.ReplyTo = replyTo;
    }

    public IMessage? Act(IMessageHandler client, IPEndPoint sender)
    {
        // client side
        return null;
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
    }

    public void Deserialize(BinaryReader bw)
    {
        IMessage.Deserialize(bw, this);
    }
}