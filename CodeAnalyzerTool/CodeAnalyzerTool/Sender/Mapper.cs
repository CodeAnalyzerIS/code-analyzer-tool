using CodeAnalyzerTool.API;
using CodeAnalyzerTool.Sender.DTO;

namespace CodeAnalyzerTool.Sender;

/// <summary>
/// Static class providing mapping extension methods for converting domain objects to DTOs.
/// </summary>
internal static class Mapper
{
    /// <summary>
    /// Maps a <see cref="ProjectAnalysis"/> object to a <see cref="ProjectAnalysisDto"/>.
    /// </summary>
    /// <param name="pa">The <see cref="ProjectAnalysis"/> object to map.</param>
    /// <returns>The mapped <see cref="ProjectAnalysisDto"/>.</returns>
    internal static ProjectAnalysisDto MapToDto(this ProjectAnalysis pa)
    {
        return new ProjectAnalysisDto(
            projectName: pa.ProjectName,
            repoUrl: pa.RepoUrl,
            ruleViolations: pa.RuleViolations.Select(rv => rv.MapToDto())
        );
    }

    /// <summary>
    /// Maps a <see cref="RuleViolation"/> object to a <see cref="RuleViolationDto"/>.
    /// </summary>
    /// <param name="rv">The <see cref="RuleViolation"/> object to map.</param>
    /// <returns>The mapped <see cref="RuleViolationDto"/>.</returns>
    private static RuleViolationDto MapToDto(this RuleViolation rv)
    {
        return new RuleViolationDto(
            rule: rv.Rule.MapToDto(),
            message: rv.Message,
            location: rv.Location.MapToDto(),
            severity: rv.Severity.ToString()
        );
    }

    /// <summary>
    /// Maps a <see cref="Rule"/> object to a <see cref="RuleDto"/>.
    /// </summary>
    /// <param name="rule">The <see cref="Rule"/> object to map.</param>
    /// <returns>The mapped <see cref="RuleDto"/>.</returns>
    private static RuleDto MapToDto(this Rule rule)
    {
        return new RuleDto(
            ruleName: rule.RuleName,
            title: rule.Title,
            description: rule.Description,
            category: rule.Category,
            pluginName: rule.PluginName,
            targetLanguage: rule.TargetLanguage,
            isEnabledByDefault: rule.IsEnabledByDefault,
            defaultSeverity: rule.DefaultSeverity.ToString(),
            codeExample: rule.CodeExample,
            codeExampleFix: rule.CodeExampleFix
        );
    }

    /// <summary>
    /// Maps a <see cref="Location"/> object to a <see cref="LocationDto"/>.
    /// </summary>
    /// <param name="location">The <see cref="Location"/> object to map.</param>
    /// <returns>The mapped <see cref="LocationDto"/>.</returns>
    private static LocationDto MapToDto(this Location location)
    {
        return new LocationDto(
            path: location.Path,
            startLine: location.StartLine,
            endLine: location.EndLine,
            fileExtension: location.FileExtension
        );
    }
}