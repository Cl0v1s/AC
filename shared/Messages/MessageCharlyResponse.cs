using System.Net;

namespace AnimalCrossing.Shared;

public class MessageCharlyResponse : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint ReplyTo { get; set; }

    public MessageCharlyResponse()
    {
        this.Type = MessageTypes.CharlyResponse;
    }

    public MessageCharlyResponse(IPEndPoint replyTo)
    {
        this.Type = MessageTypes.CharlyResponse;
        this.ReplyTo = replyTo;
    }
    
    public void Act(IOther client, IPEndPoint sender)
    {
        // client side
        client.Self = this.ReplyTo;
        Console.WriteLine("Now I known I'm " + this.ReplyTo.Address + ":" + this.ReplyTo.Port);
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