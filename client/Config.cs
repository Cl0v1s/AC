using System.Text;
using Newtonsoft.Json;
using System.IO;

namespace AnimalCrossing.Client;

public class Config
{
    private static readonly string ConfigPath =  System.IO.Directory.GetCurrentDirectory() + "/config.json";
    
    public static Config Instance = File.Exists(ConfigPath) ? 
        JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath, Encoding.UTF8))! : new Config();

    public string SaveFile { get; set; } = "save.file";
    public string ServerAddress { get; set; } = "127.0.0.1";
    public int ServerPort { get; set; } = 8888;
    public string Password { get; set; } = "TEST";

    public string Emulator { get; set; } = "Dolphin";
}