using System.ComponentModel;
using System.Net;
using System.Text;

namespace AnimalCrossing.Shared;

public class MessageSyncResponse : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }
    
    public int Index { get; set; }
    
    public byte[] Content { get; set; }
    
    public int Length { get; set; }
    
    public MessageSyncResponse() {}

    public MessageSyncResponse(IPEndPoint from, IPEndPoint to, int index, byte[] content, int length)
    {
        this.Type = MessageTypes.SyncResponse;
        // requested side
        this.From = from;
        this.To = to;

        this.Index = index;
        this.Content = content;
        this.Length = length;
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        bw.Write(this.Index);
        bw.Write(this.Content.Length);
        bw.Write(this.Content);
        bw.Write(this.Length);
    }

    public void Deserialize(BinaryReader br)
    {
        IMessage.Deserialize(br, this);
        this.Index = br.ReadInt32();
        int len = br.ReadInt32();
        this.Content = br.ReadBytes(len);
        this.Length = br.ReadInt32();
    }
}