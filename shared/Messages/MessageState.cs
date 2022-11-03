namespace AnimalCrossing.Shared;

public class MessageState: Message
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1);
    
    public DateTime ModifiedAt { get; set; }
    public string Hash { get; set; }
    public string Password { get; set; }
    
    public bool Playing { get; set; }

    public MessageState() {}
    
    public MessageState(string password, DateTime modifiedAt, string hash, bool playing)
    {
        this.Type = MessageTypes.State;

        this.Password = password;
        this.ModifiedAt = modifiedAt;
        this.Hash = hash;
        this.Playing = playing;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Password);
        double diff = (this.ModifiedAt - Epoch).TotalSeconds; 
        bw.Write(diff);
        bw.Write(this.Hash);
        bw.Write(this.Playing);
    }

    protected override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Password = br.ReadString();
        this.ModifiedAt = Epoch.AddSeconds(br.ReadDouble());
        this.Hash = br.ReadString();
        this.Playing = br.ReadBoolean();
    }
}