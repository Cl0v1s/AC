using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncRequest : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint From { get; set; }
    public IPEndPoint To { get; set; }
    
    public int Mtu { get; private set; }
    
    public byte[]? PartsToSend { get; set; }
    
    public MessageSyncRequest() {}

    public MessageSyncRequest(IPEndPoint from, IPEndPoint to, int mtu)
    {
        // requester side
        this.Type = MessageTypes.SyncRequest;
        this.From = from;
        this.To = to;

        this.Mtu = mtu;
    }

    public MessageSyncRequest(IPEndPoint from, IPEndPoint to, int mtu, byte[] partsToSend) : this(from, to, mtu)
    {
        // requester side
        this.PartsToSend = partsToSend;
    }
    

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        bw.Write(this.Mtu);
        bw.Write(this.PartsToSend?.Length ?? 0);
        bw.Write(this.PartsToSend ?? new byte[]{});
    }

    public void Deserialize(BinaryReader br)
    {
        IMessage.Deserialize(br, this);
        this.Mtu = br.ReadInt32();
        int len = br.ReadInt32();
        this.PartsToSend = len == 0 ? null : br.ReadBytes(len);
    }
}