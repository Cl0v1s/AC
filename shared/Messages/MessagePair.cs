using System.Net;

namespace AnimalCrossing.Shared {

    public class MessagePair : IMessage {
        public MessageTypes Type { get; set;}

        private IPEndPoint pair;

        public MessagePair(IPEndPoint pair) {
            this.Type = MessageTypes.Pair;
            this.pair = pair;
        }


        public MessagePair(BinaryReader br) {
            this.Type = MessageTypes.Pair;
            this.Deserialize(br);
        }

        public void Act(IOther client)
        {
            Console.WriteLine("Pair to " + this.pair.Address + ":" + this.pair.Port + " received");
        }

        public void Serialize(BinaryWriter bw) {
            bw.Write((int)this.Type);
            byte[] ip = this.pair.Address.GetAddressBytes();
            bw.Write(ip.Length);
            bw.Write(ip);
            bw.Write(this.pair.Port);
        }

        public void Deserialize(BinaryReader br) {
            // type is already parsed
            int len = br.ReadInt32();
            byte[] ip = br.ReadBytes(len);
            int port = br.ReadInt32();
            this.pair = new IPEndPoint(new IPAddress(ip), port);
        }
    }
}