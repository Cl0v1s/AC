using System.Net;

namespace AnimalCrossing.Shared;

public class MessageCharlyResponse : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }

    public MessageCharlyResponse() {}
    
    public MessageCharlyResponse(IPEndPoint replyTo)
    {
        this.Type = MessageTypes.CharlyResponse;
        this.ReplyTo = replyTo;
    }
    
    public IMessage? Act(IMessageHandler client, IPEndPoint sender)
    {
        // client side
        Console.WriteLine("Now I known I'm " + this.ReplyTo?.Address + ":" + this.ReplyTo?.Port);
        if(this.ReplyTo != null) client.Self = this.ReplyTo;
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