namespace RoslynPlugin.Exceptions;

public class UnexpectedSyntaxKindException : Exception
{
    public UnexpectedSyntaxKindException()
    {
    }

    public UnexpectedSyntaxKindException(string message) : base(message)
    {
    }
}