using System.Net;

namespace AnimalCrossing.Shared;

public class MessageBye :  IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }
    
    public MessageBye() {}

    public MessageBye(IPEndPoint from, IPEndPoint to)
    {
        this.Type = MessageTypes.Bye;
        this.To = to;
        this.From = from;
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