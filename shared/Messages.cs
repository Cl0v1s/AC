using System.Net;
namespace AnimalCrossing.Shared;

public enum MessageTypes {
    Intro = 0,
}

public interface IMessage {
    MessageTypes Type { get; set; }

    public void Act(List<Village> villages);

    public void Serialize(BinaryWriter bw);

    public void Deserialize(BinaryReader br);

    public static IMessage Parse(byte[] data) {
        MemoryStream stream = new MemoryStream(data);
        BinaryReader br = new BinaryReader(stream);

        MessageTypes type = (MessageTypes)br.ReadInt32();

        switch(type) {
            case MessageTypes.Intro:
                return new MessageIntro(br);
        }
        throw new FormatException("Message type does not exists");
    }
}


[Serializable]
public class MessageIntro : IMessage {
    public MessageTypes Type { get; set; }

    public string Password { get; set; }
    public IPAddress Address { get; set; }

    public int Port { get; set; }

    public MessageIntro(BinaryReader br) {
        this.Type = MessageTypes.Intro;

        this.Deserialize(br);
    }

    public MessageIntro(string password, IPAddress address, int port) {
        this.Type = MessageTypes.Intro;
        this.Password = password;
        this.Address = address;
        this.Port = port;
    }

    public void Act(List<Village> villages) {
        Village? village = villages.Find(x => x.Password == this.Password);
        if(village == null) {
            village = new Village(this.Password);
        }

        village.addVillager(new Villager(this.Address, this.Port));
    }

    public void Serialize(BinaryWriter bw) {
        bw.Write((int)this.Type);
        bw.Write(this.Password);
        byte[] addr = this.Address.GetAddressBytes();
        bw.Write(addr.Length);
        bw.Write(addr);
        bw.Write(this.Port);
    }

    public void Deserialize(BinaryReader br) {
        // Type is already parsed
        this.Password = br.ReadString();
        int length = br.ReadInt32();
        this.Address = new IPAddress(length);
        this.Port = br.ReadInt32();
    } 
}