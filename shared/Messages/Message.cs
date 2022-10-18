using System.Net;
using System.Net.Sockets;
using System.Runtime;

namespace AnimalCrossing.Shared;

public enum MessageTypes {
    Charly,
    Identity,
    Discover,
    SyncState,
    SyncRequest,
    SyncResponse,
    Bye,
    Ok,
}

public class Message {
    public MessageTypes Type { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }
    
    public Message() {}

    public Message(MessageTypes type, IPEndPoint from, IPEndPoint to)
    {
        this.Type = type;
        this.From = from;
        this.To = to;
    }

    public virtual void Serialize(BinaryWriter bw)
    {
        bw.Write((int)this.Type);
        Message.SerializeIpEndpoint(bw, this.From);
        Message.SerializeIpEndpoint(bw, this.To);  
    }

    protected virtual void Deserialize(BinaryReader br)
    {
        // this.type is already parse
        this.From = Message.DeserializeIpEndpoint(br)!;
        this.To = Message.DeserializeIpEndpoint(br)!;
    }

    protected static void SerializeIpEndpoint(BinaryWriter bw, IPEndPoint? endpoint)
    {
        byte[] address = endpoint?.Address.GetAddressBytes() ?? new byte[]{};
        bw.Write(address.Length);
        bw.Write(address);
        bw.Write(endpoint?.Port ?? 0);
    }

    protected static IPEndPoint DeserializeIpEndpoint(BinaryReader br)
    {
        int len = br.ReadInt32();
        byte[] address = br.ReadBytes(len);
        int port = br.ReadInt32();
        if (len == 0) return null;
        return new IPEndPoint(new IPAddress(address), port);
    }

    public static Message? Parse(byte[] data) {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        MessageTypes type = (MessageTypes)br.ReadInt32();

        Type cls;
        switch(type) {
            case MessageTypes.Charly:
                cls = typeof(MessageCharly);
                break;
            case MessageTypes.Identity:
                cls = typeof(MessageIdentity);
                break;
            case MessageTypes.Discover:
                cls = typeof(MessageDiscover);
                break;
            case MessageTypes.SyncState:
                cls = typeof(MessageSyncState);
                break;
            case MessageTypes.SyncRequest:
                cls = typeof(MessageSyncRequest);
                break;
            case MessageTypes.SyncResponse:
                cls = typeof(MessageSyncResponse);
                break;
            case MessageTypes.Bye:
                cls = typeof(MessageBye);
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
