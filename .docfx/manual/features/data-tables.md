# Data Tables

![Data Tables](/images/manual/features/data-table/data-table-logo.png)

A robust implementation of a data table type similar to [Unreal Engines](https://docs.unrealengine.com/5.2/en-US/data-driven-gameplay-elements-in-unreal-engine/). _It's every game designers dream come true!_

> [!NOTE]
> This feature heavily relies on `Unity 2022.2` or newer functionality. The editing experience is disabled in older versions of Unity. The DataTableBase API is still valid for programmatic generation and manipulation.

## Column Types

Type coverage is very much related to how and what Unity will be able to serialize inside the backing `ScriptableObject`. Out of the box, you can use:

Values | Structs | Classes | Special
--- | --- | --- | ---
`string`, `char`, `bool`, `sbyte`, `byte`, `short`, `ushort`, `int`, `uint`, `long`, `ulong`, `float`, `double` | `Vector2`, `Vector3`, `Vector4`,  `Vector2Int`,  `Vector3Int`, `Quaternion`, `Rect`, `RectInt`, `Color`, `LayerMask`, `Bounds`, `BoundsInt` | `AnimationCurve`, `Gradient`, `Hash128`, `Object` | `EnumInt` |

Usage of `Object` (`UnityEngine.Object`) and `EnumInt` types require additional information to be provided when creating a new column. A filter field will be visible when adding a column of these types asking for a fully qualified name of the desired type; this allows editing fields to isolate types. As you type a picker will appear with `public` results from Unity's `TypeCache`.

## Creating

By default, a `StableDataTable` can be created from the Assets context menu or the application menu bar.

> Assets &rarr; Create &rarr; GDX &rarr; Stable Data Table

This utilizes the `CreateAssetMenu` attribute on the `DataTableBase` implementations. If you create an implementation, you must add your own attribute.

## Inspector

![Inspector](/images/manual/features/data-table/inspector.png)

A surface-level display of information about the `DataTableBase` is made available via the inspector. Names of rows and columns are listed, including the type of column.

Clicking the **Open** button will open an editing window focused on the selected `DataTableBase`.

### Interchange

The DataTableBase allows for data-interchange between formats ([CSV](https://en.wikipedia.org/wiki/Comma-separated_values) & [JSON](https://www.json.org/json-en.html) out-of-the-box), export and non-destructive table data import. What this means in practice is that you can export an already-built table and make changes in whatever editing program a user desires. Those changes can then be re-imported back into Unity, maintaining existing references (an example being an object column referencing prefabs).

> [!WARNING]
> It is important to note that the column structure (order and types) cannot change in the imported data. The row identifiers will be used to match up rows, updating where appropriate, creating when necessary, and removing when no longer found.

#### Excel / Google Sheets

Yes, absolutely!

However, you will need to do a bit of legwork to get there. When we ship `GDX` we try to think about a lean-and-mean approach. Adding the libraries necessary to interact with those types easily would add a certain level of bloat to the package. We have modelled our existing [JSON](xref:GDX.DataTables.DataBindings.JavaScriptObjectNotationFormat) and [CSV](xref:GDX.DataTables.DataBindings.CommaSeperatedValueFormat) formats to demonstrate how to build custom formats.

```csharp
   class ExcelFormat : FormatBase
   {      
      #if UNITY_EDITOR        
      [UnityEditor.InitializeOnLoadMethod]
      #else
      [UnityEngine.RuntimeInitializeOnLoadMethod]
      #endif        
      static void RegisterFormat()
      {
         // Ensure the format is registered for usage
         DataBindingProvider.RegisterFormat(new ExcelFormat());
      }

      // Fill out other overrides ...

      public override SerializableTable Pull(string uri, ulong currentDataVersion, int currentStructuralVersion)
      {                        
         // Open excel file with OpenXML SDK            
         // Read cells and put into new             
      }

      public override bool Push(string uri, SerializableTable serializableTable)
      {
         // Open excel file with OpenXML SDK
         // Itterate through simplified table data and put into cells
      }
   }
```

### Tracker

Table information is tracked in the editor via a reference counting mechanism. This information is helpful for developers to identify leaking references that were expected to be removed. This information is updated manually every time the inspector is drawn. While some table-driven events trigger a redraw, this number sometimes needs to be refreshed. The **Refresh** button at the top of the inspector will update the numbers.

## Table Window

![Table Window](/images/manual/features/data-table/window.png)

Bringing a spreadsheet-like editing experience to DataTables, this window allows for a designer or developer to quickly work on the structure or data of a DataTable in a familiar and expected experience. A unique-to-asset window will appear by double-clicking a DataTable asset or clicking the **Open** button while inspecting it.

> [!INFO]
> A lock icon may be on the toolbar's right side indicating that the Data Table has been put in **References Only* mode, allowing for editing of only Unity-specific references.

### Shortcuts

Some default shortcuts are bound to the Table Window, modelled after existing expected behaviours.

| Command | Shortcut |
| --- | --- |
| Add Row | Ctrl + = |
| Add Column | Ctrl + Shift + - |
| Add Row (Default) | Ctrl + Shift + = |
| Remove Selected Rows (Quick) | Shift + Del |
| Remove Selected Row | Del |

> [!TIP]
> Shortcuts can be changed in Unity's Shortcut manager, available in the Edit menu.

### Sorting & Ordering

Clicking on the column headers allows for sorting the rows (ascending/descending); holding CTRL while clicking a column header will allow for multi-levels of sorting to occur. You can also manually click and drag rows up and down to reorder them. This ordering information is virtually managed at this stage and does not get committed to the Data Table without user direction.

### Table Menu

The first item in the menu bar covers table-specific operations and includes some of the functionality found in the individual inspectors (interchange operations, for example).

#### Write To Disk

A DataTable is backed by a ScriptableObject; changes reside in memory until they are committed to disk by saving the project and/or asset. This option will execute a `SaveAssetIfDirty` on the backing object writing the contents to disk.

> [!TIP]
> This option will only be available when changes are detected to the currently opened DataTable.

#### Settings

Data Tables have settings which outline the behaviour of the table as well as some nice-to-have features. The **Display Name** is a user-friendly name shown throughout the editor for this table. 

> The undo functionality is disabled by default, as it can be rather costly to execute across large datasets and is only available in Unity 2022.2 and newer.

### Columns Menu

The general Columns menu gives minimal actions that can be taken without specifically selecting a column. Adding a new column, for example, is done from this menu. The **Resize To Fit** action will ensure that the displayed table fills the window's horizontal space without requiring scrolling. Column-specific actions are accessed by right-clicking a column header.

### Rows Menu

| Action | Description |
| --- | --- |
| Add | Presents a dialogue to add a row to the Data Table, asking for a user-friendly name for the row before proceeding. |
| Add (Default) | Adds a row to the Data Table with a default randomly generated name. |
| Rename Selected | Renaming the selected row will update the row's internal user-friendly name stored in the Data Table. |
| Remove Selected | Deletes the selected row, plain and simple. |
| Commit Order | Alters the internal ordering of rows in the Data Table to match the current order of rows in the Table Window. |
| Reset Order | Clears the virtual sorting information and restores the order in the Data Table. |

## Context Menu

Currently, only the column headers have a context menu allowing the user to perform specific operations on that column.

### Rename

Renaming the column will update the column's internal user-friendly name stored in the Data Table.

### Remove

The name is self-explanatory; however, as a precaution, this will request confirmation that you want to remove the column. Ensure that the name matches the column you intended to remove.

### Move Left / Right

While the order of rows in the table window is virtual (until committed), the order of columns is bound to the internal data table. These options will alter the column order one increment at a time.

## Referencing Cells

One of the design pillars for the DataTable feature was ensuring the data consumption was intuitive, both from a designer's standpoint and a developer's. A developer can create a cache-friendly reference to a cell by type:

```csharp
   public FloatCellValue BaseHealth;

   void Awake()
   {
      // Check data version and fill
      Health = BaseHealth.Get();
   }

   void OnRespawn()
   {
      // Reset without checking data version
      Health = BaseHealth.GetUnsafe();
   }
```

Then in the object's inspector, an easy-to-use property drawer is created. 

![Drawer Selection](/images/manual/features/data-table/drawer-process.gif)

When unset, the user is presented with a dropdown of all available tables in the project to select from. A new dropdown is generated with the row names to choose from upon selecting a table. After selecting a row, the type-specific columns of data appear for selection. After all that, the selected data is presented in a locked field. 

By default, values are locked from editing in this view.
Clicking the lock icon will allow editing the data in the table directly. When Unity serializes this object, only the connection information is serialized.

> [!TIP]
> Clicking the link icon at any time will break all connection information.

All `*CellValue` structs have `Get()` and `GetUnsafe()` methods which return the value stored in the linked Data Table. 

The safer `Get()` method checks the cached DataTable version number, updating the locally cached value before returning the cached value. The `GetUnsafe()` method returns the cached value without any checks; thus it is **required** that the first time a value is accessd, the `Get()` method is used. 

## Undo Support

In the DataTable settings, there is a toggle to turn on the Undo/Redo feature in Unity for a specific table. This will allow Unity's internal serialization system to move back and forth between changes made to the DataTable. However, this has a performance cost as the data serializes with each registered state.

> [!WARNING]
> This is very experimental as a feature; use it at your own risk.