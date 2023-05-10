using CAT_API;
using Serilog;
using Serilog.Events;

namespace CodeAnalyzerTool.util;

public static class LogHelper
{
    public static void LogAnalysisResults(List<AnalysisResult> results)
    {
        if (results.Count == 0) Log.Information("No rule violations found (no analysis results)");
        else
            results.ForEach(r =>
                Log.Write(SeverityToLogLevel(r.Severity),
                    "[{Category}]({PluginId}, {RuleId}) {Message} | Path: {Path}",
                    r.Rule.Category, r.PluginId, r.Rule.Id, r.Message, r.Location.Path));
    }
    private static LogEventLevel SeverityToLogLevel(Severity severity)
    {
        return severity switch
        {
            Severity.Info => LogEventLevel.Information,
            Severity.Warning => LogEventLevel.Warning,
            Severity.Error => LogEventLevel.Error,
            _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, @"Cannot log unknown Severity")
        };
    }
}