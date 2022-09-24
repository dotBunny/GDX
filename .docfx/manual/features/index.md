---
uid: features
---
# Features

> [!NOTE]
> Below is an overview of *some of* the notable features contained within the `GDX` runtime package. For a more thorough list of functionality, explore the [API documentation](/api/GDX.html) directly.

## Extending APIs

At the root of the `GDX` namespace are static extension and utility classes that build the foundation for the framework, providing missing functionality to numerous types inside the Unity API.

### C Sharp

Extension | Description
:--- | :---
[Array](xref:GDX.ArrayExtensions) | A bunch of `Array` operations which are commonly used by other methods. For example getting the last or first index of an item or value in an `Array`.
[Byte](xref:GDX.ByteExtensions) | Contains a simple non-allocating extension method to get a stable hash code from an array of `bytes` as well as an optimized comparison method.
[Enum](xref:GDX.EnumExtensions) | Useful functionality around `Flags`
[IList](xref:GDX.IListExtensions) | A common set of methods for `IList` implementing collections allowing for optimized value and item checks, as well as some extras.
[String](xref:GDX.StringExtensions) | A vast collection of extension methods covering a wide range from encryption to hash codes, to simply finding content an existing `string`.

### GDX

Extension | Description
:--- | :---
[Array2D](xref:GDX.Array2DExtensions) | A small amount of functionality around manipulating data contained within an `Array2D`.
[SimpleList](xref:GDX.SimpleListExtensions) | A slightly less performant set of functionality for a `SimpleList`.

### Unity

Extension | Description
:--- | :---
[Addressables](xref:GDX.AddressablesExtensions) | A collection of useful spawning and management functionality built on top of the `com.unity.addressables` package. For example identifying if a `AssetReference` is valid and able to be instantiated.
[BoxCollider](xref:GDX.BoxColliderExtensions) | Find out if a position is contained within a `BoxCollider`.
[Camera](xref:GDX.CameraExtensions) | Methods to force a specific `Camera` to render to specific outputs.
[CapsuleCollider](xref:GDX.CapsuleColliderExtensions) | Simple helper for finding out which way a `CapsuleCollider` is oriented.
[GameObject](xref:GDX.GameObjectExtensions) | Numerous methods meant to replace commonly implemented functionality around the component system and scene operations.
[Mesh](xref:GDX.MeshExtensions) | A helper to calculate the approximate volume of a mesh.
[MonoBehaviour](xref:GDX.MonoBehaviourExtensions) | A small set of wrapped functionality for component based operations.
[NavMeshPath](xref:GDX.NavMeshPathExtensions) | Some helpful functions when trying to build out AI logic.
[Rigidbody](xref:GDX.RigidbodyExtensions) | A method for determining the moment of inertia.
[Transform](xref:GDX.TransformExtensions) | Some useful functionality when it comes to working with `Transform`.
[Vector2](xref:GDX.Vector2Extensions) | Additional functionality for common calculations done with `Vector2`.
[Vector3](xref:GDX.Vector3Extensions) | Additional functionality for common calculations done with `Vector3`.

## Optimized Collections

Located in the `GDX.Collections` namespace is an extensive group of `struct`s and `class`es designed for performance-sensitive environments.

> [!WARNING]
> It is important to note that many of the structures backing data, indices, counts, etc., are publicly accessible. This is meant for advanced usage; change at runtime at your own risk.

Type | Description | Base
:--- | :--- | ---
Bit Arrays | A selection of sized [8](xref:GDX.Collections.BitArray8), [16](xref:GDX.Collections.BitArray16), [32](xref:GDX.Collections.BitArray32), [64](xref:GDX.Collections.BitArray64), [128](xref:GDX.Collections.BitArray128), [256](xref:GDX.Collections.BitArray256) and [512](xref:GDX.Collections.BitArray512) index accessed bit arrays. | `struct`
[Free List](xref:GDX.Collections.FreeList) | An array where indices are allocated from and stored in an in-place linked list. | `struct`
[Native Sparse Set](xref:GDX.Collections.NativeArraySparseSet) | A [Sparse Set](xref:GDX.Collections.NativeArraySparseSet), backed by two `NativeArray`. | `struct`
[Sparse Set](xref:GDX.Collections.SparseSet) | An adapter collection for external data arrays that allows constant-time insertion, deletion, and lookup by handle, as well as array-like iteration. | `struct`

### Generic

