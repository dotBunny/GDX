# Requirements

The `GDX` package can be dropped into any compatible [Unity](http://unity3d.com) project and selectively enables portions of its functionality based on what packages it finds in the project. `GDX` accomplishes this feat by utilizing a preprocessor setup that places functionality behind gated conditionals set by the assembly definition.

## Unity Versions

The suggested minimum Unity version for using `GDX` is Unity `2022.2`; internal development is done on `2022.3`.

Compatibility is automatically tested against Unity's **L**ong **T**erm **S**upport releases `2020.3`,`2021.3` and `2022.3`, as well as the current `2023.2` release. While we do our best to support features across all versions of Unity, there are cases where we are unable to support older legacy LTS versions of Unity.

`GDX` utilizes assembly definition version defines to gate feature sets and employs specific coding patterns that may not be available in previous versions of Unity.

## Supported Packages

We try to make sure we are compatible with the latest verified release of a package across all supported versions of Unity where possible.

Define | Package | Minimum Unity Version | Minimum Package Version
:--- | :--- | :--- | ---
GDX_ADDRESSABLES | com.unity.addressables | _n/a_ | `1.18.19`
GDX_BURST | com.unity.burst | _n/a_ | `1.6.4`
GDX_MATHEMATICS | com.unity.mathematics | _n/a_ | `1.2.5`
GDX_PLATFORMS | com.unity.platforms | _n/a_ | `0.9.0`
GDX_VISUALSCRIPTING | com.unity.visual-scripting | 2021.1 |`1.5.2`
GDX_COLLECTIONS | com.unity.collections | _n/a_ | `1.2.3`

> [!NOTE]
> It is essential to remember that the GDX_* defines are only valid inside the `GDX` assembly, see [Conditionals](xref:GDX.Developer.Conditionals) if you wish to access some semblance outside of `GDX`.

## Portability

[Automation](/manual/contributing/automation.html) continually checks the portability of the code used in `GDX` to ensure the compatibility across various platforms generating a [report](/reports/portability.html).
