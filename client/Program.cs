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
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1]));
            ClientMessageHandler clientMessageHandler = new ClientMessageHandler();
            
            clientMessageHandler.Send(server, new MessageCharlyRequest("bonjour"));
                
            while (true)
            {
                clientMessageHandler.Receive();
            }
        }
    }
}