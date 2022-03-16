using System.ComponentModel;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Settings;
using Tommy;

namespace Brewery.ProjectTool.Settings;

internal class EnvironmentSettingsRegistry : IEnvironmentSettings
{
    private const string SettingsDirectoryName = ".brewery";

#if SHIPPING
    private const string SettingsFileName = "environmentsettings.toml";
#else
    private const string SettingsFileName = "environmentsettings.development.toml";
#endif

    private readonly ILogger<EnvironmentSettingsRegistry> m_logger;

    private readonly Dictionary<string, Dictionary<string, Setting>> m_settingsTable = new();
    private readonly TomlTable m_savedSettings = new();

    public EnvironmentSettingsRegistry(ILogger<EnvironmentSettingsRegistry> logger)
    {
        m_logger = logger;

        try
        {
            var file = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                SettingsDirectoryName, SettingsFileName));

            if (file.Exists)
            {
                m_logger.Debug($"Loading environment settings from {file.FullName}");
                using var reader = file.OpenText();
                m_savedSettings = TOML.Parse(reader);
            }
            else
            {
                m_logger.Debug($"Environment settings do not exist, creating empty file at {file.FullName}");
                file.Directory?.Create();
                using var _ = file.CreateText();
                _.WriteLine();
            }
        }
        catch (TomlParseException ex)
        {
            m_logger.Error("Syntax error(s) encountered when parsing Brewery environment settings:");
            foreach (var error in ex.SyntaxErrors)
            {
                m_logger.Error($"  {error.Line}:{error.Column} {error.Message}");
            }
        }
        catch (Exception ex)
        {
            m_logger.Error($"Failed to load Brewery environment settings: {ex.Message}");
        }
    }

    public void SaveSettings()
    {
        try
        {
            var file = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                SettingsDirectoryName, SettingsFileName));

            file.Directory?.Create();

            using var writer = file.CreateText();

            var table = new TomlTable();
            foreach (var (namespaceName, keys) in m_settingsTable)
            {
                foreach (var (keyName, value) in keys)
                {
                    table[namespaceName][keyName] = value.ToString();
                }
            }

            table.WriteTo(writer);
        }
        catch (Exception ex)
        {
            m_logger.Error($"Failed to save environment settings: {ex.Message}");
        }
    }

    public IEnvironmentSettings RegisterSetting<T>(string settingNamespace, string settingKey, T defaultValue)
    {
        if (!m_settingsTable.TryGetValue(settingNamespace, out var keys))
        {
            keys = new Dictionary<string, Setting>();
            m_settingsTable.Add(settingNamespace, keys);
        }

        if (keys.ContainsKey(settingKey))
            throw new DuplicateSettingException(settingNamespace, settingKey);

        var value = defaultValue;
        if (m_savedSettings.TryGetNode(settingNamespace, out var namespaceNode))
        {
            if (namespaceNode.TryGetNode(settingKey, out var keyNode))
            {
                if (typeof(IConvertible).IsAssignableFrom(typeof(T)))
                {
                    value = (T)Convert.ChangeType(keyNode.AsString, typeof(T));
                }
                else
                {
                    var converter = TypeDescriptor.GetConverter(typeof(T));
                    var x = converter.ConvertFromString(keyNode.AsString);
                    if (x != null)
                        value = (T)x;
                }
            }
        }

        keys.Add(settingKey, new Setting<T>(defaultValue, value));

        return this;
    }

    public IReadOnlySetting<T>? GetSetting<T>(string settingNamespace, string settingKey)
    {
        return GetSettingMutable<T>(settingNamespace, settingKey);
    }

    public ISetting<T>? GetSettingMutable<T>(string settingNamespace, string settingKey)
    {
        if (m_settingsTable.TryGetValue(settingNamespace, out var keys))
        {
            if (keys.TryGetValue(settingKey, out var setting))
                return setting as ISetting<T>;
        }

        return null;
    }
}