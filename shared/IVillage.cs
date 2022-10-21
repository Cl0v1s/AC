namespace shared;

public interface IVillage
{
    public string Password { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Hash { get; set; }
    public byte[]? File { get; set; }
}