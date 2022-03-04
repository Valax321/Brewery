using Brewery.Sdk.DevKitPro.BuildTasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.DevKitPro.ARM;

internal class DevKitProGBABuildSdk : DevKitProARMBuildSdk
{
    public const string SdkName = "DevkitProGBA";

    public override string Name => SdkName;

    protected override (string?, string?) GetLibraryRedirectedName(string libraryName)
    {
        return libraryName switch
        {
            "maxmod" => ("gba", "mm"),
            _ => (null, null)
        };
    }

    protected override IBuildTask GetPostBuildBinaryTask(GameProject project, FileInfo elfFile)
    {
        return GBAFixTask.Generate(project, elfFile, out _);
    }
}