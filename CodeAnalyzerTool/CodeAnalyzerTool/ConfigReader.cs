using CodeAnalyzerTool.Api.ConfigModel;
using CodeAnalyzerTool.Api.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Serilog;

namespace CodeAnalyzerTool;

public class ConfigReader
{
    public async Task<GlobalConfig> ReadAsync()
    {
        var workingDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(workingDir, StringResources.CONFIG_FILE_NAME);
        var schemaPath = Path.Combine(workingDir, StringResources.SCHEMA_FILE_NAME);

        var schema = JSchema.Parse(await File.ReadAllTextAsync(schemaPath));
        var config = JObject.Parse(await File.ReadAllTextAsync(configPath));

        var isValid = config.IsValid(schema, out IList<string> errorMessages);
        if (isValid) return config.ToObject<GlobalConfig>() ?? 
                            throw new ConfigException(StringResources.INSTANTIATE_CONFIG_MESSAGE);
        foreach (var errorMessage in errorMessages)
        {
            Log.Error("{ErrorMessage}",errorMessage);
        }
        throw new ConfigException(StringResources.INCORRECT_CONFIG_MESSAGE);
    }
}