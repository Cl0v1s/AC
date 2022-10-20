using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Server;

public class Server
{

    private List<Client> _clients = new List<Client>();

    public Server()
    {
        
    }

    public void Start(int port)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Server started...");

        while (true)
        {
            Client client = new Client(listener.AcceptTcpClient());
            _clients.Add(client);
        }
    }
}