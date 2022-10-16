using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncRequest : IMessage
{
    public MessageTypes Type { get; set; }
    public IPEndPoint? ReplyTo { get; set; }
    public IPEndPoint? From { get; set; }
    public IPEndPoint To { get; set; }
    
    public int MTU { get; set; }
    
    public byte[]? PartsToSend { get; set; }
    
    public MessageSyncRequest() {}

    public MessageSyncRequest(IPEndPoint from, IPEndPoint to, int mtu)
    {
        // requester side
        this.Type = MessageTypes.SyncRequest;
        this.From = from;
        this.To = to;

        this.MTU = mtu;
    }

    public MessageSyncRequest(IPEndPoint from, IPEndPoint to, int mtu, byte[] partsToSend) : this(from, to, mtu)
    {
        // requester side
        this.PartsToSend = partsToSend;
    }
    
    public IMessage[] Act(IMessageHandler client, IPEndPoint sender)
    {
        // requested side
        Pair thisSide = (Pair)client.Self;
        byte[][] parts = thisSide.File!.Split(this.MTU);

        IMessage[] responses;
        if (this.PartsToSend == null)
        {
            responses = new IMessage[parts.Length];
            for (int i = 0; i < parts.Length; i++)
            {
                responses[i] = new MessageSyncResponse(thisSide, this.From!, i, parts[i], parts.Length);
            }
        }
        else
        {
            responses = new IMessage[this.PartsToSend.Length];
            for (int i = 0; i < this.PartsToSend.Length; i++)
            {
                int index = (int)this.PartsToSend[i];
                responses[i] = new MessageSyncResponse(thisSide, this.From!, index, parts[index], parts.Length);
            }
        }

        return responses;
    }

    public void Serialize(BinaryWriter bw)
    {
        IMessage.Serialize(bw, this);
        bw.Write(this.MTU);
        bw.Write(this.PartsToSend?.Length ?? 0);
        bw.Write(this.PartsToSend ?? new byte[]{});
    }

    public void Deserialize(BinaryReader br)
    {
        IMessage.Deserialize(br, this);
        this.MTU = br.ReadInt32();
        int len = br.ReadInt32();
        this.PartsToSend = len == 0 ? null : br.ReadBytes(len);
    }
}