# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.3] - 2021-01-19
### Added
- `Platform.EnsureFileFolderHiearchyExists()`  to check specifically for file paths.
### Changed
- Fixed issue with output folder structure was not present for `BuildInfo` generation.
- `Platform.EnsureFolderHierarchyExists()`'s argument to be labeled `folderPath` to provide further clarity of the functional intent.

## [1.1.2] - 2021-01-18
***"EditMode Enabled"***
> Fixes for author-time code accessing runtime only parts.
### Added
- Added more `EditMode` unit test coverage.
  - `GDX.Developer.Build.BuildInfoGeneratorTests`
  - `GDX.Developer.CommandLineParserTests`
### Changed
- Renamed `GDX.Editor.Config` to `GDX.Editor.ConfigProvider`
- Fixed a bug where author-time calls to `GDXConfig.Get()` would return a null as it is meant for runtime only, they will now route through an editor safe path.
- Moved all **Tests** to follow the test runner naming `EditMode` for editor runnable unit tests, namespaces included inside the assembly have been stripped down as well.
- Exposed `GDX.Developer.CommandLineParser.ProcessArguments()` to allow for manual arguments to be added.

## [1.1.1] - 2021-01-17
***"Let Us Build"***
> A minor problem came to light after pushing the button.
### Changed
- `GDX.Developer.Build.BuildInfoGenerator` will now forcibly tell the `GDX.Developer.CommandLineParser` to do its thing prior to filling out the file.

## [1.1.0] - 2021-01-17
***"Breaking Bad"***
> We are breaking some rules! This should have been a _major release_ as we have altered method names to be consistent across the API.
### Added
- `GDX` assembly documentation contains _remarks_ where a function or class requires the presence of Unity's CoreModule to function correctly.
- `GDX.GDXConfig` scriptable object self creates to store persistent project-wide configurations for both runtime and author-time, editable through **Project Settings**. 
- `GDX.InspectorLabelAttribute` (and supporting `GDX.Editor.InspectorLabelPropertyDrawer`) to facilitate a quick way of replacing a labels content in the inspector.
- `GDX.IO.Compression.TarFile` support for decompressing tarballs.
- `GDX.SemanticVersion` struct for assistance with versioning.
- `GDX.StringExtensions` gained `HasLowerCase()` and `HasUpperCase()` checks.
- `GDX.Strings.Null` is a constant null value string
- `GDX.Editor.Config` static utility class to help with `GDX.GDXConfig` at author-time.
- `GDX.Editor.Settings` to drive specific `GDX` assembly settings to show up in the **Project Settings** window.
- `GDX.Editor.SettingsGUILayout` to assist with creating a consistent feel for `GDX` settings.
- `GDX.Editor.GDXConfigEditor` enforces that a selected `GDXConfig` does not allow for inspector changes.
- `GDX.Editor.UpdateProvider`, `GDX.Editor.PackageProvider` to facilitate updating of GDX package from different installation sources.
- `GDX.Editor.VersionControl` static utility class to help with VCS operations.
- Unit test coverage for extension classes.
  - `GDX.ArrayExtensionsTests`
  - `GDX.ListExtensionsTests`
  - `GDX.SimpleListExtensionsTests`
- Additional coverage was added to `GDX.StringExtensionsTests` to cover `HasUpperCase()` and `HasLowerCase()` methods.
- `GDX.Developer` a **separate assembly** with more developer specific functionality.
- `GDX.Developer.CommandLineParser` to provide a simple, yet configurable argument parser.
- `GDX.Developer.Editor.Build.BuildInfoProvider` to facilitate automated BuildInfo generation across different pipelines.
- `GDX.Developer.Editor.Build.BuildInfoGenerator` to generate content for the BuildInfo file.
- `GDX.Developer.Editor.Settings` to drive specific `GDX.Developer` assembly settings to show up in the **Project Settings** window.
### Changed
- Updated the `README.md` header with logo and badges.
- Added release names to `CHANGELOG.md` as well as removed namespace sub-lists, settling on having full names in description instead.
- Altered arrangement of `LICENSE` with the hopes of appeasing the _GitHub_ overlords of license type detection.
- `GDX.ArrayExtensions` to be more specific
  - Corrected documentation of `Clear()`
  - Class based operations `FirstIndexOfItem()` and `LastIndexOfItem()`
  - Struct based operations `FirstIndexOfValue()` and `LastIndexOfValue()`
  - Correctly moved into GDX namespace.
- `GDX.ByteExtensions` to be more specific
  - Renamed hashing method to `GetValueHashCode()`
- `GDX.ListExtensions` to be more specific.
  - Class based operations `AddUniqueItem()`, `ContainsItem()`, `RemoveItems()`, `RemoveFirstItem()` and `RemoveLastItem()`..
  - Struct based operations `AddUniqueValue()`, `ContainsValue()`, `RemoveValues()`, `RemoveFirstValue()` and `RemoveLastValue()`.
- `GDX.SimpleListExtensions` to be more specific.
  - Class based operations `AddUncheckedUniqueItem()`, `AddWithExpandCheckUniqueItem()`, `ContainsItem()`, `RemoveItems()`, `RemoveFirstItem()` and `RemoveLastItem()`.
  - Struct based operations `AddUncheckedUniqueValue()`, `AddWithExpandCheckUniqueValue()`, `ContainsValue()`, `RemoveValues()`, `RemoveFirstValue()` and `RemoveLastValue()`.
- Moved `GDX.Collections.Byte2` to `GDX.Mathematics.Byte2` as it made more sense to alongside other similar types in `Unity.Mathematics`.
### Removed
- Removed unused static StringBuilder from `GDX.Strings`.

## [1.0.0] - 2021-01-03
***"The Beginning"***
> Initial release containing only the GDX core library.
### Added
- Numerous static extension based functionality classes.
  - `GDX.AddressablesExtensions`
  - `GDX.ArrayExtensions`
  - `GDX.ByteExtensions`
  - `GDX.ListExtensions`
  - `GDX.SimpleListExtensions`
  - `GDX.StringExtensions`
- Numerous static utility classes.
  - `GDX.Display`
  - `GDX.Localization`
  - `GDX.Memory`
  - `GDX.Platform`
  - `GDX.Strings`
- Bit array structures.
  - `GDX.Collections.BitArray8`
  - `GDX.Collections.BitArray16`
  - `GDX.Collections.BitArray32`
  - `GDX.Collections.BitArray64`
  - `GDX.Collections.BitArray128`
  - `GDX.Collections.BitArray256`
  - `GDX.Collections.BitArray512`
- Byte vector `GDX.Collections.Byte2`.
- Sparse index pool structures `GDX.Collections.SparseSet` and `GDX.Collections.NativeSparseSet`
- `GDX.Collections.FreeList`
- Revolving buffer `GDX.Collections.Generic.CircularBuffer`.
- `List`-like structure `GDX.Collections.Generic.SimpleList`.
- A few `NativeArray` based structures.
  - `GDX.Collections.Generic.NativeArray2D`
  - `GDX.Collections.Generic.NativeSimpleList`
  - `GDX.Collections.Generic.NativeSimpleQueue`
  - `GDX.Collections.Generic.NativeUniformArray3D`.
- Numerous `int32` buffer operation jobs.
  - `GDX.Jobs.ParallelFor.IntegerBufferCopyJob`
  - `GDX.Jobs.ParallelFor.IntegerBufferFillJob`
  - `GDX.Jobs.ParallelFor.IntegerBufferSwapJob`
- Deterministic random `GDX.Mathematics.Random.MersenneTwister`.