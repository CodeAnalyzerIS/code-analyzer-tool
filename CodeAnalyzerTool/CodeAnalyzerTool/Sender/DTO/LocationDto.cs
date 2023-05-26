using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerTool.Sender.DTO;

public class LocationDto
{
    [Required] public string Path { get; set; }
    [Range(0, int.MaxValue)] public int StartLine { get; set; }
    [Range(0, int.MaxValue)] public int EndLine { get; set; }
    [Required] public string FileExtension { get; set; }

    public LocationDto(string path, int startLine, int endLine, string fileExtension)
    {
        Path = path;
        StartLine = startLine;
        EndLine = endLine;
        FileExtension = fileExtension;
    }
}