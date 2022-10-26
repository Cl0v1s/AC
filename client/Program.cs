using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;


namespace AnimalCrossing.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Config.Instance.SaveFile = args[0];
            Program.Run();
        }

        static void Run()
        {
            while (true)
            {
                try
                {
                    Server server = new Server();
                    server.Start().Wait();
                }
                catch (SocketException e)
                {
                    Console.WriteLine("Connection Failed... Retrying in 5s");
                }

                Task.Delay(5000).Wait();
            }
        }
    }
}