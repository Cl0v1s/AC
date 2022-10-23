using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Server;

public class Server
{

    private List<Client> _clients = new List<Client>();

    public Server()
    {
        
    }

    private async void ManageClient(Client client)
    {
        Console.WriteLine("Clients: " + this._clients.Count);
        await client.Start();
        this._clients.Remove(client);
        Console.WriteLine("Clients: "+this._clients.Count);
    }

    public void Start(int port)
    {
        TcpListener listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine("Server started...");

        while (true)
        {
            Client client = new Client(listener.AcceptTcpClient());
            this._clients.Add(client);
            this.ManageClient(client);
        }
    }
}