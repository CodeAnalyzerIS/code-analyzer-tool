using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.DTOs.Request;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public static class LocationMapper
{
    public static LocationResponse MapToDto(this Location location)
    {
        return new LocationResponse(location.Path, location.StartLine, location.EndLine, location.FileExtension);
    }
    
    public static Location MapToModel(this LocationRequest locationRequest)
    {
        return new Location(locationRequest.Path, locationRequest.StartLine, locationRequest.EndLine, locationRequest.FileExtension);
    }
}