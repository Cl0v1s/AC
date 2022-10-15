using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Shared;

public enum MessageTypes {
    Ping = 0,
    CharlyRequest = 1,
    CharlyResponse = 2,
    Tango = 3,
}

public interface IMessage {
    public MessageTypes Type { get; set; }
    public IPEndPoint ReplyTo { get; set; }

    public void Act(IOther client, IPEndPoint sender);

    public void Serialize(BinaryWriter bw);
    
    public void Deserialize(BinaryReader bw);

    public static void Serialize(BinaryWriter bw, IMessage message)
    {
        bw.Write((int)message.Type);
        byte[] address = message.ReplyTo.Address.GetAddressBytes();
        bw.Write(address.Length);
        bw.Write(address);
        bw.Write(message.ReplyTo.Port);

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

        IMessage message;

        switch(type) {
            case MessageTypes.Ping:
                message = new MessagePing();
                break;
            case MessageTypes.CharlyRequest:
                message = new MessageCharlyRequest();
                break;
            case MessageTypes.CharlyResponse:
                message = new MessageCharlyResponse();
                break;
            case MessageTypes.Tango:
                message = new MessageTango();
                break;
            default:
                throw new FormatException("Message type does not exists");
        }
        message.Deserialize(br);
        
        return message;
    }
}
