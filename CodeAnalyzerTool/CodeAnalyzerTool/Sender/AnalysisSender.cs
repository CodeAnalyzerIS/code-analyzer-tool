using System.Net.Http.Json;
using CodeAnalyzerTool.API;
using CodeAnalyzerTool.API.ConfigModel;
using Serilog;

namespace CodeAnalyzerTool.Sender;

/// <summary>
/// Class responsible for sending analysis results to the backend API.
/// </summary>
public class AnalysisSender
{
    private readonly HttpClient _httpClient;
    private readonly string _projectName;
    private readonly Uri _endpointUrl;
    private readonly string _repoUrl;

    public AnalysisSender(HttpClient httpClient, GlobalConfig globalConfig)
    {
        _httpClient = httpClient;
        _projectName = globalConfig.ProjectName;
        _repoUrl = globalConfig.RepoUrl;
        var url = globalConfig.ApiUrl!;
        // todo make clear in documentation that including /api causes full URL to be used, and excluding it adds the default endpoint path to the url
        _endpointUrl = url.AbsoluteUri.ToLower().Contains("/api")
            ? url
            : new Uri(url, StringResources.PUT_ENDPOINT_PATH);
    }

    /// <summary>
    /// Sends the specified rule violations to the backend API.
    /// </summary>
    /// <param name="ruleViolations">The collection of rule violations to sent.</param>
    /// <returns>A void <c>Task</c> representing the asynchronous sending operation.</returns>
    internal async Task Send(IEnumerable<RuleViolation> ruleViolations)
    {
        var projectAnalysis = new ProjectAnalysis(_projectName, _repoUrl, ruleViolations).MapToDto();
        try
        {
            var response = await _httpClient.PutAsJsonAsync(_endpointUrl, projectAnalysis);
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