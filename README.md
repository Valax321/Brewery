# Brewery - A Homebrew Game Build Tool

[![Nuget](https://img.shields.io/nuget/v/Brewery.ProjectTool?label=Project%20Tool)](https://www.nuget.org/packages/Brewery.ProjectTool/)

[![Nuget](https://img.shields.io/nuget/v/Brewery.ToolSdk?label=Tool%20SDK)](https://www.nuget.org/packages/Brewery.ToolSdk/)

[![Nuget](https://img.shields.io/nuget/v/Brewery.Sdk.Devkitpro?label=DevKitPro%20SDK)](https://www.nuget.org/packages/Brewery.Sdk.Devkitpro/)

Brewery is a build/project manager for homebrew game development. At the moment it supports the DevKitARM toolchain of DevKitPro for GBA games and MSVC for Windows. It is written to be modular, so adding new toolchains is relatively easy.

Brewery is written in C# using .NET 6, and should be able to run on Windows, macOS and Linux.

## Features

Brewery is still in the early stages of development but is being used/actively developed for a project I am working on.

✅ = completed, 🧰 = in development, ❎ = not implemented but planned

* ✅ DevKitARM support for GBA, NDS and 3DS (so far only GBA is supported).
* ✅ Visual Studio C++ toolchain for Windows.
* ❎ GCC toolchain for MinGW, macOS and Linux.
* ✅ Asset Build Pipeline: Automate converting game assets into binaries at build time. Integrated into source code on embedded platforms with no filesystem.
* 🧰 Visual Studio Code C/C++ Project Generation: Generate a C/C++ settings file for VSCode for working code completion out of the box.
* ❎ Project Generation: Generate new projects from platform-specific templates to get started quickly.

## Project
The project layout is:
* `example` - Example projects using Brewery for various supported toolchains/platforms
* `src` - The source code for Brewery. Within this are subfolders for different C# projects used by Brewery.

## License

All source code in this project is available under the MIT license.