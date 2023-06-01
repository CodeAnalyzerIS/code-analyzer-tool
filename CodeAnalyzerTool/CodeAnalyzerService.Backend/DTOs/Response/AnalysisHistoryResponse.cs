namespace CodeAnalyzerService.Backend.DTOs.Response;

public class AnalysisHistoryResponse
{
    public int Id { get; set; }
    public DateTime CreatedOn { get; set; }
    
    public AnalysisHistoryResponse()
    {
    }

    public AnalysisHistoryResponse(int id, DateTime createdOn)
    {
        Id = id;
        CreatedOn = createdOn;
    }
}