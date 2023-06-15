namespace CodeAnalyzerTool; 

/// <summary>
/// Provides various constant strings used throughout the application for configurability sake.
/// </summary>
internal static class StringResources
{
    public const string ROSLYN_PLUGIN_NAME = "Roslyn";
    public const string SCHEMA_FILE_NAME = "CATSchema.json";
    public const string CONFIG_FILE_NAME = "CATConfig.json";
    public const string INSTANTIATE_CONFIG_MESSAGE = "Config could not be instantiated as GlobalConfig";
    public const string INCORRECT_CONFIG_MESSAGE = $"Config is not valid according to the {SCHEMA_FILE_NAME}";
    public const string ASSEMBLY_NAME_MISSING_MESSAGE =
        "Invalid config: AssemblyName is a required field for external plugins.";
    public const string PUT_ENDPOINT_PATH = "/api/Project";
}