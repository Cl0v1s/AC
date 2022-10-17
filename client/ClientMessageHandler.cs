using System.Net;
using System.Net.Sockets;

using AnimalCrossing.Shared;
using System.Net.NetworkInformation;
namespace AnimalCrossing.Client;

/// <summary>
/// Manage incoming and outgoing data
/// </summary>
public class ClientMessageHandler : IMessageHandler
{
    public UdpClient Client { get; }
    public IPEndPoint Self { get; set; }
    
    private Pair Server { get; }
    
    public List<IPEndPoint> Pairs { get; set; }

    private readonly Dictionary<IPEndPoint, CancellationTokenSource> _receipts;

    public ClientMessageHandler(IPEndPoint server)
    {
        this._receipts = new Dictionary<IPEndPoint, CancellationTokenSource>();
        this.Pairs = new List<IPEndPoint>();
        this.Self = new ClientPair(this, new AnimalCrossing.Shared.File(Config.Instance.SaveFile), IPAddress.Any, 0);
        this.Client = new UdpClient();
        try
        {
            this.Client.DontFragment = true;
        }
        catch (SocketException e)
        {
            Console.WriteLine("Dont Fragment not supported.");
        }

        this.Server = new ServerPair(this, server.Address, server.Port);
        
        AppDomain.CurrentDomain.ProcessExit += new EventHandler (this.OnProcessExit);
        Console.CancelKeyPress += this.OnProcessExit;
    }

    /// <summary>
    /// Send a bye message to server before quitting
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="args"></param>
    private void OnProcessExit(object? obj, EventArgs args)
    {
        Console.WriteLine("Quitting....");
        this.Send(new MessageBye(this.Self, this.Server), false);
        this.Pairs.ForEach((pair) =>
        {
            this.Send(new MessageBye(this.Self, pair), false);
        });
    }

    /// <summary>
    /// Find the pair for a given endpoint
    /// </summary>
    /// <param name="sender">endpoint to search a pair for</param>
    /// <returns></returns>
    public Pair FindPair(IPEndPoint sender)
    {
        if (sender.Address.ToString() == this.Server.Address.ToString() && sender.Port == this.Server.Port)
        {
            return this.Server;
        }
        ClientPair? pair =
            this.Pairs.Find(x => x.Address.ToString() == sender.Address.ToString() && x.Port == sender.Port) as ClientPair;
        if (pair == null)
        {
            pair = new ClientPair(this, sender.Address, sender.Port);
            this.Pairs.Add(pair);
        }

        return pair;
    }
    
    /// <summary>
    /// Watch for new data
    /// </summary>
    public void Receive()
    {
        IPEndPoint raw = new IPEndPoint(IPAddress.Any, 0);
        byte[] data = this.Client.Receive(ref raw);

        Pair sender = this.FindPair(raw);
        
        // manage receipt validation
        if (this._receipts.ContainsKey(sender))
        {
            this._receipts[sender].Cancel();
            this._receipts.Remove(sender);
        }
        
        IMessage? message = IMessage.Parse(data);
        if (message == null)
        {
            Console.WriteLine("Unknown message from "+ sender.Address + ":" + sender.Port);
            return;
        }
        Console.WriteLine("Incoming " + message.Type + " from " + sender.Address + ":" + sender.Port);

        sender.Handle(this, message);
    }

    /// <summary>
    /// Ressend a message every 3s if we dont get any response
    /// </summary>
    /// <param name="message">Message to resend</param>
    /// <param name="token">Cancellation token</param>
    private async void EnsureResponse(IMessage message, CancellationToken token)
    {
        try
        {
            while (token.IsCancellationRequested == false)
            {
                await Task.Delay(3000, token);
                // false since we are already ensuring response
                this.Send(message, false);
            }
        }
        catch (TaskCanceledException e)
        {
            
        }
    }
    
    /// <summary>
    /// Send a message 
    /// </summary>
    /// <param name="message">message to send</param>
    /// <param name="needResponse">true if we need to wit for a receipt from the receiver of the message</param>
    public void Send(IMessage message, bool needResponse)
    {
        MemoryStream stream = new MemoryStream();
        BinaryWriter bw = new BinaryWriter(stream);
        message.Serialize(bw);
        byte[] data = stream.ToArray();
        Console.WriteLine("Sending " + message.Type + " to " + message.To.Address + ":" + message.To.Port);
        this.Client.Send(data, data.Length, message.To);
        bw.Close();
        stream.Close();

        // manage receipt start
        if (!needResponse) return;
        CancellationTokenSource source = new CancellationTokenSource();
        this._receipts.Add(message.To, source);
        this.EnsureResponse(message, source.Token);
    }


}