using CAT_API.ConfigModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CodeAnalyzerTool;

public static class ConfigReader
{
    public static async Task<GlobalConfig> ReadAsync()
    {
        var workingDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(workingDir, "CATConfig.json");
        //TODO: Not hardcoded
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "CATSchema.json");

        var schema = JSchema.Parse(await File.ReadAllTextAsync(schemaPath));
        var config = JObject.Parse(await File.ReadAllTextAsync(configPath));

        var isValid = config.IsValid(schema, out IList<string> errorMessages);
        if (isValid) return config.ToObject<GlobalConfig>() ?? throw new JsonException("Null object was created");
        foreach (var errorMessage in errorMessages)
        {
            Console.WriteLine(errorMessage);
        }
        //TODO: Not hardcoded
        throw new JsonException("Config is not correctly formed according to the CATSchema.json");

    }
}