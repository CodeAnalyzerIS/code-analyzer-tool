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
        var jsonPath = Path.Combine(workingDir, "CATConfig.json");
        //TODO: Not hardcoded
        var schemaPath = Path.Combine(AppContext.BaseDirectory, "CATSchema.json");

        var schema = JSchema.Parse(await File.ReadAllTextAsync(schemaPath));
        var json = JObject.Parse(await File.ReadAllTextAsync(jsonPath));

        var isValid = json.IsValid(schema);
        if (!isValid)
            //TODO: Not hardcoded
            throw new JsonException("Config is not correctly formed according to the CATSchema.json");

        return json.ToObject<GlobalConfig>() ?? throw new JsonException("Null object was created");
    }
}