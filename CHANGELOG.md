# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] - 2021-04-06
***"Visual Time"***
> An effort to make GDX more accessible to Visual Scripting.

### Added
- Automated compatibility tests for 2018.4 LTS, 2019.4 LTS and 2020.3 LTS have been added to internal CI.
- Automated portability checks for .NET Standard 2.0 and .NET Core 3.1 to internal CI.
- Support for Visual Scripting (Bolt) with options in **Project Settings** to add a currated portion of the `GDX` API for usage with Visual Scripting.
- New array pooling type `ArrayPool`, with corresponding `JaggedArrayWithCount`.
- Numerous collections gained `Reverse()` methods, and the newly added `Array2D` also having `ReverseRows()` and `ReverseColumns()`.

### Changed
- Stated support for GDX has shifted to current release cycle Unity, with support for 2018.4 LTS, 2019.4 LTS and 2020.3 LTS. This doesn't mean that it will not work with other versions, just our automation only checks against LTS and current versions.
- Removed the extension `Get` method from `GameObjectPool` to match the other methods.
- Only Unity 2020+ supports the Package Manager resolve function, previous versions of Unity will be presented with an options dialog.
- Consolidated access/creation of `GDXConfig`.
- Altered `NativeArray2D` to index accessor to function like `Array2D`.

### Fixed
- Force the Package Manager to resolve the package manifest during an update.
- Occasional infinite loop importing `GDXConfig` with a cache server.
- Unsupported attributes for Unity 2018.

### Removed
- Struct specific extension methods have been removed from `IListExtensions` and `SimpleListExtensions`; explicitly to avoid obsfucating boxing types and hiding a problem.

## [1.2.7] - 2021-03-16
***"Never Say Never"***
> Updates have been tested across different installation methods, a pooling system, and some fixes!

### Added
- UPM and GitHub installation methods utilizing the`dev` branch will have a "Force Update" action available to them in the **Project Settings**.
- `PlatformExtensions` now has a `IsHeadless()` method for determining if the application is running without a graphics device initialized; aka a headless server.
- `EnumExtensions` has a faster `HasFlags()` method for working with flags. 
- `PoolingSystem` now exists in the `GDX.Collections.Pooling` namespace, including a `GameObjectPool` system.
- `Trace` static now available to funnel all `GDX` based logging through, with editor/build configurations available.

