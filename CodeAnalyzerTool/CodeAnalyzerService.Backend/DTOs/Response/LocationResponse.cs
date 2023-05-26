using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DTOs.Response;

public class LocationResponse
{
    [Required] public string Path { get; set; }
    [Range(0, int.MaxValue)] public int StartLine { get; set; }
    [Range(0, int.MaxValue)] public int EndLine { get; set; }
    [Required] public string FileExtension { get; set; }
    
    public LocationResponse(string path, int startLine, int endLine, string fileExtension)
    {
        Path = path;
        StartLine = startLine;
        EndLine = endLine;
        FileExtension = fileExtension;
    }
}