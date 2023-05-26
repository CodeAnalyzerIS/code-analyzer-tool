namespace CodeAnalyzerTool.API.Exceptions;

[Serializable]
public class ConfigException : Exception
{
    public ConfigException()
    {
    }

    public ConfigException(string message) : base(message)
    {
    }
}