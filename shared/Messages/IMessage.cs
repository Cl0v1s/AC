using System.Net;
using System.Net.Sockets;
using System.Runtime;

namespace AnimalCrossing.Shared;

public enum MessageTypes {
    Identity,
    Discover,
    SyncCompare,
    SyncRequest,
    SyncResponse,
    Bye,
}

public interface IMessage {
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint To { get; set; }

    public void Serialize(BinaryWriter bw);
    
    public void Deserialize(BinaryReader bw);

    private static void SerializeIpEndpoint(BinaryWriter bw, IPEndPoint? endpoint)
    {
        byte[] address = endpoint?.Address.GetAddressBytes() ?? new byte[]{};
        bw.Write(address.Length);
        bw.Write(address);
        bw.Write(endpoint?.Port ?? 0);
    }

    private static IPEndPoint? DeserializeIpEndpoint(BinaryReader br)
    {
        int len = br.ReadInt32();
        byte[] address = br.ReadBytes(len);
        int port = br.ReadInt32();
        if (len == 0) return null;
        return new IPEndPoint(new IPAddress(address), port);
    }

    public static void Serialize(BinaryWriter bw, IMessage message)
    {
        bw.Write((int)message.Type);
        IMessage.SerializeIpEndpoint(bw, message.From);
        IMessage.SerializeIpEndpoint(bw, message.To);
        IMessage.SerializeIpEndpoint(bw, message.ReplyTo);

    }

    public static void Deserialize(BinaryReader br, IMessage message)
    {
        // this.type is already parse
        message.From = IMessage.DeserializeIpEndpoint(br);
        message.To = IMessage.DeserializeIpEndpoint(br)!;
        message.ReplyTo = IMessage.DeserializeIpEndpoint(br);
    }

    public static IMessage? Parse(byte[] data) {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        MessageTypes type = (MessageTypes)br.ReadInt32();

        Type cls;
        switch(type) {
            case MessageTypes.Identity:
                cls = typeof(MessageIdentity);
                break;
            case MessageTypes.Discover:
                cls = typeof(MessageDiscover);
                break;
            case MessageTypes.SyncCompare:
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
                return null;
        }

        IMessage message = (IMessage)Activator.CreateInstance(cls)!;
        message.Type = type;
        message.Deserialize(br);
        
        return message;
    }
}
