using System.Net;

namespace AnimalCrossing.Shared;

public class MessageCharly : Message
{
    public string Password { get; set; }
    
    public MessageCharly() {}
    
    public MessageCharly(IPEndPoint from, IPEndPoint to, string password) : base(MessageTypes.Charly, from, to)
    {
        this.Password = password;
    }
    
    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Password);
    }

    protected override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Password = br.ReadString();
    }
}