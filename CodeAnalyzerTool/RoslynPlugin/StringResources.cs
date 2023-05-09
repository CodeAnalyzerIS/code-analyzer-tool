namespace RoslynPlugin; 

public static class StringResources
{
    public const string PluginId = "Roslyn"; 
    public const string FileExtension = ".cs";
    public const string TargetLanguage = "C#";
    public const string RulesFolderName = "rules";
    public const string ExternalRuleSearchPattern = "*.dll";
    public const string SolutionSearchPattern = "*.sln";
    public const string NullCompilationMsg = "Compilation was null";
    public const string NoNameSpaceOptionMsg = "No namespace option has been provided";
}