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

    public IMessage[] Act(IMessageHandler client, IPEndPoint sender)
    {
        // serverside
        //TODO: do something with password
        List<IMessage> responses = new List<IMessage>() { new MessageCharlyResponse(client.Self!, sender) };

        client.Pairs.ForEach((pair) =>
        {
            if (pair.Address.ToString() != sender.Address.ToString() || pair.Port != sender.Port)
            {
                responses.Add(new MessageDiscover(client.Self!, pair, sender));
                responses.Add(new MessageDiscover(client.Self!, sender, pair));
            }
        });

        return responses.ToArray();
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