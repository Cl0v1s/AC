using System.Text;
using Newtonsoft.Json;

namespace AnimalCrossing.Client;

public class Config
{
    private const string ConfigPath = "./config.json";
    
    public static Config Instance = File.Exists(ConfigPath) ? 
        JsonConvert.DeserializeObject<Config>(File.ReadAllText(ConfigPath, Encoding.UTF8))! : new Config();

    public string SaveFile { get; set; } = "save.file";
    public string ServerAddress { get; set; } = "127.0.0.1";
    public int ServerPort { get; set; } = 8888;
}