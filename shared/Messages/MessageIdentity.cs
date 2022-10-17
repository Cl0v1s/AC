using System.Net;

namespace AnimalCrossing.Shared;

public class MessageIdentity : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }

    public MessageIdentity() {}
    
    public MessageIdentity(IPEndPoint from, IPEndPoint to)
    {
        // server side
        this.Type = MessageTypes.Identity;
        this.From = from;
        this.To = to;

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