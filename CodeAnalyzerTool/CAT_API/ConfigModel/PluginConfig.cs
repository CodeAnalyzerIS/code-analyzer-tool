using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace CAT_API.ConfigModel;

public class PluginConfig
{
    [JsonProperty("pluginName", Required = Required.Always)]
    public string PluginName { get; set; }

    [JsonProperty("assemblyName", Required = Required.Default)]
    public string? AssemblyName { get; set; } // todo validate that assemblyName is present if plugin is external

    [JsonProperty("folderName", Required = Required.Always)]
    public string FolderName { get; set; }

    [JsonProperty("enabled", Required = Required.Always)]
    public bool Enabled { get; set; }

    [JsonProperty("rules", Required = Required.Always)]
    public IEnumerable<RuleConfig> Rules { get; set; }

    public PluginConfig(string pluginName, string folderName, IEnumerable<RuleConfig> rules, bool enabled)
    {
        PluginName = pluginName;
        FolderName = folderName;
        Rules = rules;
        Enabled = enabled;
    }

    [JsonConstructor]
    public PluginConfig(string pluginName, string folderName, IEnumerable<RuleConfig> rules, bool enabled,
        string assemblyName) : this(pluginName, folderName, rules, enabled)
    {
        AssemblyName = assemblyName;
    }
}