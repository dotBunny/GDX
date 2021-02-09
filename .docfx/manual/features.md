---
uid: features
---
# Features
This is an overview of *some of* the notable features contained within the `GDX` package. For a more thorough list of functionality make sure to explore the [API documentation](/api/index.html) directly.

## Extensions
A set of functionality, aggressively  inlined where appropriate to augment workflows with existing or provided types and collections.

> [!NOTE]
> There are some extensions based on `GDX` types which are not listed below, but are included in the package.	

Feature | Description
:--- | :---
[Addressables](xref:GDX.AddressablesExtensions) | A collection of useful spawning and management functionality built on top of the `com.unity.addressables` package. For example identifying if a `AssetReference` is valid and able to be instantiated.
[Array](xref:GDX.ArrayExtensions) | A bunch of `Array` operations which are commonly used by other methods. For example getting the last or first index of an item or value in an `Array`.
[BoxCollider](xref:GDX.BoxColliderExtensions) | Find out if a position is contained within a `BoxCollider`.
[Byte](xref:GDX.ByteExtensions) | Contains a simple non-allocating extension method to get a stable hash code from an array of `bytes`, maybe more someday.
[CapsuleCollider](xref:GDX.CapsuleColliderExtensions) | Simple helper for finding out which way a `CapsuleCollider` is oriented.
[GameObject](xref:GDX.GameObjectExtensions) | After this long, even Unity can use a little help with some of its fundamental component based systems.
[IList](xref:GDX.IListExtensions) | A common set of methods for collections allowing for optimized value and item checks, as well as some extras.
[NavMeshPath](xref:GDX.NavMeshPathExtensions) | Some helpful functions when trying to build out AI logic.
[Mesh](xref:GDX.MeshExtensions) | A one of helper to calculate the volume of a mesh.
[Rigidbody](xref:GDX.RigidbodyExtensions) | A method for determining the moment of inertia.
[String](xref:GDX.StringExtensions) | The thing that started it all, a vast collection of extension methods covering a wide range from encryption to hash codes, to simply finding content an existing `string`.
[Transform](xref:GDX.TransformExtensions) | Some useful functionality when it comes to working with `Transform`.
[Vector2](xref:GDX.Vector2Extensions) | Additional functionality for common calculations done with `Vector2`. For example, determining approximately if two `Vector2` are the same efficiently, or finding the midpoint between two `Vector2`. This automatically switches to using `com.unity.mathematics` functionality if available.
[Vector3](xref:GDX.Vector3Extensions) | Additional functionality for common calculations done with `Vector3`. For example, determining approximately if two `Vector3` are the same efficiently, or finding the midpoint between two `Vector3`. This automatically switches to using `com.unity.mathematics` functionality if available.
 

## Types & Collections
An extensive group of `struct` and `class` designed with performance-sensitive environments in mind. Useful to avoid reinventing the wheel over and over again!

> [!WARNING]
> It's important to note that many of the structures backing data, indices, counts, etc., are publicly accessible. This is meant for advanced usage, change at runtime at your own risk. 

Feature | Description | Type
:--- | :--- | ---
Bit Arrays | A selection of sized [8](xref:GDX.Collections.BitArray8), [16](xref:GDX.Collections.BitArray16), [32](xref:GDX.Collections.BitArray32), [64](xref:GDX.Collections.BitArray64), [128](xref:GDX.Collections.BitArray128), [256](xref:GDX.Collections.BitArray256) and [512](xref:GDX.Collections.BitArray512) index accessed bit arrays. | `struct`
[Byte2](xref:GDX.Mathematics.Byte2) | A fully implemented `byte` vector. | `struct`
[Free List](xref:GDX.Collections.FreeList) | An array where indices are allocated from and stored in an in-place linked list. | `struct`
[Native Sparse Set](xref:GDX.Collections.NativeSparseSet) | A [Sparse Set](xref:GDX.Collections.NativeSparseSet), backed by two `NativeArray`. | `struct`
[Sparse Set](xref:GDX.Collections.NativeSparseSet) | An adapter collection for external data arrays that allows constant-time insertion, deletion, and lookup by handle, as well as array-like iteration. | `struct`

### Generics
Feature | Description | Type
:--- | :--- | ---
[Circular Buffer](xref:GDX.Collections.Generic.CircularBuffer`1) | A sized buffer which loops back over itself as elements are filled. | `class`
[Native 2D Array](xref:GDX.Collections.Generic.NativeArray2D`1) | A 2-dimension `NativeArray` with a `xy` based accessor. | `struct`
[Native Simple List](xref:GDX.Collections.Generic.NativeSimpleList`1) | A [SimpleList](xref:GDX.Collections.Generic.SimpleList`1), backed by a `NativeArray`. | `struct`
[Native Simple Queue](xref:GDX.Collections.Generic.NativeSimpleQueue`1) | A simple queue, backed by a `NativeArray`. | `struct`
[Native Uniform 3D Array](xref:GDX.Collections.Generic.NativeUniformArray3D`1) | A 3-dimension `NativeArray` where all axis are the same length. Numerous accessors available, including a `xyz` based. | `struct`
[SerializableDictionary](xref:GDX.Collections.Generic.SerializableDictionary`2) | A dictionary that can be serialized by Unity into its native YAML format. Usage of the editor side property drawer **requires Unity 2020.1+**. | `class`

## Utilities
Everyone loves static utility classes, and naturally `GDX` has a bunch of them to help fill out some commonly used functionality.

Feature | Description
:--- | :---
[Display](xref:GDX.Display) | Simplified querying of display capabilities.
[Localization](xref:GDX.Localization) | ISO language based formatting functionality.
[Memory](xref:GDX.Memory) | Specific functionality to ensure the heap is kept as small as possible.
[Platform](xref:GDX.Platform) | A bunch of Hardware and I/O related methods.
[Smooth](xref:GDX.Mathematics.Smooth) | Some fancy smoothing functions.


## Developer Tools
The missing or upgraded development process functionality.

Feature | Description
:--- | :---
[Build Info Writer](xref:GDX.Editor.Build.BuildInfoProvider) | A system which latches on to the build pipeline to write a configurable `BuildInfo` class containing a passed set of parameters from commandline arguments.
[Command Line Parser](xref:GDX.Developer.CommandLineParser) | A configurable command line parameter parsing system which can be queried at runtime (and in special cases during editor automation) for flags and key-value-pair data.

## Miscellaneous
A few other notable inclusions which have been found useful.

Feature | Description
:--- | :---
[Mersenne Twister](xref:GDX.Mathematics.Random.MersenneTwister) | A deterministic random number generator.
[Tarball Support](xref:GDX.IO.Compression.TarFile) | A native method to inflate `.tar` and `.tar.gz` files. Used internally in the `GDX` package to handle upgrades from [GitHub](https://github.com/dotBunny/GDX/releases).
