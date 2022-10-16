using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;


namespace AnimalCrossing.Client
{
    class Program
    {
        public static List<ClientMessageHandler> Pairs = new List<ClientMessageHandler>();
        
        static void Main(string[] args)
        {
            Config.Instance.SaveFile = args[0];
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(Config.Instance.ServerAddress), Config.Instance.ServerPort);
            ClientMessageHandler clientMessageHandler = new ClientMessageHandler(server);
            
            while (true)
            {
                clientMessageHandler.Receive();
            }
        }
    }
}