using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;


namespace AnimalCrossing.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            // Config.Instance.SaveFile = args[0];
            Server server = new Server();
            server.Start().Wait();
        }
    }
}