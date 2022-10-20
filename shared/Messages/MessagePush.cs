namespace AnimalCrossing.Shared;

public class MessagePush : Message
{
    
    public byte[] Content { get; set; }
    
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
    }

    protected override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Content = br.ReadBytes(br.ReadInt32());
    }
}