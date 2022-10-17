using System.Net;

namespace AnimalCrossing.Shared;

public class MessageIdentity : Message
{
    
    public MessageIdentity() {}
    
    public MessageIdentity(IPEndPoint from, IPEndPoint to): base(MessageTypes.Identity, from, to)
    {
    }
}