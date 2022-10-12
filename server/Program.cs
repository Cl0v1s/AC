using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;

namespace AnimalCrossing.Server {
    class Program {
        public static List<Village> villages = new List<Village>();

        public static void Main(string[] args) {
            IPAddress ip = IPAddress.Any;
            TcpListener server = new TcpListener(ip, int.Parse(args[0]));

            server.Start();
            Console.WriteLine("Server started...");

            while(true) {
                TcpClient client = server.AcceptTcpClient();
                Client c = new Client(client);
                c.start();
            }
        }
    }
}