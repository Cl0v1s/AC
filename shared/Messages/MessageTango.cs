using System.Net;

namespace AnimalCrossing.Shared;

public class MessageTango : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint ReplyTo { get; set; }

    public MessageTango()
    {
        this.Type = MessageTypes.Tango;
    }

    public MessageTango(IPEndPoint replyTo)
    {
        this.Type = MessageTypes.Tango;
        this.ReplyTo = replyTo;
    }

    public void Act(IOther client, IPEndPoint sender)
    {
        // client side
        MessagePing message = new MessagePing();
        client.Send(this.ReplyTo, message);
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