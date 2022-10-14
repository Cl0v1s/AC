using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Server {
    class Client {
        private UdpClient socket;
        private IPEndPoint pair;
        public Client(UdpClient socket, IPEndPoint pair) {
            this.socket = socket;
            this.pair = pair;
        }

        public void handle(byte[] data) {

            IMessage message = IMessage.Parse(data);
            message.Act(this.pair, Program.villages);
        }

        public void send(IMessage message) {

        }
    }
}