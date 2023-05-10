using CAT_API;
using Serilog.Events;

namespace CodeAnalyzerTool.util;

public static class LogHelper
{
    public static LogEventLevel SeverityToLogLevel(Severity severity)
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