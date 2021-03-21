# Requirements
The `GDX` package can be dropped into any compatible [Unity](http://unity3d.com) project and selectively enables portions of its functionality based on what packages it finds in the project. `GDX` accomplishes this feat by utilizing a preprocessor setup that places functionality behind gated conditionals set by the assembly definition.

### Unity Versions
The suggested minimum Unity version for using `GDX` is Unity `2020.3`. 

Compatibility is automatically tested against Unity's **L**ong **T**erm **S**upport releases `2018.4`, `2019.4` and `2020.3`.

`GDX` utilizes assembly definition version defines to gate feature sets and employs specific coding patterns that may not be available in previous versions of Unity.

### Supported Packages
We try to make sure we are compatible with the latest verified release of a package across all supported versions of Unity where possible.  For example, the `com.unity.platforms` package is only available in Unity `2020.1` and newer.

Define | Package | Minimum Unity Version | Minimum Package Version
:--- | :--- | ---
GDX_ADDRESSABLES | com.unity.addressables | 2018.4 | `1.16.16`
GDX_BURST | com.unity.burst | 2018.4 | `1.4.6`
GDX_MATHEMATICS | com.unity.mathematics | 2018.4 | `1.2.1`
GDX_PLATFORMS | com.unity.platforms | 2020.3 |`0.11.0-preview.17`

> [!NOTE]
> It is essential to remember that the GDX_* defines are only valid inside the `GDX` assembly, see [Conditionals](xref:GDX.Developer.Conditionals) if you wish to access some semblance outside of `GDX`.