### Changed
- Made `SparseSets` .NET Standard 2.0 compliant [@godjammit](https://github.com/dotBunny/GDX/pull/41)

### Fixed
- Caught issue with `initialCapcity` causing an OOB issue with `SparseSets` [@godjammit](https://github.com/dotBunny/GDX/pull/41)
- Caught a few more null coalescing assignments that are not compatible with Unity 2019 in `InspectorMessageBoxAttributeDecoratorDrawer`. (thanks Nick & Gabe)
- Resolved `PackageProvider` issues with Unity 2019.

## [1.2.6] - 2021-02-15
***"UPM Updates"***
> Starting to frame up the ability to update package adds from GitHub.

### Added
- Some functionality around being able to update a UPM based package, by removing the entry in the lockfile, a package will update.

### Fixed
- `NavMeshPathExtensions` no longer breaks compiling without `com.unity.mathematics`.

## [1.2.5] - 2021-02-12
***"Hello World"***
> Enough critical mass has been hit with functionality that with this release we will start to look at publicizing the package more.

### Added
- Environment section of GDX project settings allows for an initial toggling of `GDX` scripting define symbol enforcement.
- `TransformExtensions` gained a `GetScenePath()` to create an easy way to identify an object in logs.
- `GameObjectExtensions` extended out to have more helper functionality.
  - A `GetOrAddComponent()` method which ensures that a component exists (by adding) when requested.
  - The `GetScenePath()` for easy identification as well.

### Changed
- The `CHANGELOG.md` has had it's identifiers simplified, retroactively.
- Functionality from inside of `Editor.Build.BuildInfoProvider` has been split into more specific classes for different build pipeline / paths.  
  - Classic Build Pipeline
    - `BuildInfoBuildCustomizer` produces the `BuildInfo` file during that pipeline execution.
    - `ScriptingDefinesBuildCustomizer` ensures the `GDX` scripting define is set during builds (if enabled).
  - Legacy Build Pipeline
    - `BuildInfoBuildProcessor` is used to produce the `BuildInfo` file during legacy builds, as well as resetting it in *both* Classic and Legacy builds.
    - `ScriptingDefinesBuildProcessor` ensures the `GDX` scripting define is set during legacy builds.

### Fixed
- The `CHANGELOG.md` retroactively has had a *Fixed* section added.

## [1.2.4] - 2021-02-11
***"Cold Brew"***
> Bugfixes, feedback, and features, just what a growing library needs.

### Added
- `GDX` scripting define is automatically added to all build targets.
- `GameObjectExtensions` and `TransformExtensions` received a `DestroyChildren()` function.
- `DisableInInspectorAttribute` for all your disabled field needs.
- `InspectorMessageBoxAttribute` for ease of messaging/reminders.

### Changed
- Adopted using `dev` branch on GitHub for active development, pulling into version named branches for patching.

### Removed
- `InspectorLabelAttribute`, Unity has native `InspectorNameAttribute` in 2019.1+

### Fixed
- Restored reference to Unity.PerformanceTesting

## [1.2.3] - 2021-02-09
***"Workplace 2.0"***
> Extending functionality out with more code from the backlog, while still addressing some oddities in the existing codebase.

### Added
- A Unity serializable dictionary based type `Collections.Generic.SerializableDictionary`.
  - The `SerializableDictionaryCustomPropertyDrawer` requires Unity 2020.1 or newer; while the `SerializableDictionary` will still work without it, just without a pretty `PropertyDrawer`.
- `Vector3Extensions.DistanceToRay()`
- A way to get key positions `CapsuleCollider.OutSphereCenters()` from a `CapsuleCollider`.
- Improved on `StringExtensions`:
  - Optimized test `IsBooleanValue()`.
  - Optimized test `IsBooleanPositiveValue()`.
  - Optimized test `IsIntegerValue()`
  - Optimized test `IsNumericalValue()`.
  - Exposed ASCII markers.
- `NavMeshPathExtensions` to help with working with AI/navigation.  
- `TransformExtensions`
  - A depth-limited `GetFirstComponentInChildrenComplex()` method.
  - A quick `GetActiveChildCount()` reporting only on active child transforms.
- Automated culture setting on main thread when an unknown system language is found, protecting against specific calender situations.
- `LICENSE.meta` has returned to the package to stop compile warnings, we will just have to deal with GitHub not being able to figure out the license model automatically.
### Changed
- Added language to `README.md` and documentation regarding associations to Unity.
- Optimized `GDXConfig` loading at runtime.
- Wrap `InspectorLabelAttribute` in `UNITY_EDITOR` define requirement.
- Consolidated entirety of `Editor.Settings` related classes into private classes inside of itself.
- Moved `Editor.InspectorLabelPropertyDrawer` to `Editor.PropertyDrawers.InspectorLabelPropertyDrawer`
### Removed
- Centralized `Strings`, moving ownership of data to the actual primary consumer.
- Reference to unit test locations from files.
- Corrected Jobs package being included in Unity's CoreModule.

## [1.2.2] - 2021-02-1
***"Cookie Monster"***
> A whole lot of work went into trying to solidify what documentation is going to look like, as well as get foundational work in place to make sure anyone can contribute.

### Added
- Moved all static `GUIContent` from settings window into new `SettingsContent`.
- Moved all static layout functionality from `SettingsStyles` into `SettingsLayout`.
- `Developer.Conditionals` contains constant status indicators of packages used by `GDX`. Useful for determine if a certain feature set is available.
- Properly set define GDX_PLATFORMS based on the `com.unity.platforms` package.
- The`Localization.GetHumanReadableFileSize()` method to create more readable file size outputs.
- More functionality in `Vector2Extensions`
  - `NearestIndex()` to find the closest position in an array.
  - `Slope()`
- Additional functionality in `Vector3Extensions`
  - `HorizontalDistance()` to get a horizontal distance ignoring vertically.
  - `NearestIndex()` to find the closest position in an array of positions.
- `StringExtensions`'s `TryParseVector2()` and `TryParseVector3()` will rehydrate `0,0` and `0,0,0` formatted strings.
- A complex version of `GetComponentInChildren()`, called `GetFirstComponentInChildrenComplex()` is available in `GameObjectExtensions` and `MonoBehaviourExtensions`, allowing for recursion limits.
- `BoxColliderExtensions.ContainsPosition()` as a quick method to determine if a world space position is inside of a `BoxCollider`.
- `CapsuleColliderExtensions.Direction()` to get a `Vector3` based direction for a `CapsuleCollider`.
- `RigidbodyExtensions.MomentOfInertia()` for an inertia calculation based on axis.
- `Mathematics.Smooth` adds `Exponential()` smoothing functionality.
- `Mathematics.Rotate` adds `Towards()` calculations.

### Changed
- [Generated documentation](https://gdx.dotbunny.com/) now includes `private` and `internal` classes.
- Lowered feature requirement of `com.unity.jobs` to `0.2.7`, and `com.unity.burst` to `1.0.0`.
- `GDX.Developer` assembly collapsed back into main `GDX` assembly.
- Modified settings framework to be in a single `GDX` category in the **Project Settings** window, with collapsable sections contained within.
- `StringExtensions.GetLowerCaseHashCode()` renamed to `StringExtensions.GetStableLowerCaseHashCode()`.
- `StringExtensions.GetUpperCaseHashCode()` renamed to `StringExtensions.GetStableUpperCaseHashCode()`.
- `ByteExtensions.GetValueHashCode()` renamed to `ByteExtensions.GetStableHashCode()`.
- Better package installation type detection and handling of upgrades. 
  - This has cut down the possibilities of automatic upgrades, however efforts will continue to expand on this functionality.

## [1.2.1] - 2021-01-24
***"Old Is New"***
> A DocFX generated [site](https://gdx.dotbunny.com/) is now being stood up by our internal CI when a new commit is made to the `main` branch.
### Added
- A proper [**Getting Started**](https://gdx.dotbunny.com/manual/getting-started.html) section to the manual
### Changed
- Fixes to compilation of code requiring C# 8 using `UNITY_2020_2_OR_NEWER` preprocessor in `IO.Compression.TarFile`, `StringExtensions` and `Editor.UpdateProvider`.
### Removed
- Removed the Wiki links and entries on GitHub, favoring discussions and content additions to the [documentation](https://gdx.dotbunny.com/).

## [1.2.0] - 2021-01-20
***"Cisco's Birthday"***
> A bit of refactoring around `BuildInfo` to make it a little easier to work with and some small additions to functionality by request.
### Added
- Separation of checking folders and files path structure, new `Platform.EnsureFileFolderHiearchyExists()` just for files.
- The ability (by default) to encompass the `BuildInfo` output folder in an assembly definition.
- An ability from **Project Settings** to output a default `BuildInfo` file.
- `AssemblyInfo` to each assembly to support _internal_ access during unit testing.
- Applied `MethodImplOptions.AggressiveInlining` to many methods.
- A bunch of split related functionality to `StringExtensions`.
  - `GetAfterFirst()`
  - `GetAfterLast()`
  - `GetBeforeFirst()`
  - `GetBeforeLast()`
- `Vector2Extensions` and `Vector3Extensions` with associated unit testing.
  - `Approximately()`
  - `Midpoint()`
- `StringExtensions`
  - `SplitCamelCase()` to help with formatting of internal data.
  - `Encrypt()` and `Decrypt()` for all your string hiding needs. 
### Changed
- All classes/structs with unit testing will reference the class in a comments
- `ListExtensions` renamed to `IListExtensions` (as well as its unit test class)
- Removed feature highlight section from `README.md`.
- Dropped in some unsafe attributes to `StringExtensions.HasLowerCase()` and `StringExtensions.HasUpperCase()` in `StringExtensions`.
- Reorganized **Project Settings** sections to be alphabetically sorted.
- `Platform.EnsureFolderHierarchyExists()`'s argument to be labeled `folderPath` to provide further clarity of the functional intent.
- Combined `Developer.Build.BuildInfoGenerator` into `Developer.Build.BuildInfoProvider`.
- Renamed `Editor.Build.BuildInfoGeneratorTests` to `Editor.Build.BuildInfoProviderTests`
- Fixed issue with output folder structure was not present for `BuildInfo` generation.
- Renamed `Editor.SettingsGUILayout` to `Editor.SettingsStyles`, while exposing more internals for reuse.

## [1.1.2] - 2021-01-18
***"EditMode Enabled"***
> Fixes for author-time code accessing runtime only parts.
### Added
- Added more `EditMode` unit test coverage.
  - `Developer.Build.BuildInfoGeneratorTests`
  - `Developer.CommandLineParserTests`
### Changed
- Renamed `Editor.Config` to `Editor.ConfigProvider`
- Fixed a bug where author-time calls to `GDXConfig.Get()` would return a null as it is meant for runtime only, they will now route through an editor safe path.
- Moved all **Tests** to follow the test runner naming `EditMode` for editor runnable unit tests, namespaces included inside the assembly have been stripped down as well.
- Exposed `ProcessArguments()` in `Developer.CommandLineParser` to allow for manual arguments to be added.

## [1.1.1] - 2021-01-17
***"Let Us Build"***
> A minor problem came to light after pushing the button.
### Changed
- `Developer.Build.BuildInfoGenerator` will now forcibly tell the `Developer.CommandLineParser` to do its thing prior to filling out the file.

## [1.1.0] - 2021-01-17
***"Breaking Bad"***
> We are breaking some rules! This should have been a _major release_ as we have altered method names to be consistent across the API.
### Added
- `GDX` assembly documentation contains _remarks_ where a function or class requires the presence of Unity's CoreModule to function correctly.
- `GDXConfig` scriptable object self creates to store persistent project-wide configurations for both runtime and author-time, editable through **Project Settings**. 
- `InspectorLabelAttribute` (and supporting `Editor.InspectorLabelPropertyDrawer`) to facilitate a quick way of replacing a labels content in the inspector.
- `IO.Compression.TarFile` support for decompressing tarballs.
- `SemanticVersion` struct for assistance with versioning.
- `StringExtensions` gained `HasLowerCase()` and `HasUpperCase()` checks.
- `Strings.Null` is a constant null value string
- `Editor.Config` static utility class to help with `GDXConfig` at author-time.
- `Editor.Settings` to drive specific `GDX` assembly settings to show up in the **Project Settings** window.
- `Editor.SettingsGUILayout` to assist with creating a consistent feel for `GDX` settings.
- `Editor.GDXConfigEditor` enforces that a selected `GDXConfig` does not allow for inspector changes.
- `Editor.UpdateProvider`, `Editor.PackageProvider` to facilitate updating of GDX package from different installation sources.
- `Editor.VersionControl` static utility class to help with VCS operations.
- Unit test coverage for extension classes.
  - `ArrayExtensionsTests`
  - `ListExtensionsTests`
  - `SimpleListExtensionsTests`
- Additional coverage was added to `StringExtensionsTests` to cover `HasUpperCase()` and `HasLowerCase()` methods.
- `GDX.Developer` a **separate assembly** with more developer specific functionality.
- `Developer.CommandLineParser` to provide a simple, yet configurable argument parser.
- `Developer.Editor.Build.BuildInfoProvider` to facilitate automated BuildInfo generation across different pipelines.
- `Developer.Editor.Build.BuildInfoGenerator` to generate content for the BuildInfo file.
- `Developer.Editor.Settings` to drive specific `GDX.Developer` assembly settings to show up in the **Project Settings** window.
### Changed
- Updated the `README.md` header with logo and badges.
- Added release names to `CHANGELOG.md` as well as removed namespace sub-lists, settling on having full names in description instead.
- Altered arrangement of `LICENSE` with the hopes of appeasing the _GitHub_ overlords of license type detection.
- `ArrayExtensions` to be more specific
  - Corrected documentation of `Clear()`
  - Class based operations `FirstIndexOfItem()` and `LastIndexOfItem()`
  - Struct based operations `FirstIndexOfValue()` and `LastIndexOfValue()`
  - Correctly moved into GDX namespace.
- `ByteExtensions` to be more specific
  - Renamed hashing method to `GetValueHashCode()`
- `ListExtensions` to be more specific.
  - Class based operations `AddUniqueItem()`, `ContainsItem()`, `RemoveItems()`, `RemoveFirstItem()` and `RemoveLastItem()`..
  - Struct based operations `AddUniqueValue()`, `ContainsValue()`, `RemoveValues()`, `RemoveFirstValue()` and `RemoveLastValue()`.
- `SimpleListExtensions` to be more specific.
  - Class based operations `AddUncheckedUniqueItem()`, `AddWithExpandCheckUniqueItem()`, `ContainsItem()`, `RemoveItems()`, `RemoveFirstItem()` and `RemoveLastItem()`.
  - Struct based operations `AddUncheckedUniqueValue()`, `AddWithExpandCheckUniqueValue()`, `ContainsValue()`, `RemoveValues()`, `RemoveFirstValue()` and `RemoveLastValue()`.
- Moved `Collections.Byte2` to `Mathematics.Byte2` as it made more sense to alongside other similar types in `Unity.Mathematics`.
### Removed
- Removed unused static StringBuilder from `Strings`.

## [1.0.0] - 2021-01-03
***"The Beginning"***
> Initial release containing only the GDX core library.
### Added
- Numerous static extension based functionality classes.
  - `AddressablesExtensions`
  - `ArrayExtensions`
  - `ByteExtensions`
  - `ListExtensions`
  - `SimpleListExtensions`
  - `StringExtensions`
- Numerous static utility classes.
  - `Display`
  - `Localization`
  - `Memory`
  - `Platform`
  - `Strings`
- Bit array structures.
  - `Collections.BitArray8`
  - `Collections.BitArray16`
  - `Collections.BitArray32`
  - `Collections.BitArray64`
  - `Collections.BitArray128`
  - `Collections.BitArray256`
  - `Collections.BitArray512`
- Byte vector `Collections.Byte2`.
- Sparse index pool structures `Collections.SparseSet` and `Collections.NativeSparseSet`
- `Collections.FreeList`
- Revolving buffer `Collections.Generic.CircularBuffer`.
- `List`-like structure `Collections.Generic.SimpleList`.
- A few `NativeArray` based structures.
  - `Collections.Generic.NativeArray2D`
  - `Collections.Generic.NativeSimpleList`
  - `Collections.Generic.NativeSimpleQueue`
  - `Collections.Generic.NativeUniformArray3D`.
- Numerous `int32` buffer operation jobs.
  - `Jobs.ParallelFor.IntegerBufferCopyJob`
  - `Jobs.ParallelFor.IntegerBufferFillJob`
  - `Jobs.ParallelFor.IntegerBufferSwapJob`
- Deterministic random `Mathematics.Random.MersenneTwister`.