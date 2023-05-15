namespace CodeAnalyzerTool.Api.Exceptions;

[Serializable]
public class ConfigException : Exception
{
    public ConfigException() : base()
    {
    }

    public ConfigException(string message) : base(message)
    {
    }
}