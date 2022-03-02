namespace Brewery.ToolSdk.Project;

/// <summary>
/// Exception thrown when an error occurs loading a <see cref="GameProject"/>.
/// </summary>
public class GameProjectReadException : Exception
{
    internal GameProjectReadException(string message) : base(message) 
    { }

    internal GameProjectReadException(string message, Exception innerException) : base(message, innerException)
    { }
}