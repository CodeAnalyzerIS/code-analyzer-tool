using Newtonsoft.Json;

namespace CAT_API.ConfigModel;

public class GlobalConfig
{

    [JsonProperty("projectId", Required = Required.Always)]
    public string ProjectId { get; set; }
    [JsonProperty("apiUrl", Required = Required.Always)]
    public string ApiUrl { get; set; }

    [JsonProperty("pluginsPath", Required = Required.Always)]
    public string PluginsPath { get; set; }

    [JsonProperty("plugins", Required = Required.Always)]
    public IEnumerable<PluginConfig> Plugins { get; set; }

    public GlobalConfig( string projectId, string apiUrl, string pluginsPath, IEnumerable<PluginConfig> plugins)
    {
        ProjectId = projectId;
        ApiUrl = apiUrl;
        PluginsPath = pluginsPath;
        Plugins = plugins;
    }
}