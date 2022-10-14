using System.Net;
using System.Net.Sockets;


namespace AnimalCrossing.Shared {
    public class Client {
        private UdpClient socket;
        public IPEndPoint Pair { get;}

        public List<Village> Villages { get; }
        public Client(List<Village> villages,  UdpClient socket, IPEndPoint pair) {
            this.socket = socket;
            this.Pair = pair;
            this.Villages = villages;
        }

        public void Handle(byte[] data) {
            IMessage message = IMessage.Parse(data);
            Village? modified = message.Act(this);
        }

        public void Send(IMessage message, IPAddress address, int port) {
            Console.WriteLine("Sending " + message.Type + " to " + address + ":" + port);
            MemoryStream stream = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(stream);
            message.Serialize(bw);
            byte[] data = stream.ToArray();
            if(this.socket.Client.Connected) {
                this.socket.Send(data);
            } else {
                this.socket.Send(data, data.Length, new IPEndPoint(address, port));
            }
            bw.Close();
            stream.Close();
        }

        public void Send(IMessage message) {
            this.Send(message, this.Pair.Address, this.Pair.Port);
        }
    }
}