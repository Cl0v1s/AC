namespace AnimalCrossing.Shared;

public class MessagePush : Message
{
    private static readonly DateTime Epoch = new DateTime(1970, 1, 1);
    
    public DateTime ModifiedAt { get; set; }
    public byte[] Content { get; set; }
    
    public MessagePush() {}
    
    public MessagePush(byte[] content, DateTime modifiedAt)
    {
        this.Type = MessageTypes.Push;

        this.ModifiedAt = modifiedAt;
        this.Content = content;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Content.Length);
        bw.Write(this.Content);
        double diff = (this.ModifiedAt - Epoch).TotalSeconds;
        bw.Write(diff);
    }

    protected override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        int length = br.ReadInt32();
        this.Content = br.ReadBytes(length);
        this.ModifiedAt = Epoch.AddSeconds(br.ReadDouble());
    }
}