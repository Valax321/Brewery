using System.Text;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Utility;

namespace Brewery.ToolSdk.Project;

/// <summary>
/// Code representation of the .brewproj for this project.
/// </summary>
public class GameProject
{
    /// <summary>
    /// Extension for game project files.
    /// </summary>
    public static string Extension => ".brewproj";

    public string Configuration { get; internal set; }

    /// <summary>
    /// Directory that the project is located at.
    /// </summary>
    public DirectoryInfo ProjectDirectory { get; }

    /// <summary>
    /// The directory that source code is located at.
    /// </summary>
    public DirectoryInfo SourceDirectory { get; internal set; }

    /// <summary>
    /// The directory that binary-packed assets are located.
    /// </summary>
    public DirectoryInfo AssetsDirectory { get; internal set; }

    /// <summary>
    /// The directory where intermediate build files are written.
    /// </summary>
    public DirectoryInfo IntermediateDirectory { get; internal set; }

    /// <summary>
    /// The SDK used for building this project.
    /// </summary>
    public IBuildSdk? BuildSdk { get; internal set; }

    /// <summary>
    /// <see cref="IBuildSdk"/> specific project settings.
    /// </summary>
    public IBuildSdkProjectSettings? BuildSdkProjectSettings { get; internal set; }

    /// <summary>
    /// Rules for building game assets.
    /// </summary>
    public List<AssetBuildRule> AssetRules { get; } = new();

    public IEnumerable<FileInfo> AssetBuildArtifacts { get; set; }

    /// <summary>
    /// Rules for building source code.
    /// </summary>
    public List<SourceBuildRule> SourceRules { get; } = new();

    public IEnumerable<FileInfo> SourceBuildArtifacts { get; set; }

    public List<string> DefineSymbols { get; set; } = new();

    /// <summary>
    /// The name of the output file.
    /// </summary>
    public string OutputName { get; internal set; }

    /// <summary>
    /// Reads a <see cref="GameProject"/> from a .brewproj file.
    /// </summary>
    /// <param name="projectFile">The project file.</param>
    /// <param name="services">Service provider for accessing tool services.</param>
    /// <returns>A <see cref="GameProject"/> instance representing the .brewproj content.</returns>
    /// <exception cref="GameProjectReadException"></exception>
    public static GameProject Read(FileInfo projectFile, IServiceProvider services, string configuration)
    {
        return GameProjectReader.ReadProject(projectFile, services, configuration);
    }

    internal GameProject(DirectoryInfo projectDirectory)
    {
        ProjectDirectory = projectDirectory;
        SourceDirectory = projectDirectory.GetSubDirectory("source");
        AssetsDirectory = projectDirectory.GetSubDirectory("assets");
        IntermediateDirectory = projectDirectory.GetSubDirectory("obj");
    }

    /// <summary>
    /// Perform a build of the project.
    /// </summary>
    /// <returns>The result of the build.</returns>
    public BuildResult Build()
    {
        if (BuildSdk is null)
            throw new InvalidOperationException("No BuildSdk available for performing the build.");

        return BuildSdk.PerformBuild(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Project Directory: {ProjectDirectory.FullName}");
        sb.AppendLine($"Source Directory: {SourceDirectory.FullName}");
        sb.AppendLine($"Assets Directory: {AssetsDirectory.FullName}");
        sb.AppendLine($"SDK: {BuildSdk?.Name}");
        if (BuildSdkProjectSettings is not null)
            sb.Append(BuildSdkProjectSettings);
        return sb.ToString();
    }
}
