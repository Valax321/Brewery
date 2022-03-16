using Brewery.ToolSdk.Settings;

namespace Brewery.ProjectTool.Settings;

internal class Setting
{ }

internal class Setting<T> : Setting, ISetting<T>
{
    private T m_value;
    private T m_defaultValue;

    public Setting(T defaultValue, T value)
    {
        m_defaultValue = defaultValue;
        m_value = value;
    }

    public T GetValue()
    {
        return m_value;
    }

    public void SetValue(T value)
    {
        m_value = value;
    }

    public override string ToString()
    {
        return m_value?.ToString() ?? string.Empty;
    }
}