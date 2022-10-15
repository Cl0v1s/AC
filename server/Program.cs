using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Server {
    class Program {
        private static readonly Dictionary<string, ClientUser> Clients = new Dictionary<string, ClientUser>();
        public static List<Village> Villages = new List<Village>();

        public static void Main(string[] args) {
            UdpClient server = new UdpClient(new IPEndPoint(IPAddress.Any, int.Parse(args[0])));
            Console.WriteLine("Server started...");

            while(true) {
                IPEndPoint pair = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = server.Receive(ref pair);

                string key = pair.Address.ToString();
#if DEBUG
                key = key + ":" + pair.Port;
#endif
                if(Clients.ContainsKey(key) == false) {
                    Clients[key] = new ClientUser(server, pair.Address, pair.Port);
                }
                Clients[key].Receive(data);
            }
        }
    }
}