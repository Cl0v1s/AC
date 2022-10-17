using System.ComponentModel;
using System.Net;
using System.Text;

namespace AnimalCrossing.Shared;

public class MessageSyncResponse : Message
{
    public int Index { get; set; }
    
    public byte[] Content { get; set; }
    
    public int Length { get; set; }
    
    public MessageSyncResponse() {}
    
    public MessageSyncResponse(IPEndPoint from, IPEndPoint to, int index, byte[] content, int length): base(MessageTypes.SyncResponse, from, to)
    {
        this.Index = index;
        this.Content = content;
        this.Length = length;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Index);
        bw.Write(this.Content.Length);
        bw.Write(this.Content);
        bw.Write(this.Length);
    }

    public override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Index = br.ReadInt32();
        int len = br.ReadInt32();
        this.Content = br.ReadBytes(len);
        this.Length = br.ReadInt32();
    }
}