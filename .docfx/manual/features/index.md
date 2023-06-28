---
uid: features
---
# Features

Over time `GDX` has grown into a robust set of functionality; with that comes the much larger problem of discoverability. There are too many things to cover in a list on this page (*hence we removed it*). We are working on this problem by building key *feature* sub-pages to highlight functionality; thus, this page only encompasses part of the package. 

> [!TIP]
> For a more thorough list of functionality, explore the [API documentation](/api/GDX.html) directly.

Looking at the GDX runtime namespace, we can extract highlights from each area to get you thinking. Not going to dive into the authoring namespace; for now.

## [GDX](xref:GDX)

The root of all ~~evil~~ awesomeness? Numerous extension classes live at the core, matching and extending Unity's types. We also have a standard set of utilities that live here as well.

Highlight | Description
:-- | :--
[ArrayExtensions](xref:GDX.ArrayExtensions) | A bunch of `Array` operations which are commonly used by other methods. For example getting the last or first index of an item or value in an `Array`.
[EnumExtensions](xref:GDX.EnumExtensions) | Useful functionality around `Flags`
[IList](xref:GDX.IListExtensions) | A common set of methods for `IList` implementing collections allowing for optimized value and item checks, as well as some extras.
[PlayerLoopSystemExtensions](xref:GDX.PlayerLoopSystemExtensions) | A helpful set of tools for manipulating Unity's `PlayerLoop`.
[Reflection](xref:GDX.Reflection) | It makes reflection a little less painful to utilize; it still has the same performance problems.
[String](xref:GDX.StringExtensions) | A vast collection of extension methods covering a wide range from encryption to hash codes, to simply finding content an existing `string`.
[Trace](xref:GDX.Trace) | Our take on categorized cross-platform logging.

## [GDX.Collections](xref:GDX.Collections)

We've found that the collections that ship with Unity and .Net don't live up to our desired performance characteristics. `GDX` provides various collection types to meet the performance criteria needs of demanding environments. 

> [!WARNING]
> It is important to note that many structures backing data, indices, counts, etc., are publicly accessible for advanced usage. Change at runtime at your own risk.

Highlight | Description
:-- | :--
[FreeList](xref:GDX.Collections.FreeList) | An array where indices are allocated from and stored in an in-place linked list.
[IntKeyDictionary](xref:GDX.Collections.Generic.IntKeyDictionary`1) | An optimzed `Dictionary<int, T>` replacement.
[SerializableDictionary](xref:GDX.Collections.Generic.SerializableDictionary`2) | A dictionary that can be serialized by Unity into its native YAML format. Usage of the editor side property drawer **requires Unity 2020.1+**.
[SimpleList](xref:GDX.Collections.Generic.SimpleList`1) | An optimized `List` replacement.
[StringKeyDictionary](xref:GDX.Collections.Generic.StringKeyDictionary`1) | An optimzed `Dictionary<string, T>` replacement.
[SparseSet](xref:GDX.Collections.SparseSet) | An adapter collection for external data arrays that allows constant-time insertion, deletion, and lookup by handle, as well as array-like iteration.

## [GDX.DataTables](xref:GDX.DataTables)

A game designer's dream comes true for Unity, plain and simple. We've built an extensive system allowing an expected workflow to fit in with the traditional Unity development pipeline. While we don't ship directly with a Microsoft Excel or Google Sheets integration, the format framework is extensible and can easily have any data source.

> [!INFO]
> We are looking into ways to ship Excel support out of the box, but you have to create the new Format for now (*hint: use [OpenXML](https://github.com/dotnet/Open-XML-SDK)*).

## [GDX.Developer](xref:GDX.Developer)

A toolbox of various functionality that developers tend to rely on while making a game but should not show up in a gold master build; however often does.

Highlight | Description
:-- | :--
[InputProxy](xref:GDX.Developer.InputProxy) | A way to simulate hardware input on windows based machines, think automated testing.
[ResourceAuditReport](xref:GDX.Developer.Reports.ResourcesAuditReport) | A reporting mechanism for outputting managed objects at runtime as well as  differences between captures.
[TextGenerator](xref:GDX.Developer.TextGenerator) | A text file generator allowing for easy formatting and unwinding.


## [GDX.Experimental](xref:GDX.Experimental)

While the purpose of `GDX` is to provide battle-tested code to developers, this section of the namespace can be considered the battleground where that happens with games in development. Often we work alongside other developers to create new features for `GDX`; this is where they live and get tested. The API is bound to change, and there are bound to be bugs, yet there is nothing but glory for the adventuresome contributors.

Highlight | Description
:-- | :--
DebugDraw | 

## [GDX.IO](xref:GDX.IO)

Not the most glorious chunks of code, we fill a gap of some commonly expected operations with different file streams or, more importantly, what to do with massive files.

## [GDX.Jobs](xref:GDX.Jobs)

We have found a small but mighty group of jobs we repeatedly reuse when building out systems in Unity. While small, they are still titans in their ways.

## [GDX.Mathematics](xref:GDX.Mathematics)

A series of helper functionality used throughout GDX itself. They are excellent standalone additions when it comes to everyday math operations. Nothing glorious; they provide solutions to repetitively created functions.

Highlight | Description
:-- | :--
[WELL1024a](xref:GDX.Mathematics.Random.WELL1024a) | A deterministic random number generator.

## [GDX.Rendering](xref:GDX.Rendering)

Just ignore this one; honestly, it's got nothing of value outside of a tiny bit of functionality we rely on for other parts of the package. What? We're telling you the truth :)

## [GDX.Threading](xref:GDX.Threading)

Optimizing for concurrency and utilization is complex; GDX provides some exciting systems to move stuff off of the main thread while respecting Unity's main-thread operation. It took a while to bring some of this functionality to GDX; however, it played a heavy role in the initial reasoning for creating GDX as we kept having to make similar systems for numerous titles.

Highlight | Description
:-- | :--
TaskDirector |
WaitFor |
WaitWhile |