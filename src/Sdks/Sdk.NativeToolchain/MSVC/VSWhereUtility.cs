using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Brewery.Sdk.NativeToolchain.MSVC;

internal static class VSWhereUtility
{
    private const string Executable = "vswhere.exe";

    public static IEnumerable<VSInstall> GetValidVisualStudioInstalls()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new InvalidOperationException("Operating system not supported. vswhere only runs on windows.");

        var appPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Executable);

        if (!File.Exists(appPath))
            throw new FileNotFoundException($"Could not find vswhere at {appPath}");

        var output = new StringBuilder();

        var process = new Process
        {
            StartInfo =
            {
                FileName = appPath,
                Arguments = BuildVSWhereInstallsArgs(),
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
            }
        };

        process.OutputDataReceived += (_, args) =>
        {
            output.AppendLine(args.Data);
        };

        process.Start();
        process.BeginOutputReadLine();

        process.WaitForExit();

        return ParseVSWhereInstalls(output.ToString());
    }

    private static IEnumerable<VSInstall> ParseVSWhereInstalls(string outputXml)
    {
        var doc = XDocument.Parse(outputXml);

        var installs = new List<VSInstall>();

        if (doc.Root?.Name == "instances")
        {
            installs.AddRange(doc.Root.Elements("instance")
                .Select(VSInstall.ParseFromXElement).Where(x => x.IsLaunchable));
            installs.Sort((a, b) => -a.InstallationVersion.CompareTo(b.InstallationVersion));
            return installs;
        }

        return Array.Empty<VSInstall>();
    }

    private static string BuildVSWhereInstallsArgs()
    {
        var args = new List<string>
        {
            "-nologo",
            "-nocolor",
            // Only include installs with MSVC
            "-requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64",
            "-format xml"
        };

        return string.Join(' ', args);
    }

    public static IEnumerable<WindowsSDKInstall> GetWindowsSDKInstalls()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            throw new InvalidOperationException("Operating system not supported. Can only locate Windows SDKs on Windows.");

        // TODO: is there a smarter way to look this path up?
        // Feels dodgy to rely on a hardcoded install directory
        // but I can't find any info online about registry/VS config file
        // that informs this path.
        var defaultInstallPath =
            $"C:\\Program Files{(RuntimeInformation.OSArchitecture == Architecture.X86 ? string.Empty : " (x86)")}\\Windows Kits\\10";

        if (!Directory.Exists(defaultInstallPath))
            return Array.Empty<WindowsSDKInstall>();

        var installs = new List<WindowsSDKInstall>();

        var installsLookupPath = new DirectoryInfo(Path.Combine(defaultInstallPath, "Lib"));
        foreach (var dir in installsLookupPath.EnumerateDirectories())
        {
            if (Version.TryParse(dir.Name, out var version))
            {
                var install = new WindowsSDKInstall
                {
                    Version = version,
                    InstallPath = defaultInstallPath
                };

                installs.Add(install);
            }
        }

        installs.Sort((a, b) => -a.Version.CompareTo(b.Version));
        return installs;
    }
}