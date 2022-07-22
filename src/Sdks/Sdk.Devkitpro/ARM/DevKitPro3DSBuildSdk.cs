using Brewery.ToolSdk.Project;

namespace Brewery.Sdk.DevKitPro.ARM;

internal class DevKitPro3DSBuildSdk : DevKitProARMBuildSdk
{
    public const string SdkName = "DevkitPro3DS";

    public override string Name => SdkName;

    protected override void AddSpecsFlag(GameProject project, DevKitProBuildSdkProjectSettings settings, List<string> flags)
    {
        flags.Add("-specs=3dsx.specs");
    }

    protected override void AddCpuAndTuneFlags(GameProject project, DevKitProBuildSdkProjectSettings settings, List<string> flags)
    {
        flags.Add("-march=armv6k");
        flags.Add("-mtune=mpcore");
        flags.Add("-mfloat-abi=hard");
        flags.Add("-mtp=soft");
    }
}