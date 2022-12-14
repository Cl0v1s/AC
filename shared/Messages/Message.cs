using System.Net;
using System.Net.Sockets;
using System.Runtime;
using System.Security.Cryptography;
using System.Text;

namespace AnimalCrossing.Shared;

public enum MessageTypes {
    State,
    Pull,
    Push,
    Playing,
}

public class Message {
    public MessageTypes Type { get; set; }
    
    protected Message() {}

    public Message(MessageTypes type)
    {
        this.Type = type;
    }

    public virtual void Serialize(BinaryWriter bw)
    {
        bw.Write((byte)this.Type);
    }

    protected virtual void Deserialize(BinaryReader br)
    {
        // this.type is already parse
    }

    public static Message? Parse(byte[] data) {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        MessageTypes type = (MessageTypes)br.ReadByte();

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
                return null;
        }

        Message message = (Message)Activator.CreateInstance(cls)!;
        message.Type = type;
        message.Deserialize(br);
        
        return message;
    }

    public static string ComputeHash(byte[] content)
    {
        SHA256 s = SHA256.Create();
        content = s.ComputeHash(content);
        return Encoding.ASCII.GetString(content);
    }
}
