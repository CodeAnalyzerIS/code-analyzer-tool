using CodeAnalyzerTool.Api;
using Serilog;
using Serilog.Events;

namespace CodeAnalyzerTool.util;

internal static class LogHelper
{
    public static void InitLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }

    public static void LogAnalysisResults(IEnumerable<RuleViolation> results)
    {
        var res = results.ToList();
        if (!res.Any())
        {
            Log.Information("No rule violations found");
            return;
        }

        foreach (var r in res)
        {
            Log.Write(SeverityToLogLevel(r.Severity),
                "[{Category}]({PluginId}, {RuleId}) {Message} | Path: {Path} at line: {Line}",
                r.Rule.Category, r.PluginId, r.Rule.RuleName, r.Message, r.Location.Path, r.Location.StartLine);
        }
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