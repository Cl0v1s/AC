using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

using AnimalCrossing.Shared;

namespace AnimalCrossing.Server {
    class Program {
        public static System.Collections.Generic.Dictionary<string, Client> clients = new System.Collections.Generic.Dictionary<string, Client>();
        public static List<Village> villages = new List<Village>();

        public static void Main(string[] args) {
            IPAddress ip = IPAddress.Any;
            UdpClient server = new UdpClient(new IPEndPoint(IPAddress.Any, int.Parse(args[0])));
            Console.WriteLine("Server started...");

            while(true) {
                IPEndPoint pair = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = server.Receive(ref pair);

                string key = pair.Address.ToString() + ":" + pair.Port;
                if(clients.ContainsKey(key) == false) {
                    clients[key] = new Client(Program.villages, server, new IPEndPoint(pair.Address, pair.Port));
                }
                clients[key].Handle(data);
            }
        }
    }
}