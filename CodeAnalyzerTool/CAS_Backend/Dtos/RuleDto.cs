using System.ComponentModel.DataAnnotations;
using CAT_API;

namespace CAS_Backend.Dtos;

public class RuleDto
{
    [Required] public string Id { get; set; }
    [Required] public string Title { get; set; }
    [Required] [MinLength(10)] public string Description { get; set; }
    [Required] public string Category { get; set; }
    public bool IsEnabledByDefault { get; set; }
    public string DefaultSeverity { get; set; }
    
    public RuleDto(string id, string title, string description, string category, bool isEnabledByDefault,
        string defaultSeverity)
    {
        Id = id;
        Title = title;
        Description = description;
        Category = category;
        IsEnabledByDefault = isEnabledByDefault;
        DefaultSeverity = defaultSeverity;
    }
}