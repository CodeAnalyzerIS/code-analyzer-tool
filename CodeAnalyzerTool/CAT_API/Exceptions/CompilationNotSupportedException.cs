namespace CAT_API.Exceptions;

public class CompilationNotSupportedException : Exception
{
    public CompilationNotSupportedException()
    {
    }

    public CompilationNotSupportedException(string message) : base(message)
    {
    }
}