Type | Description | Base
:--- | :--- | ---
[2D Array](xref:GDX.Collections.Generic.Array2D`1) | A 2-dimension array backed by a singular array. | `struct`
[Circular Buffer](xref:GDX.Collections.Generic.CircularBuffer`1) | A sized buffer which loops back over itself as elements are filled. | `class`
[Native 2D Array](xref:GDX.Collections.Generic.NativeArray2D`1) | A 2-dimension `NativeArray` with a `xy` based accessor. | `struct`
[Native Uniform 3D Array](xref:GDX.Collections.Generic.NativeUniformArray3D`1) | A 3-dimension `NativeArray` where all axis are the same length. Numerous accessors available, including a `xyz` based. | `struct`
[Serializable Dictionary](xref:GDX.Collections.Generic.SerializableDictionary`2) | A dictionary that can be serialized by Unity into its native YAML format. Usage of the editor side property drawer **requires Unity 2020.1+**. | `class`
[Simple List](xref:GDX.Collections.Generic.SimpleList`1) | An optimized `List` replacement. | `struct`
[String Key Dictionary](xref:GDX.Collections.Generic.StringKeyDictionary`1) | An optimzed `Dictionary<string, T>` replacement. | `struct`

### Pooling

Type | Description | Base
:--- | :--- | ---
[Array Pool](xref:GDX.Collections.Pooling.ArrayPool`1) | An object pool for arrays with power-of-two lengths. | `struct`
[SimpleList Managed Pool](xref:GDX.Collections.Pooling.SimpleListManagedPool) | A basic implementation of an [IManagedPool](xref:GDX.Collections.Pooling.IManagedPool) backed by a `SimpleList`, usable by the [ManagedPoolBuilder](xref:GDX.Collections.Pooling.ManagedPoolBuilder).

## Helpful Utilities

Everyone loves static utility classes, and naturally `GDX` has a bunch of them to help fill out some commonly used functionality spread out across its namespace.

Utility | Description
:--- | :---
[Display](xref:GDX.Display) | Simplified querying of display capabilities.
[Localization](xref:GDX.Localization) | ISO language based formatting functionality.
[Memory](xref:GDX.Memory) | Specific functionality to ensure the heap is kept as small as possible.
[Platform](xref:GDX.Platform) | A bunch of Hardware and I/O related methods.
[Profiling](xref:GDX.Developer.Profiling) | A wrapper for Unity's builtin profiler with file management.
[Range](xref:GDX.Mathematics.Range) | Simplified logic for picking values within a range.
[Reflection](xref:GDX.Reflection) | Collection of methods to help with language level reflection.
[Rotate](xref:GDX.Mathematics.Rotate) | A set of functionality to extend on Unity's rotation based methods.
[Smooth](xref:GDX.Mathematics.Smooth) | Some fancy smoothing functions.

## Developer Toolbox

The missing or upgraded development process functionality.

Feature | Description
:--- | :---
[Build Info Writer](xref:GDX.Editor.Build.BuildInfoProvider) | A system which latches on to the build pipeline to write a configurable `BuildInfo` class containing a passed set of parameters from commandline arguments.
[Build Verification Report](xref:GDX.Developer.Reports.BuildVerificationReport) | A framework for build verification tests without the need for build-specific instrumentation and inclusion of additional libraries.
[Command Line Parser](xref:GDX.Developer.CommandLineParser) | A configurable command line parameter parsing system which can be queried at runtime (and in special cases during editor automation) for flags and key-value-pair data.
[Managed Resource Audit](xref:GDX.Developer.Reports.ResourcesAuditReport) | A reporting mechanism for outputting managed objects at runtime as well as  differences between captures.
[NUnit Report](xref:GDX.Developer.Reports.NUnitReport) | An abstraction of the NUnit report format allowing for generating of arbitrary reports that can be captured by different systems.
[Text Generator](xref:GDX.Developer.TextGenerator) | A text file generator allowing for easy formatting and unwinding.

## Miscellaneous

A few other notable inclusions which have been found useful.

Feature | Description
:--- | :---
[Coalesce Stream](xref:GDX.IO.CoalesceStream) | An infinitely sized `Stream`.
[Tarball Support](xref:GDX.IO.Compression.TarFile) | A native method to inflate `.tar` and `.tar.gz` files. Used internally in the `GDX` package to handle upgrades from [GitHub](https://github.com/dotBunny/GDX/releases).
[WELL1024a](xref:GDX.Mathematics.Random.WELL1024a) | A deterministic random number generator.
