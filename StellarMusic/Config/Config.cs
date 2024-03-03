using Newtonsoft.Json;

namespace StellarMusic.Config;

public class Config
{
    public static Models.Config Current { get; private set; } = new();
    
    public void Save()
    {
        const string path = "./data/config.json";

        var json = JsonConvert.SerializeObject(Current, Formatting.Indented);
        File.WriteAllText(path, json);
    }
    
    public static void Initialize()
    {
        const string path = "./data/config.json";

        if (!File.Exists(path))
        {
            try
            {
                var json2 = JsonConvert.SerializeObject(Current, Formatting.Indented);
                File.WriteAllText(path, json2);
            }
            catch (Exception e)
            {
                throw new FileNotFoundException("Could not find config file and failed to create one!");
            }
        }

        var json = File.ReadAllText(path);
        var config = JsonConvert.DeserializeObject<Models.Config>(json);

        if (config is null) return;
        
        Current = config;
    }
}