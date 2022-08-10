# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [3.1.0] - ?
***"Dinner Table"***
>

### Addded

- New toggle added to Project Settings -> GDX -> Environment, ensuring that shaders included are always included in a build

### Fixed

- `StringKeyDictionary` and `IntKeyDictionary` can now be initialized with capacities as static fields.

## [3.0.1] - 2022-07-12

***"3D Arrays"***
> A few additions with a bug fix that emerged.

### Added

- New `Array3D` and `NativeArray3D` flattened 3D arrays.

### Changes

- `BitArray8`, `BitArray16`, `BitArray32`, `BitArray64` have been marked `Serializable`.

### Fixed

- `IntKeyDictionary.Clear()` now properly sets bucket values.

## [3.0.0] - 2022-07-05

***"Clean Slate"***
> As we began planning what this release might look like, the decision was made to attempt to clear out a significant portion of our technical debt by sunsetting unsupported versions of Unity. We also took this opportunity to reevaluate and upgrade portions of the framework to better position it for future growth.

### Added

- Dependencies on `com.unity.mathematics`, `com.unity.burst`, `com.unity.jobs` and `com.unity.collections` packages.
- Support for `com.unity.runtime.dots` (UNITY_DOTSRUNTIME) builds. Unsupported functionality will cause compilation errors during builds for immediate feedback on unsupported usage. Additionally, unsupported methods are flagged with an `UnsupportedRuntimeException` which is visible in [documentation](https://gdx.dotbunny.com/api) as well as IDE intelli-sense.
- Support for `com.unity.entities` programming patterns.
- `BuildVerificationReport` structure for quick validation of builds, including a `TestBehaviour`.
- `StringKeyDictionary` to optimize one of the most common use cases for dictionaries.
- `SegmentedString` to support string search patterns.
- `OriginalValueAttribute` to support a default value-like pattern.
- Unity editor non-pro (light) theme.
- Additional information around [coding standards](https://gdx.dotbunny.com/manual/contributing/coding-standard.html) to documentation.
- `UniformArray3D` counter part to `NativeUniformArray3D`

### Changed

- Properly formatted the `CHANGELOG.md` thanks to a proper linter, as well as tried to unify nomenclature used to describe changes.
- The `GDX` namespace has been reorganized.
- Conform naming conventions to Unity standard.
- `GDXConfig` has been simplified to `Config`, with member variables being statically accessible.
- Package's project settings were rebuilt to use `UIElement`s instead of `IMGUI` with added searchability.
- `Smooth::HalfLifeToSmoothingFactor()` no longer defaults elapsed time to `Time.deltaTime`
- The visual scripting module detects the package based installation available in `Unity 2021` and forward. If you wish to have support in `Unity 2020.3` via the Asset Store you will need to add a scripting define symbol of `GDX_VISUALSCRIPTING` to your project.
- All internal `Dictionary<string, value>` have been replaced with `StringKeyDictionary<value>`
- `BuildInfoProvider` now uses a `TextGenerator` for codegen.
- `Platform::GetOutputFolder()` supports being overridden via command-line argument `GDX_OUTPUT_FOLDER`.
- Internal members of `SerializableDictionary` have been renamed, `FormerlySerializedAs` attribute has been used to hopefully upgrade content.
- `Localization::GetIETF()` was renamed for clarity to `Localization::GetIETF_BCP47()`, with further value definitions.
- Optimized internals of `CircularBuffer` removing some garbage being made when using `ToArray()`.
- `ListManagedPool` rebuilt as `SimpleListManagedPool` for all `object` based pooling needs.
- Flagged `DisableInInspectorAttributePropertyDrawer` to only use UIElements version with `UNITY_2022_2_OR_NEWER`.
- `GenerateProjectFiles` now has more macOS options.
- The Camera extension method `CaptureToPNG()` has been renamed `RenderToPNG()` to reflect better its actual actions.
- `TransformExtensions::DestroyChildren()` now has an immediate mode to remove children at author time.
- Moved `SemanticVersion` into the `GDX.Developer` namespace.
- Moved `TransientReference` into the `GDX.Developer` namespace.
- `Byte2` accessor throws `IndexOutOfRangeException` when using `ENABLE_UNITY_COLLECTIONS_CHECKS`.

### Fixed  

- `Platform::IsFocused()` now returns the proper focus state on desktop platforms.
- `TransformExtensions::DestroyChildren()` no longer has issues with child counts.
- `NativeUniformArray3D.GetFromIndex()` provides the correct index now.
- `NUnitReport.GetResultCount()` now reflects accurate count post `NUnitReport.Process()`.

### Removed

- `Automation::GetTempFolder()` in favour of using `Platform::GetOutputFolder()`.
- `Automation::GetTempFilePath()` in favour of using `Platform::GetUniqueOutputFilePath()`.
- `NativeSimpleList` in favour of builtin collection `UnsafeList`.
- `NativeSimpleQueue` in favour of builtin collection `UnsafeQueue`.
- `GDXConfigInspector` no longer needed as the serialized config model has been replaced.
- All "Requires UnityEngine.CoreModule.dll" remarks removed.

## [2.0.3] - 2021-12-01

***"Automate This"***
> This marks the start of our effort to refactor **GDX** to be more compatible outside of the GameObject world that we currently live in. Over the next couple versions a lot of effort will be going into making types Burst compatible. We will do this by changing the backing types to NativeCollections, however this will result in some slight changes to the API to force cleanup.

### Added

- A `RandomWrapper` was created to allow for `System.Random` to be used with the `IRandomProvider` interface.
- Cleaning method `StringExtensions::StripNonAscii()`.
- Editor scoped functionality, with supporting functionality builtin to provide reliable Unity editor testing.
  - `Automation::CaptureEditorWindow<T>()`
  - `Automation::CaptureEditorWindowToPNG<T>()`
  - `Automation::CaptureFocusedEditorWindow()`
  - `Automation::CaptureFocusedEditorWindowToPNG()`
- Numerous color comparison operation jobs.
  - `Jobs.ParallelFor.Color32CompareJob`
  - `Jobs.ParallelFor.Color32MatchJob`
  - `Jobs.ParallelFor.ColorCompareJob`
  - `Jobs.ParallelFor.ColorMatchJob`

### Changed

- Now using a `NativeArray<uint>` to store state in the 'WELL1024a', now requires the use of `Dispose()`.
- Some `Platform` methods behaved like extensions when they should not have been.

### Fixed

- `WELL1024a` exclusive methods truly will exclude the values correctly.

## [2.0.2] - 2021-11-03

***"Told Ya"***
> Mistakes were made; fixes happened.

### Added

- Infinitely sized `CoalesceStream` available for dealing with large data streams.
- A few (`Platform::EnsureFileWritable()`, `Platform::ForceDeleteFile()`, `Platform::IsFileWritable()`) file permission operations have been added.

### Fixed

- The manifest for the package can be found again! Corrected path information to reflect new hierarchy.

## [2.0.1] - 2021-11-02

***"Feature Branches"***
> A lot of experimental work is being now done in *feature* branches; this should speed up iteration time on releases.

### Added

- `Trace` configuration matrix available in project settings.

### Changed

- Internally used `SettingsStyles` has been split into `SettingsStyles` and `SettingsLayoutOptions` accordingly.
- Moved everything up one folder layer in package to fit with package standards.

### Fixed

- Optimized referencing of `GDXConfig` in author time operations (now similar to runtime).
- `IListExtensions::ContainsItem()` now uses `Equals()` to resolve literals issues with strings.
- Categories for Visual Scripting based entries are now correct.
- Resolved issue with newer Package Manager based lock files having no tag identities.

## [2.0.0] - 2021-05-09

***"Fresh Paint"***
> Breaking changes and a new license (BSL-1.0); making GDX even easier to include in projects!

### Added

- New [FAQ](https://gdx.dotbunny.com/manual/faq.html) section of the website, addressing some of the more frequently asked questions.
- `WELL1024a` implementation to replace removed `MersenneTwister` in GDX .
- `IRandomProvider` and `RandomAdaptor` to allow for some interchange with existing usages; these are slow and should be used as a last resort.
- `StringExtensions::GetStableHashCode()` for generating hashcode of strings identical to `GetHashCode()`, without the virtual call.
- `TransientReference` provides a comparable non-garbage collection blocking reference type.
- `Report` provides some of the common logic used by the newly added `ResourcesAudit` and `ResourcesDiff`. Think of this as an incredibly simple way to find resource memory leaks.

### Changed

- File license headers, repository wide now reference the BSL-1.0 license.
- Corrected minimum compatible version to `2018.4` in `README.md`
- Added latest version `2.0.0` to `SECURITY.md`, sunsetting `1.2.x`.
- `VisualScriptingCollectionAttribute`, `VisualScriptingExtensionAttribute`, `VisualScriptingTypeAttribute`, `VisualScriptingUtilityAttribute` have been consolidated to `VisualScriptingCompatible`.
- `NextDouble`, `NextSingle` functionality on `IRandomProviders` do not default to extreme values.
- `NativeSimpleList` and `NativeSimpleQueue` are only available when `com.unity.collections` is not present.
- Altered API documentation to have a heirachial namespace index using custom tooling.  

### Removed

- Removed some supporting types and methods used by GDX's project settings from [documentation](https://gdx.dotbunny.com).
- Removed `MersenneTwister` to allow for our new licensing model (functionally replaced with `WELL1024a` implementation).

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

- A proper [**Getting Started**](https://gdx.dotbunny.com/manual/getting-started/index.html) section to the manual

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
- `AssemblyInfo` to each assembly to support *internal* access during unit testing.
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
> We are breaking some rules! This should have been a *major release* as we have altered method names to be consistent across the API.

### Added

- `GDX` assembly documentation contains *remarks* where a function or class requires the presence of Unity's CoreModule to function correctly.
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
- Altered arrangement of `LICENSE` with the hopes of appeasing the *GitHub* overlords of license type detection.
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
