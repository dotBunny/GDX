# Assisting with Documentation
> [!IMPORTANT]
> A midlevel developer given `GDX` should be able to pick it up and use it effectively just by reading the inline documentation. 

A pretty ambitious goal; that should set the tone for what we are aiming to do when it comes to documentation.

## Local Generation
The documentation is statically generated using the [DocFx](https://dotnet.github.io/docfx/) project file located at `.docfx/docfx.json`. 

The scripts found in the `GDX.Documentation` repository are setup with assumed paths from an internal [dotbunny](https://dotbunny.com/) project. 
If you wish to utilize those scripts you will need to arrange your folders accordingly. 

> [!NOTE]
> The scripts extensively use the relative folder depth to path to their targets. So the `GDX` repository path doesn't need to be exactly the same, but it does need to be 4 folders deep from the root.

Path | Description
--- | ---
\\\\Documentation | Git clone of `https://github.com/dotBunny/GDX.Documentation`
\\\\Projects\000_Development\Assets\com.dotbunny.gdx | Git clone of `https://github.com/dotBunny/GDX`
\\\\External\docfx\docfx.exe | Extracted version (2.x) from `https://github.com/dotnet/docfx/releases`

### Scripts
Path | Description
--- | ---
clean-build-deploy | Clean all cached data, generate and deploy documentation to the `GDX.Documentation` `docs` folder.
clean-build | Clean all cached data and generate documentation.
clean-serve | Clean all cached data, generate and [host](http://localhost:8080) documentation for preview.
serve | Iteratively generate and [host](http://localhost:8080) documentation for preview.

### Common Issues

#### Missing NuGet Build Tasks
One issue that often comes up when trying to build the documents from source is a warning message in the log:

> [!WARNING]
> The "GetReferenceNearestTargetFrameworkTask" task was not found.

This happens when a specific **Code tools** component `NuGet targets and build tasks` was not installed during the Visual Studio installation. You can resolve this by launching the **Visual Studio Installer** and modifying the existing installation.