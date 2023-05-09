namespace CAT_API; 

public class Location {
    public int Id { get; set; }
    // Normally only possible to create AnalysisResult instance with public constructor so Path, FileExtension cannot be null
    public string Path { get; set; } = null!;
    public int StartLine { get; set; }
    public int EndLine { get; set; }
    public string FileExtension { get; set; } = null!;
    
    // Only meant for Entity Framework
    // ReSharper disable once UnusedMember.Local
    private Location() {}

    public Location(string path, int startLine, int endLine, string fileExtension)
    {
        Path = path;
        StartLine = startLine;
        EndLine = endLine;
        FileExtension = fileExtension;
    }
}