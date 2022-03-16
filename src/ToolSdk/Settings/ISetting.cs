namespace Brewery.ToolSdk.Settings;

/// <summary>
/// Interface for entries in <see cref="IEnvironmentSettings"/>.
/// </summary>
/// <typeparam name="T">The type of the setting.</typeparam>
public interface IReadOnlySetting<out T>
{
    /// <summary>
    /// Gets the value of this setting.
    /// </summary>
    T GetValue();
}

/// <inheritdoc />
public interface ISetting<T> : IReadOnlySetting<T>
{
    /// <summary>
    /// Sets the value of this setting.
    /// </summary>
    /// <param name="value">The new value tos set.</param>
    void SetValue(T value);
}