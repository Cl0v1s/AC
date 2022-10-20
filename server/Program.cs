using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Server {
    class Program {
        public static void Main(string[] args)
        {
            Server server = new Server();
            server.Start(int.Parse(args[0]));
        }
    }
}