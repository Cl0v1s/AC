using System.Net;
using System.Net.Sockets;
using System.Threading;
using AnimalCrossing.Shared;


namespace AnimalCrossing.Client
{
    struct UDPState {
        public UdpClient socket;
    }


    class Program
    {
        static void Main(string[] args)
        {
            Dictionary<string, Shared.Client> pairs = new Dictionary<string, Shared.Client>();

            List<Village> village = new List<Village>() { new Village("bonjour") };

            UdpClient socket = new UdpClient();

            IPEndPoint server = new IPEndPoint(IPAddress.Parse(args[0]), int.Parse(args[1]));
            string serverKey = server.Address.ToString()+":"+server.Port;
            pairs[serverKey] =  new Shared.Client(village, socket, server);

            // init connexion
            IMessage intro = new MessageIntro(village[0].Password);
            pairs[serverKey].Send(intro);

            while(true) {
                IPEndPoint pair = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = socket.Receive(ref pair);
                string key = pair.Address.ToString() + ":" + pair.Port;

                if(pairs.ContainsKey(key) == false) pairs[key] = new Shared.Client(village, socket, new IPEndPoint(pair.Address, pair.Port));
                pairs[key].Handle(data);
            }
        }
    }
}