using System.Net;

namespace AnimalCrossing.Shared;

public class MessageSyncState : Message
{
    private static readonly DateTime Epoch = new DateTime (1970, 01, 01);
    
    public string Hash { get; private set; }
    public DateTime ModifiedAt { get; private set; }
    public bool Playing { get; set; }
    
    public MessageSyncState() {}

    public MessageSyncState(IPEndPoint from, IPEndPoint to, string hash, DateTime modifiedAt, bool playing): base(MessageTypes.SyncState, from, to)
    {
        this.Hash = hash;
        this.ModifiedAt = modifiedAt;
        this.Playing = playing;
    }

    public override void Serialize(BinaryWriter bw)
    {
        base.Serialize(bw);
        bw.Write(this.Hash);
        bw.Write((this.ModifiedAt - MessageSyncState.Epoch).TotalSeconds);
        bw.Write(this.Playing);
    }

    public override void Deserialize(BinaryReader br)
    {
        base.Deserialize(br);
        this.Hash = br.ReadString();
        this.ModifiedAt = Epoch.AddSeconds(br.ReadDouble());
        this.Playing = br.ReadBoolean();
    }
}