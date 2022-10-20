namespace AnimalCrossing.Server;

public class Village
{
    public string Password { get; set; }
    public List<Client> Clients { get; set; } = new List<Client>();

    public DateTime ModifiedAt { get; set; } = new DateTime(1970, 1, 1);
    public string Hash { get; set; } = "A HOUSE IN A MIDDLE OF A";

    public byte[]? File { get; set; }
    public Client? Newest { get; set; }
    
    public Village(string password)
    {
        this.Password = password;
    }
}