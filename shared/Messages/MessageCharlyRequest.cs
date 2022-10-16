using System.Net;

namespace AnimalCrossing.Shared;

public class MessageCharlyRequest : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint? To { get; set; }

    private string Password { get; set; }
    
    public MessageCharlyRequest() {}

    public MessageCharlyRequest(IPEndPoint to, string password)
    {
        // client side
        this.Type = MessageTypes.CharlyRequest;
        this.ReplyTo = null;
        this.From = null;
        this.To = to;
        this.Password = password;
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        bw.Write(this.Password);
    }

    public void Deserialize(BinaryReader bw)
    {
        IMessage.Deserialize(bw, this);
        this.Password = bw.ReadString();
    }
}