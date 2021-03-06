using System.Xml.Linq;
using Brewery.ToolSdk.Project;

namespace Brewery.ToolSdk.Sdk;

/// <summary>
/// Interface for an SDK providing compiler and build tools for a project.
/// </summary>
public interface IBuildSdk
{
    /// <summary>
    /// The identifier for this SDK,
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Method called at sdk creation time to get services it requires.
    /// </summary>
    /// <param name="services"></param>
    void Initialize(IServiceProvider services);

    /// <summary>
    /// Creates the <see cref="IBuildSdkProjectSettings"/> for this SDK.
    /// </summary>
    /// <returns>SDK-specific settings object, or null if not needed.</returns>
    IBuildSdkProjectSettings? CreateSdkSettings();

    /// <summary>
    /// Implementation-specific <see cref="GameProject"/> reading.
    /// Settings read should be written into an object implementing <see cref="IBuildSdkProjectSettings"/>
    /// so that it can be stored on the <see cref="GameProject"/> instance.
    /// </summary>
    /// <param name="rootElement">The root XML element of the project file.</param>
    /// <param name="settings">Settings object to read into.</param>
    /// <param name="isConfiguration">Are we reading from the root xml element, or a custom configuration block?</param>
    void ReadSdkSettings(XElement rootElement, IBuildSdkProjectSettings? settings, bool isConfiguration);

    /// <summary>
    /// Performs a build of the given <see cref="GameProject"/>.
    /// </summary>
    /// <param name="project">The project to build.</param>
    /// <returns>The result of the build.</returns>
    BuildResult PerformBuild(GameProject project);
}