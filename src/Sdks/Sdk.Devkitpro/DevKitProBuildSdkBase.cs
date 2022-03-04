﻿using System.Xml.Linq;
using Brewery.Sdk.DevKitPro.BuildTasks;
using Brewery.ToolSdk.Build;
using Brewery.ToolSdk.Logging;
using Brewery.ToolSdk.Project;
using Brewery.ToolSdk.Sdk;
using Brewery.ToolSdk.Utility;
using Brewery.ToolSdk.Xml;
using Microsoft.Extensions.DependencyInjection;

namespace Brewery.Sdk.DevKitPro;

/// <summary>
/// Base class for all the DevKitPro SDKs.
/// </summary>
public abstract class DevKitProBuildSdkBase : IBuildSdk
{
    /// <inheritdoc />
    public abstract string Name { get; }

    /// <summary>
    /// The directory the SDK's compiler is located in.
    /// </summary>
    public abstract string CompilerDirectory { get; }

    /// <summary>
    /// The GCC prefix for compiler executables.
    /// </summary>
    public abstract string CompilerPrefix { get; }

    /// <summary>
    /// The path DevKitPro is installed at.
    /// </summary>
    public DirectoryInfo? DevKitProPath { get; private set; }

    private ILogger<DevKitProBuildSdkBase> m_logger = null!;
    private IServiceProvider m_services = null!;

    /// <inheritdoc />
    public virtual void Initialize(IServiceProvider services)
    {
        m_logger = services.GetRequiredService<ILogger<DevKitProBuildSdkBase>>();
        m_services = services;

        var envPath = Environment.GetEnvironmentVariable("DEVKITPRO");
        if (envPath is not null && Path.IsPathRooted(envPath))
        {
            var validateDir = Path.Combine(envPath, CompilerDirectory);
            if (!Directory.Exists(validateDir))
            {
                m_logger.Error("Location specified does not seem to be a valid Devkitpro install");
            }

            DevKitProPath = new DirectoryInfo(envPath);
            m_logger.Debug($"Devkitpro install: {DevKitProPath.FullName}");
        }
        else
        {
            m_logger.Error("No Devkitpro install found. Is the DEVKITPRO environment variable set correctly?");
        }
    }

    /// <inheritdoc />
    public IBuildSdkProjectSettings? ReadSdkSettings(XElement rootElement)
    {
        var settings = InternalReadSdkSettings(rootElement);

        rootElement.ReadProperty<string>("SystemLib", value => settings.SystemLib = value)
            .ReadListProperty<string>("AdditionalLibs", "Lib", value => settings.AdditionalLibs.AddRange(value));

        return settings;
    }

    protected virtual DevKitProBuildSdkProjectSettings InternalReadSdkSettings(XElement rootElement) => new();

    /// <inheritdoc />
    public BuildResult PerformBuild(GameProject project)
    {
        var dispatcher = new BuildCommandDispatcher(m_services);

        dispatcher.RunParallel(project.GenerateAssetBuildTasks(out var assetBuildArtifacts));
        project.AssetBuildArtifacts = assetBuildArtifacts;

        // Here we need to actually the asset build before source compilation occurs
        // Otherwise the build artifacts don't actually exist and will be missed by the file matching
        // in the source build rules.
        var result = dispatcher.ExecuteTasks();
        if (result == BuildResult.Failed)
            return result;

        // Cursed concatenation of normal source files and generated .c and .s compiled asset files.
        dispatcher.RunParallel(project.GenerateSourceBuildTasks(out var sourceBuildArtifacts)
            .Concat(project.GenerateSourceBuildTasks(out var compiledAssetsBuildArtifacts, 
                project.IntermediateDirectory.GetSubDirectory("assets"))));
        project.SourceBuildArtifacts = sourceBuildArtifacts.Concat(compiledAssetsBuildArtifacts);

        dispatcher.RunTask(LinkTask.Generate(project, out var elfFile));
        var postBuildTask = GetPostBuildBinaryTask(project, elfFile);
        if (postBuildTask is not null)
            dispatcher.RunTask(postBuildTask);

        return dispatcher.ExecuteTasks();
    }

    /// <summary>
    /// Get commandline arguments for compiling source code files using this SDK.
    /// </summary>
    /// <param name="project">The project being compiled.</param>
    /// <param name="rule">Source rule being generated.</param>
    /// <param name="sourceFile">Path to the file being compiled.</param>
    /// <returns><see cref="CompileInfo"/> describing the compilation command.</returns>
    public abstract CompileInfo GetCompileCommand(GameProject project, SourceBuildRule rule, string sourceFile);

    /// <summary>
    /// Get commandline arguments for linking object files using this SDK.
    /// </summary>
    /// <param name="project">The project being compiled.</param>
    /// <param name="objectFiles">Object files that are being linked.</param>
    /// <returns><see cref="CompileInfo"/> describing the linker command.</returns>
    public abstract CompileInfo GetLinkCommand(GameProject project, IEnumerable<FileInfo> objectFiles);

    /// <summary>
    /// Gets a <see cref="IBuildSdk"/> task that should be executed after linking/binary generation is complete.
    /// </summary>
    /// <param name="project">The project being compiled.</param>
    /// <param name="elfFile"><see cref="FileInfo"/> describing the ELF file generated by the linker.</param>
    /// <returns></returns>
    protected virtual IBuildTask? GetPostBuildBinaryTask(GameProject project, FileInfo elfFile)
    {
        return null;
    }
}