using Newtonsoft.Json;

namespace CodeAnalyzerTool.API.ConfigModel;

public class GlobalConfig
{

    [JsonProperty("projectName", Required = Required.Always)]
    public string ProjectName { get; set; }
    [JsonProperty("apiUrl", Required = Required.Default)]
    public Uri ApiUrl { get; set; }
    [JsonProperty("repoUrl", Required = Required.Default)]
    public string RepoUrl { get; set; }
    [JsonProperty("failSeverityThreshold", Required = Required.Default)]
    public Severity FailSeverityThreshold { get; set; }
    [JsonProperty("pluginsPath", Required = Required.Default)]
    public string PluginsPath { get; set; }
    [JsonProperty("plugins", Required = Required.Always)]
    public IEnumerable<PluginConfig> Plugins { get; set; }

    public GlobalConfig(string projectName, Uri apiUrl, string repoUrl, string pluginsPath, IEnumerable<PluginConfig> plugins)
    {
        ProjectName = projectName;
        ApiUrl = apiUrl;
        RepoUrl = repoUrl;
        PluginsPath = pluginsPath;
        Plugins = plugins;
    }
}