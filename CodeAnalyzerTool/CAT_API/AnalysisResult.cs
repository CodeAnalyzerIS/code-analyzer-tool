namespace CAT_API;

public class AnalysisResult
{
    public Rule Rule { get; set; }
    public string PluginId { get; set; }
    public string Message { get; set; }
    // What (programming) language the analyzer is analysing (i.e. c#, markdown, etc.)
    public string TargetLanguage { get; set; }
    public Location Location { get; set; }
    public Severity Severity { get; set; }
}