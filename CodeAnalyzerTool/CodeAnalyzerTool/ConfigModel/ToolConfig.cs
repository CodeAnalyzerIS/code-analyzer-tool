using Newtonsoft.Json;

namespace CodeAnalyzerTool.ConfigModel;

public class ToolConfig
{
    [JsonProperty("apiUrl", Required = Required.Always)]
    public string ApiUrl { get; set; }
    
    [JsonProperty("pluginsPath", Required = Required.Always)]
    public string PluginsPath { get; set; }
    
    [JsonProperty("plugins", Required = Required.Always)]
    public IEnumerable<PluginConfig> Plugins { get; set; }
    
    public ToolConfig(string apiUrl, string pluginsPath, IEnumerable<PluginConfig> plugins)
    {
        ApiUrl = apiUrl;
        PluginsPath = pluginsPath;
        Plugins = plugins;
    }
}