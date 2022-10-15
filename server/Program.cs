using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Server {
    class Program {
        public static void Main(string[] args)
        {
            IPEndPoint self = new IPEndPoint(IPAddress.Any, int.Parse(args[0]));
            UdpClient socket = new UdpClient(self);
            ServerMessageHandler server = new ServerMessageHandler(self, socket);
            Console.WriteLine("Server started...");

            while(true) {
                IPEndPoint pair = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = socket.Receive(ref pair);
                server.Receive(pair, data);
            }
        }
    }
}