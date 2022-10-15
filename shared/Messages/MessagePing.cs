using System.Net;

namespace AnimalCrossing.Shared;

public class MessagePing : IMessage
{

    public MessageTypes Type { get; set; }
    public IPEndPoint ReplyTo { get; set; }
    
    public MessagePing()
    {
        this.Type = MessageTypes.Ping;
        this.ReplyTo = new IPEndPoint(IPAddress.Any, 0);
    }

    public void Act(IOther client, IPEndPoint sender)
    {
        client.Send(sender, new MessagePing());
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