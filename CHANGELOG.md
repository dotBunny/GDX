# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.2.4] - 2021-02-11
***"Cold Brew"***
> Bugfixes, feedback, and features, just what a growing library needs.

### Added
- `GDX` scripting define is automatically added to all build targets.
- `GDX.GameObjectExtensions` and `GDX.TransformExtensions` received `DestroyChildren()` functionality.
- `GDX.DisableInInspectorAttribute` and associated `PropertyDrawer`.
- `InspectorMessageBoxAttribute` for ease of messaging/reminders.

### Changed
- Restored reference to Unity.PerformanceTesting
- Adopted using `dev` branch for active development, pulling into version named branches for patching.

### Removed
- `InspectorLabelAttribute`, Unity has native `InspectorNameAttribute` in 2019.1+

## [1.2.3] - 2021-02-09
***"Workplace 2.0"***
> Extending functionality out with more code from the backlog, while still addressing some oddities in the existing codebase.

### Added
- A Unity serializable dictionary based type `GDX.Collections.Generic.SerializableDictionary`.
  - The `SerializableDictionaryCustomPropertyDrawer` requires Unity 2020.1 or newer; while the `SerializableDictionary` will still work without it, just without a pretty `PropertyDrawer`.
- `GDX.Vector3Extensions.DistanceToRay()`
- A way to get key positions `GDX.CapsuleCollider.OutSphereCenters()` from a `CapsuleCollider`.
- Improved on `GDX.StringExtensions`.
  - Optimized test `IsBooleanValue()`.
  - Optimized test `IsBooleanPositiveValue()`.
  - Optimized test `IsIntegerValue()`
  - Optimized test `IsNumericalValue()`.
  - Exposed ASCII markers.
- `GDX.NavMeshPathExtensions` to help with working with AI/navigation.  
- `GDX.TransformExtensions`
  - A depth-limited `GetFirstComponentInChildrenComplex()` method.
  - A quick `GetActiveChildCount()` reporting only on active child transforms.
- Automated culture setting on main thread when an unknown system language is found, protecting against specific calender situations.
- `LICENSE.meta` has returned to the package to stop compile warnings, we will just have to deal with GitHub not being able to figure out the license model automatically.
### Changed
- Added language to `README.md` and documentation regarding associations to Unity.
- Optimized `GDX.GDXConfig` loading at runtime.
- Wrap `GDX.InspectorLabelAttribute` in `UNITY_EDITOR` define requirement.
- Consolidated entirety of `GDX.Editor.Settings` related classes into private classes inside of itself.
- Moved `GDX.Editor.InspectorLabelPropertyDrawer` to `GDX.Editor.PropertyDrawers.InspectorLabelPropertyDrawer`
### Removed
- Centralized `GDX.Strings`, moving ownership of data to the actual primary consumer.
- Reference to unit test locations from files.
- Corrected Jobs package being included in Unity's CoreModule.

## [1.2.2] - 2021-02-1
***"Cookie Monster"***
> A whole lot of work went into trying to solidify what documentation is going to look like, as well as get foundational work in place to make sure anyone can contribute.

### Added
- Moved all static `GUIContent` from settings window into new `SettingsContent`.
- Moved all static layout functionality from `SettingsStyles` into `SettingsLayout`.
- `GDX.Developer.Conditionals` contains constant status indicators of packages used by `GDX`. Useful for determine if a certain feature set is available.
- Properly set define GDX_PLATFORMS based on the `com.unity.platforms` package.
- The`GDX.Localization.GetHumanReadableFileSize()` method to create more readable file size outputs.
- More functionality in `GDX.Vector2Extensions`
  - `NearestIndex()` to find the closest position in an array.
  - `Slope()`
- Additional functionality in `GDX.Vector3Extensions`
  - `HorizontalDistance()` to get a horizontal distance ignoring vertically.
  - `NearestIndex()` to find the closest position in an array of positions.
- `GDX.StringExtensions.TryParseVector2()` and `GDX.StringExtensions.TryParseVector3()` will rehydrate `0,0` and `0,0,0` formatted strings.
- A complex version of `GetComponentInChildren()`, called `GetFirstComponentInChildrenComplex()` is available in `GDX.GameObjectExtensions` and `GDX.MonoBehaviourExtensions`, allowing for recursion limits.
- `GDX.BoxColliderExtensions.ContainsPosition()` as a quick method to determine if a world space position is inside of a `BoxCollider`.
- `GDX.CapsuleColliderExtensions.Direction()` to get a `Vector3` based direction for a `CapsuleCollider`.
- `GDX.RigidbodyExtensions.MomentOfInertia()` for an inertia calculation based on axis.
- `GDX.Mathematics.Smooth` adds `Exponential()` smoothing functionality.
- `GDX.Mathematics.Rotate` adds `Towards()` calculations.

