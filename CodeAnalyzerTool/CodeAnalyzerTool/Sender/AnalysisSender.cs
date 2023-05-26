using System.Net.Http.Json;
using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;
using Serilog;

namespace CodeAnalyzerTool.Sender;

public class AnalysisSender
{
    private readonly HttpClient _httpClient;
    private readonly string _projectName;
    private readonly Uri _endpointUrl;

    public AnalysisSender(HttpClient httpClient, GlobalConfig globalConfig)
    {
        _httpClient = httpClient;
        _projectName = globalConfig.ProjectName;
        var url = globalConfig.ApiUrl;
        // todo make clear in documentation that including /api causes full URL to be used, and excluding it adds the default endpoint path to the url
        _endpointUrl = url.AbsoluteUri.ToLower().Contains("/api")
            ? url
            : new Uri(url, StringResources.PUT_ENDPOINT_PATH);
    }

    internal async Task Send(IEnumerable<RuleViolation> ruleViolations)
    {
        var projectAnalysis = new ProjectAnalysis(_projectName, ruleViolations).MapToDto();
        try
        {
            var response = await _httpClient.PutAsJsonAsync(_endpointUrl, projectAnalysis);
            // response.Content.ReadAsStringAsync()
            if (!response.IsSuccessStatusCode)
                Log.Error(
                    "Sending analysis results to backend failed with status code: {StatusCode} | URL: {EndpointUrl}",
                    response.StatusCode, _endpointUrl);
            else Log.Information("Successfully sent analysis results to backend (URL: {EndpointUrl}", _endpointUrl);
        }
        catch
        {
            Log.Error("Sending analysis results to backend FAILED (URL: {EndpointUrl}", _endpointUrl);
        }
    }
}