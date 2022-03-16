namespace Brewery.ToolSdk.Settings
{
    /// <summary>
    /// Exception thrown when two settings with the same namespace and key are registered to one <see cref="IEnvironmentSettings"/>.
    /// </summary>
    public class DuplicateSettingException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="DuplicateSettingException"/>.
        /// </summary>
        /// <param name="settingNamespace">The namespace of the offending setting.</param>
        /// <param name="settingKey">The key of the offending setting.</param>
        public DuplicateSettingException(string settingNamespace, string settingKey) : base(
            $"A setting named {settingNamespace}.{settingKey} already exists")
        { }
    }
}
