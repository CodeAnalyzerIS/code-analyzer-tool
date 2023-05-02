using Newtonsoft.Json;

namespace CodeAnalyzerTool.ConfigModel;

public class PluginConfig
{
    [JsonProperty("pluginName", Required = Required.Always)]
    public string PluginName { get; set; }
    
    [JsonProperty("rules", Required = Required.Always)]
    public IEnumerable<RuleConfig> Rules { get; set; }
    
    public PluginConfig(string pluginName, IEnumerable<RuleConfig> rules)
    {
        PluginName = pluginName;
        Rules = rules;
    }

}