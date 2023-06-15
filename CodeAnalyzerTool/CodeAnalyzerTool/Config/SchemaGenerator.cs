using CodeAnalyzerTool.API.ConfigModel;
using Newtonsoft.Json.Schema.Generation;

namespace CodeAnalyzerTool.Config;


/// <summary>
/// The <c>SchemaGenerator</c> class generates a JSON schema for the <c>GlobalConfig</c> class.
/// </summary>
internal static class SchemaGenerator
{
    /// <summary>
    /// Generates a JSON schema for the <c>GlobalConfig</c> class.
    /// </summary>
    /// <returns>A void <c>Task</c> representing the asynchronous operation.</returns>
    public static async Task GenerateSchema()
    {
        var generator = new JSchemaGenerator();
        generator.GenerationProviders.Add(new StringEnumGenerationProvider());
        var schema = generator.Generate(typeof(GlobalConfig));
        await File.WriteAllTextAsync(StringResources.SCHEMA_FILE_NAME, schema.ToString());
    }
}