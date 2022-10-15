using System.Net;
using System.Net.Sockets;
using System.Runtime;

namespace AnimalCrossing.Shared;

public enum MessageTypes {
    CharlyRequest = 1,
    CharlyResponse = 2,
    Tango = 3,
}

public interface IMessage {
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }

    public IMessage? Act(IMessageHandler client, IPEndPoint sender);

    public void Serialize(BinaryWriter bw);
    
    public void Deserialize(BinaryReader bw);

    public static void Serialize(BinaryWriter bw, IMessage message)
    {
        bw.Write((int)message.Type);
        byte[] address = message.ReplyTo?.Address.GetAddressBytes() ?? new byte[]{};
        bw.Write(address.Length);
        bw.Write(address);
        bw.Write(message.ReplyTo?.Port ?? 0);

    }

    public static void Deserialize(BinaryReader br, IMessage message)
    {
        // this.type is already parse
        int len = br.ReadInt32();
        IPAddress address = new IPAddress(br.ReadBytes(len));
        message.ReplyTo = new IPEndPoint(address, br.ReadInt32());
    }

    public static IMessage Parse(byte[] data) {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        MessageTypes type = (MessageTypes)br.ReadInt32();
        Console.WriteLine(type);

        Type cls;
        switch(type) {
            case MessageTypes.CharlyRequest:
                cls = typeof(MessageCharlyRequest);
                break;
            case MessageTypes.CharlyResponse:
                cls = typeof(MessageCharlyResponse);
                break;
            case MessageTypes.Tango:
                cls = typeof(MessageTango);
                break;
            default:
                throw new FormatException("Message type does not exists");
        }

        IMessage message = (IMessage)Activator.CreateInstance(cls)!;
        message.Type = type;
        message.Deserialize(br);
        
        return message;
    }
}
