namespace Brewery.ProjectTool.Utility;

internal static class ProjectHelper
{
    public static string ResolveProjectPath(string projectPath, string currentDirectory)
    {
        if (Path.IsPathRooted(projectPath))
            return projectPath;

        return Path.GetFullPath(projectPath, currentDirectory);
    }
}