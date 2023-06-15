using CodeAnalyzerTool.API;

namespace CodeAnalyzerTool;

/// <summary>
/// Represents the end-result of an analysis.
/// </summary>
public class AnalysisResult
{
    /// <summary>
    /// The <c>StatusCode</c> of the <c>AnalysisResult</c> represents the type of outcome the analysis has produced.
    /// </summary>
    public StatusCode StatusCode { get; set; }
    public IEnumerable<RuleViolation> RuleViolations { get; set; }

    /// <summary>
    /// Initializes a new instance of the AnalysisResult class.
    /// </summary>
    /// <param name="statusCode">The status code of the analysis result.</param>
    /// <param name="ruleViolations">The group of rule violations.</param>

    public AnalysisResult(StatusCode statusCode, IEnumerable<RuleViolation> ruleViolations)
    {
        StatusCode = statusCode;
        RuleViolations = ruleViolations;
    }
}