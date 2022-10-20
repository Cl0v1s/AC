namespace AnimalCrossing.Shared;

public class MessagePull : Message
{
    public MessagePull()
    {
        this.Type = MessageTypes.Pull;
    }
}