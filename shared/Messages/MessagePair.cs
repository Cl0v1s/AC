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

        public Village? Act(Client client) {

            return null;
        }

        public void Serialize(BinaryWriter bw) {
            byte[] ip = this.pair.Address.GetAddressBytes();
            bw.Write(ip.Length);
            bw.Write(ip);
            bw.Write(this.pair.Port);
        }

        public void Deserialize(BinaryReader br) {
            int len = br.ReadInt32();
            byte[] ip = br.ReadBytes(len);
            int port = br.ReadInt32();
            this.pair = new IPEndPoint(new IPAddress(ip), port);
        }
    }
}