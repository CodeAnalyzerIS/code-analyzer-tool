using Newtonsoft.Json;

namespace CodeAnalyzerTool.Api.ConfigModel;

public class RuleConfig
{
    [JsonProperty("ruleName", Required = Required.Always)]
    public string RuleName { get; set; }
    
    [JsonProperty("enabled", Required = Required.Always)]
    public bool Enabled { get; set; }
    
    [JsonProperty("severity", Required = Required.Always)]
    public Severity Severity { get; set; }

    [JsonProperty("options", Required = Required.Default)]
    public Dictionary<string, string> Options { get; set; }

    public RuleConfig(string ruleName, bool enabled, Dictionary<string, string> options)
    {
        RuleName = ruleName;
        Enabled = enabled;
        Options = options;
    }
}