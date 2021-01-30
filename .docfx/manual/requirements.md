# Requirements
The `GDX` package can be dropped into any compatible [Unity](http://unity3d.com) project and selectively enables portions of its functionality based on what packages it finds in the project. `GDX` accomplishes this feat by utilizing a preprocessor setup that places functionality behind gated conditionals set by the assembly definitions.

### Unity Versions
The suggested minimum Unity version for using `GDX` is Unity `2019.1`.

`GDX` utilizes assembly definition version defines to gate feature sets and employs specific coding patterns that may not be available in previous versions of Unity.

### Supported Packages
Define | Package | Minimum Version
:--- | :--- | ---
GDX_ADDRESSABLES | com.unity.addressables | `1.16.0`
GDX_BURST | com.unity.burst | `1.4.0`
GDX_JOBS | com.unity.jobs | `0.6.9`
GDX_MATHEMATICS | com.unity.mathematics | `1.2.1`

It is essential to note that the GDX_* defines are only valid inside the `GDX` assemblies.