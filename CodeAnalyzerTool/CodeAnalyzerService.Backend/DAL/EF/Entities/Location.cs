using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DAL.EF.Entities;

public class Location
{
    public int Id { get; set; }
    [Required] public string Path { get; set; }
    [Range(0, int.MaxValue)] public int StartLine { get; set; }
    [Range(0, int.MaxValue)] public int EndLine { get; set; }
    [Required] public string FileExtension { get; set; }

    public Location(string path, int startLine, int endLine, string fileExtension)
    {
        Path = path;
        StartLine = startLine;
        EndLine = endLine;
        FileExtension = fileExtension;
    }

    public override string ToString()
    {
        return $"In file: {Path}, at line: {StartLine}";
    }
}