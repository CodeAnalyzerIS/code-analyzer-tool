﻿using System.ComponentModel.DataAnnotations;

namespace CodeAnalyzerService.Backend.DTOs.Request;

public class LocationRequest
{
    [Required] public string Path { get; set; }
    [Range(0, int.MaxValue)] public int StartLine { get; set; }
    [Range(0, int.MaxValue)] public int EndLine { get; set; }
    [Required] public string FileExtension { get; set; }
    
    public LocationRequest(string path, int startLine, int endLine, string fileExtension)
    {
        Path = path;
        StartLine = startLine;
        EndLine = endLine;
        FileExtension = fileExtension;
    }
}