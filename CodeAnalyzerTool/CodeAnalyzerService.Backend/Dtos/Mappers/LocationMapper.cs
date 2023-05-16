using CodeAnalyzerService.Backend.DAL.EF.Entities;

namespace CodeAnalyzerService.Backend.Dtos.Mappers;

public class LocationMapper
{
    public static LocationDto MapToDto(Location location)
    {
        return new LocationDto(location.Path, location.StartLine, location.EndLine, location.FileExtension);
    }
    
    public static Location MapToModel(LocationDto locationDto)
    {
        return new Location(locationDto.Path, locationDto.StartLine, locationDto.EndLine, locationDto.FileExtension);
    }
}