using Newtonsoft.Json;

namespace CAT_API.ConfigModel;

public class PluginConfig
{
    [JsonProperty("pluginName", Required = Required.Always)]
    public string PluginName { get; set; }
    
    [JsonProperty("enabled", Required = Required.Always)]
    public bool Enabled { get; set; }
    
    [JsonProperty("rules", Required = Required.Always)]
    public IEnumerable<RuleConfig> Rules { get; set; }
    
    public PluginConfig(string pluginName, IEnumerable<RuleConfig> rules, bool enabled)
    {
        PluginName = pluginName;
        Rules = rules;
        Enabled = enabled;
    }

}