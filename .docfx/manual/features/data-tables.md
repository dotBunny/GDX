# Data Tables

TODO: Add stuff here!


## Column Types

Type coverage is very much related to how and what Unity will be able to serialize inside the backing `ScriptableObject`. Out of the box, you can use:

Values | Structs
| --- | --- |
| `string`, `char`, `bool`, `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` | `Vector2`, `Vector3`, `Vector4`,  `Vector2Int`,  `Vector3Int`, `Quaternion`, `Rect`, `RectInt`, `Color`, `LayerMask`, `Bounds`, `BoundsInt`, `Hash128`, `Gradient`, `AnimationCurve` | `UnityEngine.Object`

Usage of `Object` (`UnityEngine.Object`) and `EnumInt` types require additional information to be provided when creating a new column. A filter field will be visible when adding a column of these types asking for a fully qualified name of the desired type; this allows editing fields to isolate types.

## Creating

By default, a `StableDataTable` can be created from the Assets context menu or the application menu bar.

> Assets &rarr; Create &rarr; GDX &rarr; Stable Data Table

This utilizes the `CreateAssetMenu` attribute on the `DataTableBase` implementations. If you create an implementation, you must add your own attribute.

## Inspector

![Inspector](/images/manual/features/data-table/inspector.png)

### Rows
### Columns
### Interchange
### Tracker



## Table Window

![Table Window](/images/manual/features/data-table/window.png)

Bringing a spreadsheet-like editing experience to DataTables, this window allows for a designer or developer to quickly work on the structure or data of a DataTable in a familiar and expected experience. A unique-to-asset window will appear by double-clicking a DataTable asset or clicking the **Open Table** button while inspecting it.

### Table Menu
#### Write To Disk

A DataTable is backed by a ScriptableObject; changes reside in memory until they are committed to disk by saving the project and/or asset. This option will execute a `SaveAssetIfDirty` on the backing object writing the contents to disk.

> [!NOTE]
> This option will only be available when changes are detected to the currently opened DataTable.

#### Export To CSV
#### Import From CSV
#### Settings

### Columns Menu
#### Add
#### Resize To Fit

### Rows Menu
#### Add
#### Add (Default)
#### Rename Selected
#### Remove Selected
#### Commit Order
#### Reset Order

## Context Menu

Currently, only the column headers have a context menu allowing the user to perform specific operations on that column.

### Resize To Fit
### Rename
### Remove
### Move Left
### Move Right
### Reset Order



# Undo
Undo is only supported in 2022.2 and newer