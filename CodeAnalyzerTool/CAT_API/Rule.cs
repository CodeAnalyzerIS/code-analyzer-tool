using System.ComponentModel.DataAnnotations;

namespace CAT_API;

public class Rule
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; }
    public bool IsEnabledByDefault { get; set; }
    public Severity DefaultSeverity { get; set; }
}