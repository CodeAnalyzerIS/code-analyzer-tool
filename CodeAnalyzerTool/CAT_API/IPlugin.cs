namespace CAT_API;

public interface IPlugin
{
    public Task<IEnumerable<AnalysisResult>> Analyze();
}