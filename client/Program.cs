using System.Net;
using System.Net.Sockets;
using AnimalCrossing.Shared;


namespace AnimalCrossing.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Village village = new Village("bonjour");

            Pair server = new Pair(village, IPAddress.Parse(args[0]), int.Parse(args[1]));
            server.Send(new MessageIntro(village.Password));
        }
    }
}