using CodeAnalyzerTool.API;
using Serilog;
using Serilog.Events;

namespace CodeAnalyzerTool.util;

/// <summary>
/// Helper class for logging analysis results and initializing the static logging system.
/// </summary>
internal static class LogHelper
{
    /// <summary>
    /// Initializes the static logging system for use throughout the application.
    /// </summary>
    public static void InitLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();
    }
    
    /// <summary>
    /// Logs the provided collection of rule violations.
    /// </summary>
    /// <param name="results">The collection of rule violations to log.</param>
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
                "[{Category}]({PluginName}, {RuleId}) {Message} | Path: {Path} at line: {Line}",
                r.Rule.Category, r.Rule.PluginName, r.Rule.RuleName, r.Message, r.Location.Path, r.Location.StartLine);
        }
    }

    /// <summary>
    /// Converts the <see cref="Severity"/> level to the corresponding Serilog log event level.
    /// </summary>
    /// <param name="severity">The severity level to convert.</param>
    /// <returns>The corresponding Serilog log event level.</returns>
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