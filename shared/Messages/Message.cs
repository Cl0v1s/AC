using System.Net;
using System.Net.Sockets;
using System.Runtime;

namespace AnimalCrossing.Shared;

public enum MessageTypes {
    State,
    Pull,
    Push,
}

public class Message {
    public MessageTypes Type { get; set; }
    
    public Message() {}

    public Message(MessageTypes type)
    {
        this.Type = type;
    }

    public virtual void Serialize(BinaryWriter bw)
    {
        bw.Write((int)this.Type);
    }

    protected virtual void Deserialize(BinaryReader br)
    {
        // this.type is already parse
    }

    public static Message Parse(byte[] data) {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        MessageTypes type = (MessageTypes)br.ReadInt32();

        Type cls;
        switch(type) {
            case MessageTypes.Pull:
                cls = typeof(MessagePull);
                break;
            case MessageTypes.Push:
                cls = typeof(MessagePush);
                break;
            case MessageTypes.State:
                cls = typeof(MessageState);
                break;
            default:
                cls = typeof(Message);
                break;
        }

        Message message = (Message)Activator.CreateInstance(cls)!;
        message.Type = type;
        message.Deserialize(br);
        
        return message;
    }
}
