using Microsoft.CodeAnalysis.MSBuild;
using Serilog;

namespace RoslynPlugin;

//This class shows loading progress in the console
public class ProgressReporter : IProgress<ProjectLoadProgress> {
    public void Report(ProjectLoadProgress loadProgress) {
        var projectDisplay = Path.GetFileName(loadProgress.FilePath);
        if (loadProgress.TargetFramework != null) {
            projectDisplay += $" ({loadProgress.TargetFramework})";
        }
        var operation = $"{loadProgress.Operation,-15}";
        var elapsedTime = @$"{loadProgress.ElapsedTime,-15:m\:ss\.fffffff}";
        Log.Debug("{Operation} {ElapsedTime} {ProjectDisplay}", operation, elapsedTime, projectDisplay);
    }
}