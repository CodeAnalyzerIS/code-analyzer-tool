using System.Text.Json;

namespace CodeAnalyzerTool;

public static class ConfigReader
{
    //TODO: Niet dynamic maken buiten voor rules?
    public static async Task<object?> ReadAsync()
    {
        var workingDir = Directory.GetCurrentDirectory();
        var jsonPath = Path.Combine(workingDir, "CATConfig.json");
        Console.WriteLine($@"JsonPath: {jsonPath}");
        await using var stream = File.OpenRead(jsonPath);
        dynamic jsonObject = await JsonSerializer.DeserializeAsync<object>(stream) ?? throw new InvalidOperationException();
        return jsonObject;
    }
}