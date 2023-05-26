using CodeAnalyzerService.Backend.DAL.EF.Entities;
using CodeAnalyzerService.Backend.DTOs.Request;
using CodeAnalyzerService.Backend.DTOs.Response;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class LocationMapper
{
    public static LocationResponse MapToDto(Location location)
    {
        return new LocationResponse(location.Path, location.StartLine, location.EndLine, location.FileExtension);
    }
    
    public static Location MapToModel(LocationRequest locationRequest)
    {
        return new Location(locationRequest.Path, locationRequest.StartLine, locationRequest.EndLine, locationRequest.FileExtension);
    }
}