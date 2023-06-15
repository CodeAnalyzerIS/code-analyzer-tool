namespace CodeAnalyzerTool;

/// <summary>
/// The status code of the represents the type of outcome the analysis has produced.
/// It illustrates whether any severe enough violations were found or not, in other words if the analyzed target codebase is approved or not.
/// However, it can also show if a fatal error has occured during analysis, meaning the analyzer tool itself crashed. 
/// </summary>
public enum StatusCode
{
    /// <summary>
    /// The analysis was successful, and no severe violations were found in the code.
    /// </summary>
    Success = 0,
    /// <summary>
    /// The analysis was not successful because severe violations were found in the code.
    /// </summary>
    Failed = 1,
    /// <summary>
    /// A fatal error occurred during analysis, leading to the crash of the analyzer tool.
    /// </summary>
    FatalError = 2
}