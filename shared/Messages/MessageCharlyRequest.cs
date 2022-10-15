using System.Net;

namespace AnimalCrossing.Shared;

public class MessageCharlyRequest : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }

    private string Password { get; set; }
    
    public MessageCharlyRequest() {}

    public MessageCharlyRequest(string password)
    {
        this.Type = MessageTypes.CharlyRequest;
        this.ReplyTo = new IPEndPoint(IPAddress.Any, 0);
        this.Password = password;
    }

    public IMessage Act(IMessageHandler client, IPEndPoint sender)
    {
        // serverside
        //TODO: do something with password
        return new MessageCharlyResponse(sender);
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