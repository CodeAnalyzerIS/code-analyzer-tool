using CodeAnalyzerTool.ConfigModel;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace CodeAnalyzerTool;

public static class ConfigReader
{
    //TODO: Niet dynamic maken buiten voor rules?
    public static async Task<ToolConfig?> ReadAsync()
    {
        var workingDir = Directory.GetCurrentDirectory();
        var jsonPath = Path.Combine(workingDir, "CATConfig.json");
        Console.WriteLine($@"JsonPath: {jsonPath}");
        var jsonString = await File.ReadAllTextAsync(jsonPath);
        var result = JsonConvert.DeserializeObject<ToolConfig>(jsonString);
        // dynamic jsonObject = await JsonSerializer.DeserializeAsync<ToolConfig>(stream) ?? throw new InvalidOperationException();
        // return jsonObject;
        return result;
    }
}