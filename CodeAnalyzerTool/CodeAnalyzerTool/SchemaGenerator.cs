using CAT_API.ConfigModel;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;

namespace CodeAnalyzerTool;

public static class SchemaGenerator
{
    public static async Task GenerateSchema()
    {
        var generator = new JSchemaGenerator();
        generator.GenerationProviders.Add(new StringEnumGenerationProvider());
        var schema = generator.Generate(typeof(GlobalConfig));
        //TODO: Not hardcoded
        await File.WriteAllTextAsync("CATSchema.json", schema.ToString());
    }
}