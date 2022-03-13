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

    protected override void AddLinkerArchFlags(GameProject project, DevKitProBuildSdkProjectSettings settings, List<string> flags)
    {
        flags.Add("-mthumb");
        flags.Add("-mthumb-interwork");
    }

    protected override void AddCpuAndTuneFlags(GameProject project, DevKitProBuildSdkProjectSettings settings, List<string> flags)
    {
        flags.Add("-mcpu=arm7tdmi");
        flags.Add("-mtune=arm7tdmi");
    }

    protected override void AddSpecsFlag(GameProject project, DevKitProBuildSdkProjectSettings settings, List<string> flags)
    {
        flags.Add("-specs=gba.specs");
    }

    protected override IBuildTask GetPostBuildBinaryTask(GameProject project, FileInfo elfFile)
    {
        return GBAFixTask.Generate(project, elfFile, out _);
    }
}