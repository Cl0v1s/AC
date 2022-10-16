using System.Net;

namespace AnimalCrossing.Shared;

public class MessageCharlyResponse : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint To { get; set; }

    public MessageCharlyResponse() {}
    
    public MessageCharlyResponse(IPEndPoint from, IPEndPoint to)
    {
        // server side
        this.Type = MessageTypes.CharlyResponse;
        this.ReplyTo = null;
        this.From = from;
        this.To = to;

    }
    
    public IMessage[] Act(IMessageHandler client, IPEndPoint sender)
    {
        // client side
        Console.WriteLine("Now I known I'm " + this.To.Address + ":" + this.To.Port);
        client.Self.Address = this.To.Address;
        client.Self.Port = this.To.Port;
        return new IMessage[] {};
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