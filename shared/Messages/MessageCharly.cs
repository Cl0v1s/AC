using System.Net;

namespace AnimalCrossing.Shared;

public class MessageCharly : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }
    
    public string Password { get; set; }
    
    public MessageCharly() {}

    public MessageCharly(IPEndPoint from, IPEndPoint to, string password)
    {
        this.Type = MessageTypes.Charly;
        this.From = from;
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