using CodeAnalyzerTool.API.ConfigModel;
using CodeAnalyzerTool.API.Exceptions;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using Serilog;

namespace CodeAnalyzerTool.Config;

internal class ConfigReader
{
    public async Task<GlobalConfig> ReadAsync()
    {
        Log.Information("Reading CAT Config file");
        var workingDir = Directory.GetCurrentDirectory();
        var configPath = Path.Combine(workingDir, StringResources.CONFIG_FILE_NAME);

        var generator = new JSchemaGenerator();
        generator.GenerationProviders.Add(new StringEnumGenerationProvider());
        var schema = generator.Generate(typeof(GlobalConfig));
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