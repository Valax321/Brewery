using System.Drawing;
using Brewery.ProjectTool;
using Console = Colorful.Console;

#if !DEBUG
try {
#endif
ToolHost.Create(args).Run();
#if !DEBUG
} catch (Exception ex)
{
    Console.WriteLine("The tool encountered an unhandled exception:", Color.Red);
    Console.WriteLine(ex.ToString(), Color.Red);
}
#endif