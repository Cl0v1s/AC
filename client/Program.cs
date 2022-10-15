using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;


namespace AnimalCrossing.Client
{
    class Program
    {
        public static List<Pair> Pairs = new List<Pair>();
        
        static void Main(string[] args)
        {
            IPEndPoint server = new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1]));
            Pair pair = new Pair();
            
            pair.Send(server, new MessageCharlyRequest("bonjour"));
                
            while (true)
            {
                pair.Receive();
            }
        }
    }
}