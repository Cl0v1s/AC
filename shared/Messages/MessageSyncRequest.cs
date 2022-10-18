using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncRequest : Message
{
    public int Mtu { get; private set; }
    
    public byte[]? PartsToSend { get; set; }
    
    public MessageSyncRequest() {}

    public MessageSyncRequest(IPEndPoint from, IPEndPoint to, int mtu) : base(MessageTypes.SyncRequest, from, to)
    {
        this.Mtu = mtu;
    }

    public MessageSyncRequest(IPEndPoint from, IPEndPoint to, int mtu, byte[] partsToSend) : this(from, to, mtu)
    {
        this.PartsToSend = partsToSend;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Mtu);
        bw.Write(this.PartsToSend?.Length ?? 0);
        bw.Write(this.PartsToSend ?? new byte[]{});
    }

    protected override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Mtu = br.ReadInt32();
        int len = br.ReadInt32();
        this.PartsToSend = len == 0 ? null : br.ReadBytes(len);
    }
}