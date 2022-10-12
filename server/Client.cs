using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Server {
    class Client {
        private TcpClient socket;
        private Thread? thread;

        public Client(TcpClient socket) {
            this.socket = socket;
        }

        public void start() {
            this.thread = new Thread(this.handle);
            this.thread.Start();
        }

        private void handle() {
            NetworkStream network = this.socket.GetStream();
            byte[] bytes = new byte[this.socket.ReceiveBufferSize];
            network.Read(bytes, 0, this.socket.ReceiveBufferSize);

            IMessage message = IMessage.Parse(bytes);
            message.Act(Program.villages);
        }
    }
}