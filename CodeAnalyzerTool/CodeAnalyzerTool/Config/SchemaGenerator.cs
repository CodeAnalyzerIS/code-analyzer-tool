using CodeAnalyzerTool.API.ConfigModel;
using Newtonsoft.Json.Schema.Generation;

namespace CodeAnalyzerTool.Config;

internal static class SchemaGenerator
{
    public static async Task GenerateSchema()
    {
        var generator = new JSchemaGenerator();
        generator.GenerationProviders.Add(new StringEnumGenerationProvider());
        var schema = generator.Generate(typeof(GlobalConfig));
        await File.WriteAllTextAsync(StringResources.SCHEMA_FILE_NAME, schema.ToString());
    }
}