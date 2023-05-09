namespace CAT_API; 

public class Location {
    public int Id { get; set; }
    public string Path { get; set; }
    public int StartLine { get; set; }
    public int EndLine { get; set; }
    public string FileExtension { get; set; }

    public Location(string path, int startLine, int endLine, string fileExtension)
    {
        Path = path;
        StartLine = startLine;
        EndLine = endLine;
        FileExtension = fileExtension;
    }
}