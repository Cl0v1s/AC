using System.IO;

using AnimalCrossing.Shared;
namespace AnimalCrossing.Client;

public class FileChangedEventArgs
{
    public Shared.File File { get; set; }

    public FileChangedEventArgs(string path)
    {
        this.File = new Shared.File(path);
    }
}

public class LocalFile
{
    public delegate void FileChangedHandler(object source, FileChangedEventArgs e);
    
    private readonly string _path;
    private readonly FileSystemWatcher _watcher;
    public event FileChangedHandler? FileChanged;
    
    public LocalFile(string path)
    {
        FileSystemWatcher watcher;
        this._path = path;
        this._watcher = new FileSystemWatcher(Path.GetDirectoryName(path)!);
        this._watcher.Created += this.SomethingChanged;
        this._watcher.Changed += this.SomethingChanged;
        this._watcher.EnableRaisingEvents = true;
    }

    private void SomethingChanged(object obj, FileSystemEventArgs args)
    {
        if (args.Name != Path.GetFileName(this._path) || this.FileChanged == null) return;
        this.FileChanged(this, new FileChangedEventArgs(this._path));
    }
}