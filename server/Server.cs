using System.Net;
using System.Net.Sockets;

namespace AnimalCrossing.Server;

public class Server
{

    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
    public readonly List<Client> _clients = new List<Client>();

    public Server()
    {
        
    }

    public void Stop()
    {
        this._cancellationTokenSource.Cancel();
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

        while (this._cancellationTokenSource.IsCancellationRequested == false)
        {
            Client client = new Client(listener.AcceptTcpClient(), this._cancellationTokenSource.Token);
            this._clients.Add(client);
            this.ManageClient(client);
        }
    }
}