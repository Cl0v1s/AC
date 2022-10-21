using AnimalCrossing.Shared;
using shared;

namespace AnimalCrossing.Client;

public class Village : IVillage
{
    public static Village Instance = new Village(Config.Instance.Password);
    
    public string Password { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Hash { get; set; }
    public byte[] File { get; set; }

    public Village(string password)
    {
        this.Password = password;
        this.Load();
    }

    private void Load()
    {
        this.ModifiedAt = System.IO.File.GetLastWriteTime(Config.Instance.SaveFile);
        this.File = System.IO.File.ReadAllBytes(Config.Instance.SaveFile);
        this.Hash = Message.ComputeHash(this.File);
    }

    public void Save(byte[] content)
    {
        string newHash = Message.ComputeHash(content);
        if (newHash == this.Hash) return;
        string fileName = Path.GetFileNameWithoutExtension(Config.Instance.SaveFile) + "-" + DateTime.Now.ToString("dd-MM-yyy-hh-mm-ss") + Path.GetExtension(Config.Instance.SaveFile);
        string newPath = Path.Combine(Path.GetDirectoryName(Config.Instance.SaveFile)!, fileName);
        System.IO.File.Copy(Config.Instance.SaveFile, newPath);
        this.File = content;
        System.IO.File.WriteAllBytes(Config.Instance.SaveFile, this.File);

        this.Load();
    }
}