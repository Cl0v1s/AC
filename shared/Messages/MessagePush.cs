namespace AnimalCrossing.Shared;

public class MessagePush : Message
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1);
    
    public DateTime ModifiedAt { get; set; }
    public byte[] Content { get; set; }
    
    public MessagePush() {}
    
    public MessagePush(byte[] content)
    {
        this.Type = MessageTypes.Push;
        
        this.Content = content;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Content.Length);
        bw.Write(this.Content);
        bw.Write((this.ModifiedAt - Epoch).TotalSeconds);
    }

    protected override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Content = br.ReadBytes(br.ReadInt32());
        this.ModifiedAt = Epoch.AddSeconds(br.ReadInt32());
    }
}