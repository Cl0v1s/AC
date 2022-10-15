using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Shared;

public enum MessageTypes {
    Intro = 0,
    Pair = 1,
    Hi = 2,
}

public interface IMessage {
    MessageTypes Type { get; set; }

    public void Act(IOther client);

    public void Serialize(BinaryWriter bw);

    public void Deserialize(BinaryReader br);

    public static IMessage? Parse(byte[] data) {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        MessageTypes type = (MessageTypes)br.ReadInt32();

        switch(type) {
            case MessageTypes.Intro:
                return new MessageIntro(br);
            case MessageTypes.Pair:
                return new MessagePair(br);
        }
        // throw new FormatException("Message type does not exists");
        return null;
    }
}
