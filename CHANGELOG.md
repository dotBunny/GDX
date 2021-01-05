# Changelog
All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]
### Added
#### GDX.Editor
- Added `UpdateManager` to facilitate updating of Unity Asset Store installations.

### Changed
- Updated the `README.md` header with logo and badges.

## [1.0.0] - 2021-01-03
Initial release containing only the GDX core library.
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
