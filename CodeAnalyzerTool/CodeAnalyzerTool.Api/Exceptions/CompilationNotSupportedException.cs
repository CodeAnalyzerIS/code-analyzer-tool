namespace CodeAnalyzerTool.Api.Exceptions;

[Serializable]
public class CompilationNotSupportedException : Exception
{
    public CompilationNotSupportedException()
    {
    }

    public CompilationNotSupportedException(string message) : base(message)
    {
    }
}