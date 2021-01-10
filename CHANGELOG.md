# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2021-01-18
***"Breaking Bad"***
> We are breaking some rules! This should have been a _major release_ as we have altered method names to be consistent across the API.
### Added
#### GDX
- Included `SemanticVersion` struct for assistance with versioning.
#### GDX.Developer
- Included a **separate assembly** with more developer specific functionality.
  - `CommandLineParser`
#### GDX.IO.Compression
- Included `TarFile` support for decompressing tarballs.
#### GDX.Editor
- Included `Config` static utility class to store persistent project-wide configurations for both runtime and author-time.
- Added `GDXStyles` to assist with editor styling.
- Added `UpdateProvider`, `PackageProvider` to facilitate updating of GDX package from different installation sources.
#### GDX.Tests
- Added `ArrayExtensionsTests`, `ListExtensionsTests` and `SimpleListExtensionsTests`
### Changed
- Updated the `README.md` header with logo and badges.
- Added release names to `CHANGELOG.md`.
- Altered arrangement of `LICENSE` with the hopes of appeasing the _GitHub_ overlords of license type detection.
#### GDX
- Changed `ArrayExtensions` to be more specific
  - Class based operations `FirstIndexOfItem()` and `LastIndexOfItem()`
  - Struct based operations `FirstIndexOfValue()` and `LastIndexOfValue()`
  - Correctly moved into GDX namespace.
- Changed `ListExtensions` to be more specific.
  - Class based operations `ContainsItem()`, `RemoveItems()`, `RemoveFirstItem()` and `RemoveLastItem()`..
  - Struct based operations `ContainsValue()`, `RemoveValues()`, `RemoveFirstValue()` and `RemoveLastValue()`.
- Changed `SimpleListExtensions` to be more specific.
  - Class based operations `ContainsItem()`, `RemoveItems()`, `RemoveFirstItem()` and `RemoveLastItem()`.
  - Struct based operations `ContainsValue()`, `RemoveValues()`, `RemoveFirstValue()` and `RemoveLastValue()`.
#### GDX.Mathematics
- Moved `Byte2` to `GDX.Mathematics` namespace (formerly in `GDX.Collections`) as it made more sense to alongside other similar types in `Unity.Mathematics`.

## [1.0.0] - 2021-01-03
***"The Beginning"***
> Initial release containing only the GDX core library.
### Added
#### GDX
- Included numerous static extension based functionality classes `AddressablesExtensions`, `ArrayExtensions`, `ByteExtensions`, `ListExtensions`, `SimpleListExtensions`, `StringExtensions`.
- Included numerous static utility classes `Display`, `Localization`, `Memory`, `Platform`, `Strings`.
#### GDX.Collections
- Included bit array structures `BitArray8`, `BitArray16`, `BitArray32`, `BitArray64`, `BitArray128`, `BitArray256`, `BitArray512`.
- Included byte vector `Byte2`.
- Included sparse index pool structures `SparseSet` and `NativeSparseSet`
- Included a `FreeList` data structure.
#### GDX.Collections.Generic
- Included a revolving buffer `CircularBuffer`.
- Included a `List`-like structure `SimpleList`.
- Included a few `NativeArray` based structures `NativeArray2D`, `NativeSimpleList`, `NativeSimpleQueue`, `NativeUniformArray3D`.
#### GDX.Jobs.ParallelFor
- Included numerous `int32` buffer operation jobs `IntegerBufferCopyJob`, `IntegerBufferFillJob`, `IntegerBufferSwapJob`.
#### GDX.Mathematics.Random
- Included deterministic random `MersenneTwister`.
