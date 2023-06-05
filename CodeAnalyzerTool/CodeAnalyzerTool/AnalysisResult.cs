using CodeAnalyzerTool.API;

namespace CodeAnalyzerTool;

public class AnalysisResult
{
    public StatusCode StatusCode { get; set; }
    public IEnumerable<RuleViolation> RuleViolations { get; set; }

    public AnalysisResult(StatusCode statusCode, IEnumerable<RuleViolation> ruleViolations)
    {
        StatusCode = statusCode;
        RuleViolations = ruleViolations;
    }
}