using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;

namespace Brewery.GBAPluginExample
{
    internal class StringsCompilerTask : IBuildTask
    {
        public Action<string, LogLevel> Log { get; set; }

        public static StringsCompilerTask Generate(GameProject project, string stringsFile, out FileInfo sourceFile)
        {
            sourceFile = null;
            return new StringsCompilerTask();
        }

        public BuildResult Build()
        {
            Log("Processing strings file", LogLevel.Information);

            return BuildResult.Succeeded;
        }
    }
}
