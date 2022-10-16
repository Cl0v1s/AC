using System.IO;
using System.Security.Cryptography;

namespace AnimalCrossing.Shared;

public class File
{
    public string Hash { get; set; }
    public DateTime ModifiedAt { get; set; }
    public byte[]? Content { get; set; }

    public File(string path)
    {
        this.Content = System.IO.File.ReadAllBytes(path);

        SHA256 sha = SHA256.Create();
        this.Hash = System.Text.Encoding.ASCII.GetString((sha.ComputeHash(this.Content)));
        this.ModifiedAt = System.IO.File.GetLastWriteTime(path);
    }

    public File(string hash, DateTime modifiedAt)
    {
        this.Hash = hash;
        this.ModifiedAt = modifiedAt;
    }

    public byte[][] Split(int mtu)
    {
        int size = Math.Min(this.Content!.Length, mtu);
        return this.Content!.Chunk(size).ToArray();
    }
}