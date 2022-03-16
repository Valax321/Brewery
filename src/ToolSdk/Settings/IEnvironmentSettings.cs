namespace Brewery.ToolSdk.Settings;

/// <summary>
/// Environment-specific brewery settings.
/// </summary>
public interface IEnvironmentSettings
{
    /// <summary>
    /// Register a setting that can be saved.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="settingNamespace"></param>
    /// <param name="settingKey"></param>
    /// <param name="defaultValue"></param>
    public IEnvironmentSettings RegisterSetting<T>(string settingNamespace, string settingKey, T defaultValue);

    /// <summary>
    /// Gets a setting, given a namespace and settings key.
    /// </summary>
    /// <typeparam name="T">Type of setting to get.</typeparam>
    /// <param name="settingNamespace">The namespace for the setting.
    /// Usually the sdk/plugin that it belongs to.</param>
    /// <param name="settingKey">The name of the setting.</param>
    /// <returns>Settings object, or null if it is not found.</returns>
    IReadOnlySetting<T>? GetSetting<T>(string settingNamespace, string settingKey);

    /// <inheritdoc cref="GetSetting{T}"/>
    ISetting<T>? GetSettingMutable<T>(string settingNamespace, string settingKey);
}