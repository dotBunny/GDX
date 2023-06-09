# Data Tables

A robust implementation of a data table type similar to [Unreal Engines](https://docs.unrealengine.com/5.2/en-US/data-driven-gameplay-elements-in-unreal-engine/). _It's every game designers dream come true!_

> [!NOTE]
> This feature heavily relies on `Unity 2022.2` or newer functionality. The editing experience is disabled in older versions of Unity. The DataTableBase API is still valid for programmatic generation and manipulation.

## Column Types

Type coverage is very much related to how and what Unity will be able to serialize inside the backing `ScriptableObject`. Out of the box, you can use:

Values | Structs
| --- | --- |
| `string`, `char`, `bool`, `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` | `Vector2`, `Vector3`, `Vector4`,  `Vector2Int`,  `Vector3Int`, `Quaternion`, `Rect`, `RectInt`, `Color`, `LayerMask`, `Bounds`, `BoundsInt`, `Hash128`, `Gradient`, `AnimationCurve`

Usage of `Object` (`UnityEngine.Object`) and `EnumInt` types require additional information to be provided when creating a new column. A filter field will be visible when adding a column of these types asking for a fully qualified name of the desired type; this allows editing fields to isolate types.

## Creating

By default, a `StableDataTable` can be created from the Assets context menu or the application menu bar.

> Assets &rarr; Create &rarr; GDX &rarr; Stable Data Table

This utilizes the `CreateAssetMenu` attribute on the `DataTableBase` implementations. If you create an implementation, you must add your own attribute.

## Inspector

![Inspector](/images/manual/features/data-table/inspector.png)

A surface-level display of information about the `DataTableBase` is made available via the inspector. Names of rows and columns are listed, including the type of column.

Clicking the **Open** button will open an editing window focused on the selected `DataTableBase`.

### Interchange

The DataTableBase allows for interchange between formats, export and non-destructive table data import. What this means in practice is that you can export an already-built table and make changes in whatever editing program a user desires. Those changes can then be re-imported back into Unity, maintaining existing references (an example being an object column referencing prefabs).

> [!WARNING]
> It is important to note that the column structure (order and types) cannot change in the imported data. The row identifiers will be used to match up rows, updating where appropriate, creating when necessary, and removing when no longer found.

### Tracker

Table information is tracked in the editor via a reference counting mechanism. This information is helpful for developers to identify leaking references that were expected to be removed. This information is updated manually every time the inspector is drawn. While some table-driven events trigger a redraw, this number sometimes needs to be refreshed. The **Refresh** button at the top of the inspector will update the numbers.

## Table Window

![Table Window](/images/manual/features/data-table/window.png)

Bringing a spreadsheet-like editing experience to DataTables, this window allows for a designer or developer to quickly work on the structure or data of a DataTable in a familiar and expected experience. A unique-to-asset window will appear by double-clicking a DataTable asset or clicking the **Open** button while inspecting it.

### Table Menu

The first item in the menu bar covers table-specific operations and includes some of the functionality found in the individual inspectors (interchange operations, for example).

#### Write To Disk

A DataTable is backed by a ScriptableObject; changes reside in memory until they are committed to disk by saving the project and/or asset. This option will execute a `SaveAssetIfDirty` on the backing object writing the contents to disk.

> [!TIP]
> This option will only be available when changes are detected to the currently opened DataTable.

#### Settings

`DataTableBase`s have settings which outline the behaviour of the table as well as some nice-to-have features. The **Display Name** is a user-friendly name shown throughout the editor for this table. 

> The undo functionality is disabled by default, as it can be rather costly to execute across large datasets and is only available in Unity 2022.2 and newer.

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

## Referencing Cells

One of the design pillars for the DataTable feature was ensuring the data consumption was intuitive, both from a designer's standpoint and a developer's. A developer can create a cache-friendly reference to a cell by type:

```csharp
   public StringCellValue Name;
```

Then in the object's inspector, an easy-to-use property drawer is created. When unset, the user is presented with a dropdown of all available tables in the project to select from.

![Table Selection](/images/manual/features/data-table/drawer-select-table.png)

A new dropdown is generated with the row names to choose from upon selecting a table.

![Row Selection](/images/manual/features/data-table/drawer-select-row.png)

After selecting a row, the type-specific columns of data appear for selection.

![Column Selection](/images/manual/features/data-table/drawer-select-column.png)

After all that, the selected data is presented in a locked field. Clicking the lock icon will allow editing the data in the table directly (_seen below_). When Unity serializes this object, only the connection information is serialized.

![Data Linked](/images/manual/features/data-table/drawer-select-edit.png)

> [!TIP]
> Clicking the link icon at any time will break all connection information.