﻿namespace Brewery.Sdk.DevKitPro;

/// <summary>
/// Information describing a compile/link command.
/// </summary>
public class CompileInfo
{
    /// <summary>
    /// The file generated by the command.
    /// </summary>
    public string OutputFile { get; set; } = default!;

    /// <summary>
    /// List of individual arguments to be parsed by the executed tool. Index 0 is the full path to the executable to run.
    /// </summary>
    public IReadOnlyList<string> CompileCommand { get; set; } = default!;
}