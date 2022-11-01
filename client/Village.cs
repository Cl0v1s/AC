using System;
using AnimalCrossing.Shared;
using System.IO;
using System.Threading.Tasks;
using ACSE.Core;
using ACSE.Core.Housing;
using ACSE.Core.Players;
using ACSE.Core.Saves;

namespace AnimalCrossing.Client;

public class Village : IVillage
{
    public static readonly Village Instance = new Village(Config.Instance.Password);

    public delegate void FileChangeHandler();
    public event FileChangeHandler? FileChanged;
    private FileSystemWatcher _watcher;
    
    public string Password { get; set; }
    public DateTime ModifiedAt { get; set; }
    public string Hash { get; set; }
    public Task<byte[]> GetContent()
    {
        return Task.Run(() => this._content!) ;
    }

    private byte[]? _content;

    private House[]? _houses;
    private Player[]? _players;

    private Village(string password)
    {
        this._watcher = new FileSystemWatcher(Path.GetDirectoryName(Path.GetFullPath(Config.Instance.SaveFile))!);
        this._watcher.NotifyFilter = NotifyFilters.LastWrite;
        this._watcher.Changed += this.FileMayHaveChanged;
        this._watcher.EnableRaisingEvents = true;

        this.Password = password;
        this.Load();
    }

    private void FileMayHaveChanged(object obj, FileSystemEventArgs args)
    {
        if (args.Name != Path.GetFileName(Config.Instance.SaveFile)) return;
        this._watcher.Changed -= this.FileMayHaveChanged;
        this.Load();
        this.FileChanged?.Invoke();
        this.EnableWatch();
    }

    private async void EnableWatch()
    {
        await Task.Delay(100);
        this._watcher.Changed += this.FileMayHaveChanged;
    }

    private void Load()
    {
        this.ModifiedAt = System.IO.File.GetLastWriteTime(Config.Instance.SaveFile);
        this._content = System.IO.File.ReadAllBytes(Config.Instance.SaveFile);
        this.Hash = Message.ComputeHash(this._content);

        Save save = new Save(Config.Instance.SaveFile);
        
        this._houses = HouseInfo.LoadHouses(save);
        this._players = new Player[4];
        for (int i = 0; i < 4; i++)
        {
            this._players[i] = new Player(save.SaveDataStartOffset + save.SaveInfo.SaveOffsets.PlayerStart + i * save.SaveInfo.SaveOffsets.PlayerSize, i);
        }
    }

    public void Save(byte[] content)
    {
        string newHash = Message.ComputeHash(content);
        if (newHash == this.Hash) return;
        string fileName = Path.GetFileNameWithoutExtension(Config.Instance.SaveFile) + "-" + DateTime.Now.ToString("dd-MM-yyy-hh-mm-ss") + Path.GetExtension(Config.Instance.SaveFile);
        string newPath = Path.Combine(Path.GetDirectoryName(Config.Instance.SaveFile)!, fileName);
        System.IO.File.Copy(Config.Instance.SaveFile, newPath);
        this._content = content;
        System.IO.File.WriteAllBytes(Config.Instance.SaveFile, this._content);

        this.Load();
    }
}