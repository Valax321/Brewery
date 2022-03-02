namespace Brewery.Sdk.DevKitPro;

public class CompileInfo
{
    public string OutputFile { get; set; } = default!;
    public IReadOnlyList<string> CompileCommand { get; set; } = default!;
}