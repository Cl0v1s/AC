namespace AnimalCrossing.Shared;

public class MessageState: Message
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1);
    
    public DateTime ModifiedAt { get; set; }
    public string Hash { get; set; }
    public string Password { get; set; }

    public MessageState() {}
    
    public MessageState(string password, DateTime modifiedAt, string hash)
    {
        this.Type = MessageTypes.State;

        this.Password = password;
        this.ModifiedAt = modifiedAt;
        this.Hash = hash;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Password);
        //bw.Write((this.ModifiedAt - Epoch).TotalSeconds);
        //bw.Write(this.Hash);
    }

    protected override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Password = br.ReadString();
        //this.ModifiedAt = Epoch.AddSeconds(br.ReadInt32());
        //this.Hash = br.ReadString();
    }
}