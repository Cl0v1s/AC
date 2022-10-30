namespace AnimalCrossing.Shared;

public interface IVillage
{
    public string Password { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Hash { get; set; }
    public Task<byte[]> GetContent();
}