using AnimalCrossing.Shared;

namespace AnimalCrossing.Server;

public class Village : IVillage
{
    public string Password { get; set; }
    public List<Client> Clients { get; private set; } = new List<Client>();

    public DateTime ModifiedAt { get; set; } = new DateTime(1970, 1, 1);
    public string Hash { get; set; } = "A HOUSE IN A MIDDLE OF A";

    private bool _playing;
    private Task _resetPlaying;
    private CancellationTokenSource _cancelResetPlaying = new CancellationTokenSource();

    public bool Playing
    {
        get => this._playing;

        set
        {
            if (this._playing != value)
            {
                // if we are entering playing mode, we set a reset operation after 10min
                if (value)
                {
                    this._cancelResetPlaying.Cancel();
                    this._cancelResetPlaying = new CancellationTokenSource();
                    CancellationToken token = this._cancelResetPlaying.Token;
                    this._resetPlaying = Task.Run(async () =>
                    {
                        try
                        {
                            await Task.Delay(1000 * 60 * 5, token);
                            this.Playing = false;
                        }
                        catch (OperationCanceledException e)
                        {
                            Console.WriteLine("Playing reset canceled.");
                        }
                    }, token);
                }
                this.Clients.ForEach((c) => c.Send(new MessageState(this.Password, this.ModifiedAt, this.Hash, value)));
            }

            this._playing = value;
        }

    }
 
    private Client? _newest;
    
    public Village(string password)
    {
        this.Password = password;
        this.Playing = false;
    }

    ~Village()
    {
        this.Clients.ForEach((c) => c.OnPushReceived -= OnPushClientsChanged);
    }
    
    private byte[]? File
    {
        get
        {
            if (System.IO.File.Exists(this.Password + ".sav"))
            {
                return System.IO.File.ReadAllBytes(this.Password + ".sav");
            }
            return null;
        }

        set
        {
            if (value == null)
            {
                System.IO.File.Delete(this.Password + ".sav");
                return;
            }
            System.IO.File.WriteAllBytes(this.Password + ".sav", value);
        }
    }
    
    public Task<byte[]> GetContent()
    {
        // if we keep the file locally
        if (this.File != null)
        {
            return Task.Run(() => this.File);
        }
        // else 
        TaskCompletionSource<byte[]> source = new TaskCompletionSource<byte[]>();
        Task<byte[]> result = source.Task;

        void OnPushReceived(Client cl, MessagePush push)
        {
            this._newest!.OnPushReceived -= OnPushReceived;
            // TODO: Maybe check if received hash match the one we know
            source.SetResult(push.Content);
        }

        this._newest!.OnPushReceived += OnPushReceived;
        this._newest!.Send(new MessagePull());

        return result;
    }

    
    void OnPushClientsChanged(Client cl, MessagePush push)
    {
        cl.OnPushReceived -= OnPushClientsChanged;
        // check if it's still the newest and that client is not alone
        if (this.Clients.Count > 1 && push.ModifiedAt < this.ModifiedAt) return;
        string h = Message.ComputeHash(push.Content);
        this.Hash = h;
        this.ModifiedAt = push.ModifiedAt;
        // if client is alone
        this.File = this.Clients.Count < 2 ? push.Content : null;
        Console.WriteLine(this.Hash + " -> " + this.ModifiedAt);
        this.Clients.ForEach((c) =>
        {
            if (c == cl) return;
            c.Send(new MessageState(this.Password, this.ModifiedAt, this.Hash, this.Playing));
        });
    }

    public void AddClient(Client client, string hash, DateTime modifiedAt, bool playing)
    {
        this.Clients.Add(client);
        this.Compare(client, hash, modifiedAt, playing);
        // if we have two or more clients we remove the File
        if (this.Clients.Count >= 2)
        {
            this.File = null;
        }
    }

    public void Compare(Client client, string hash, DateTime modifiedAt, bool playing)
    {
        client.OnPushReceived += OnPushClientsChanged;
        if (hash != this.Hash && modifiedAt > this.ModifiedAt)
        {
            client.Send(new MessagePull());
            this._newest = client;
            return;
        }
        // manage playing state
        if (playing != this.Playing)
        {
            this.Playing = playing;
        }
        client.Send(new MessageState(this.Password, this.ModifiedAt, this.Hash, this.Playing));
    }

    public void RemoveClient(Client client)
    {
        client.OnPushReceived -= OnPushClientsChanged;
        this.Clients.Remove(client);
        if (this.Clients.Count == 1)
        {
            this.Clients[0].Send(new MessagePull());  
        }
    }

}