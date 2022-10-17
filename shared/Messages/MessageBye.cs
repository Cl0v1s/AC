using System.Net;

namespace AnimalCrossing.Shared;

public class MessageBye :  Message
{
    
    public MessageBye() {}
    
    public MessageBye(IPEndPoint from, IPEndPoint to) : base(MessageTypes.Bye, from, to)
    {
    }
}