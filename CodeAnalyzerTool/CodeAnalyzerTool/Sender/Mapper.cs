using CodeAnalyzerTool.API;
using CodeAnalyzerTool.Sender.DTO;

namespace CodeAnalyzerTool.Sender;

internal static class Mapper
{
    internal static ProjectAnalysisDto MapToDto(this ProjectAnalysis pa)
    {
        return new ProjectAnalysisDto(
            projectName: pa.ProjectName,
            repoUrl: pa.RepoUrl,
            ruleViolations: pa.RuleViolations.Select(rv => rv.MapToDto())
        );
    }

    private static RuleViolationDto MapToDto(this RuleViolation rv)
    {
        return new RuleViolationDto(
            rule: rv.Rule.MapToDto(),
            message: rv.Message,
            location: rv.Location.MapToDto(),
            severity: rv.Severity.ToString()
        );
    }

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