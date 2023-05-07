using CAT_API.ConfigModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

namespace CodeAnalyzerTool;

public static class ConfigReader
{
    public static async Task<GlobalConfig> ReadAsync()
    {
        var workingDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(workingDir, StringResources.ConfigFileName);
        var schemaPath = Path.Combine(workingDir, StringResources.SchemaFileName);

        var schema = JSchema.Parse(await File.ReadAllTextAsync(schemaPath));
        var config = JObject.Parse(await File.ReadAllTextAsync(configPath));

        var isValid = config.IsValid(schema, out IList<string> errorMessages);
        if (isValid) return config.ToObject<GlobalConfig>() ?? throw new JsonException(StringResources.NullConfigMsg);
        foreach (var errorMessage in errorMessages)
        {
            Console.WriteLine(errorMessage);
        }
        throw new JsonException(StringResources.IncorrectConfigMsg);

    }
}