### Changed
- [Generated documentation](https://gdx.dotbunny.com/) now includes `private` and `internal` classes.
- Lowered feature requirement of `com.unity.jobs` to `0.2.7`, and `com.unity.burst` to `1.0.0`.
- `GDX.Developer` assembly collapsed back into main `GDX` assembly.
- Modified settings framework to be in a single `GDX` category in the **Project Settings** window, with collapsable sections contained within.
- `GDX.StringExtensions.GetLowerCaseHashCode()` renamed to `GDX.StringExtensions.GetStableLowerCaseHashCode()`.
- `GDX.StringExtensions.GetUpperCaseHashCode()` renamed to `GDX.StringExtensions.GetStableUpperCaseHashCode()`.
- `GDX.ByteExtensions.GetValueHashCode()` renamed to `GDX.ByteExtensions.GetStableHashCode()`.
- Better package installation type detection and handling of upgrades. 
  - This has cut down the possibilities of automatic upgrades, however efforts will continue to expand on this functionality.

## [1.2.1] - 2021-01-24
***"Old Is New"***
> A DocFX generated [site](https://gdx.dotbunny.com/) is now being stood up by our internal CI when a new commit is made to the `main` branch.
### Added
- A proper [**Getting Started**](https://gdx.dotbunny.com/manual/getting-started.html) section to the manual
### Changed
- Fixes to compilation of code requiring C# 8 using `UNITY_2020_2_OR_NEWER` preprocessor in `GDX.IO.Compression.TarFile`, `GDX.StringExtensions` and `GDX.Editor.UpdateProvider`.
### Removed
- Removed the Wiki links and entries on GitHub, favoring discussions and content additions to the [documentation](https://gdx.dotbunny.com/).

## [1.2.0] - 2021-01-20
***"Cisco's Birthday"***
> A bit of refactoring around `BuildInfo` to make it a little easier to work with and some small additions to functionality by request.
### Added
- Separation of checking folders and files path structure, new `GDX.Platform.EnsureFileFolderHiearchyExists()` just for files.
- The ability (by default) to encompass the `BuildInfo` output folder in an assembly definition.
- An ability from **Project Settings** to output a default `BuildInfo` file.
- `AssemblyInfo` to each assembly to support _internal_ access during unit testing.
- Applied `MethodImplOptions.AggressiveInlining` to many methods.
- A bunch of split related functionality to `GDX.StringExtensions`.
  - `GetAfterFirst()`
  - `GetAfterLast()`
  - `GetBeforeFirst()`
  - `GetBeforeLast()`
- `GDX.Vector2Extensions` and `GDX.Vector3Extensions` with associated unit testing.
  - `Approximately()`
  - `Midpoint()`
- `GDX.StringExtensions`
  - `SplitCamelCase()` to help with formatting of internal data.
  - `Encrypt()` and `Decrypt()` for all your string hiding needs. 
### Changed
- All classes/structs with unit testing will reference the class in a comments
- `GDX.ListExtensions` renamed to `GDX.IListExtensions` (as well as its unit test class)
- Removed feature highlight section from `README.md`.
- Dropped in some unsafe attributes to `GDX.StringExtensions.HasLowerCase()` and `GDX.StringExtensions.HasUpperCase()` in `GDX.StringExtensions`.
- Reorganized **Project Settings** sections to be alphabetically sorted.
- `GDX.Platform.EnsureFolderHierarchyExists()`'s argument to be labeled `folderPath` to provide further clarity of the functional intent.
- Combined `GDX.Developer.Build.BuildInfoGenerator` into `GDX.Developer.Build.BuildInfoProvider`.
- Renamed `Editor.Build.BuildInfoGeneratorTests` to `Editor.Build.BuildInfoProviderTests`
- Fixed issue with output folder structure was not present for `BuildInfo` generation.
- Renamed `GDX.Editor.SettingsGUILayout` to `GDX.Editor.SettingsStyles`, while exposing more internals for reuse.

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
- Exposed `ProcessArguments()` in `GDX.Developer.CommandLineParser` to allow for manual arguments to be added.

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