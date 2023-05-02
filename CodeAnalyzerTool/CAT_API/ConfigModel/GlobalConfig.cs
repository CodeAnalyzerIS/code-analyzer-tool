using Newtonsoft.Json;

namespace CAT_API.ConfigModel;

public class GlobalConfig
{
    [JsonProperty("apiUrl", Required = Required.Always)]
    public string ApiUrl { get; set; }
    
    [JsonProperty("pluginsPath", Required = Required.Always)]
    public string PluginsPath { get; set; }
    
    [JsonProperty("plugins", Required = Required.Always)]
    public IEnumerable<PluginConfig> Plugins { get; set; }
    
    public GlobalConfig(string apiUrl, string pluginsPath, IEnumerable<PluginConfig> plugins)
    {
        ApiUrl = apiUrl;
        PluginsPath = pluginsPath;
        Plugins = plugins;
    }
}