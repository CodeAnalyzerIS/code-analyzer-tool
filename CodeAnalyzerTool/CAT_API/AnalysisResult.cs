namespace CAT_API;


public class AnalysisResult {
    public string RuleId { get; set; }
    public Location location { get; set; }
    public string PluginId { get; set; }
    // What (programming) language the analyzer is analysing (i.e. c#)
    public string AnalysisTarget { get; set; }
}