using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

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
            
        }
    }
}