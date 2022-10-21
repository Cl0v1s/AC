using AnimalCrossing.Shared;
using shared;

namespace AnimalCrossing.Server;

public class Village : IVillage
{
    public string Password { get; set; }
    public List<Client> Clients { get; set; } = new List<Client>();

    public DateTime ModifiedAt { get; set; } = new DateTime(1970, 1, 1);
    public string Hash { get; set; } = "A HOUSE IN A MIDDLE OF A";

    private byte[]? _file;

    private Client? _newest;
    
    public Village(string password)
    {
        this.Password = password;
    }

    public byte[]? File
    {
        get => this._file;

        set
        {
            this._file = value;
            if (value == null) return;
            this._newest = null;
        }
    }

    public Client? Newest
    {
        get => this._newest;

        set
        {
            this._newest = value;
            if (value == null) return;
            // we free the previous stocked file
            this._file = null;
        }
    }

}