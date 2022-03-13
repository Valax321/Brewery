using System.Text;
using Brewery.ToolSdk.Sdk;

namespace Brewery.Sdk.DevKitPro;

/// <summary>
/// DevKitPro-specific SDK project settings.
/// </summary>
public class DevKitProBuildSdkProjectSettings : IBuildSdkProjectSettings
{
    /// <summary>
    /// System library that should be linked against.
    /// Some platforms have several options, such as the GBA having tonc and libgba.
    /// </summary>
    public string SystemLib { get; set; } = string.Empty;

    /// <summary>
    /// Additional libraries that should be linked. The DevKitPro SDK root path is searched for these.
    /// </summary>
    public List<string> AdditionalLibs { get; set; } = new();

    /// <summary>
    /// Paths to search for additional libraries.
    /// </summary>
    public List<string> LibrarySearchPaths { get; set; } = new();

    /// <summary>
    /// The GCC optimization level to use when compiling.
    /// </summary>
    public GCCOptimizationLevel OptimizationLevel { get; set; } = GCCOptimizationLevel.O1;

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"System Library: {SystemLib}");
        sb.AppendLine($"Additional Libraries: {string.Join(", ", AdditionalLibs)}");
        sb.AppendLine($"Library Search paths:\n{string.Join('\n', LibrarySearchPaths)}");
        return sb.ToString();
    }
}