[![GDX](https://dotbunny.com/wp-content/uploads/2021/01/gdx-logo-fun.png)](https://github.com/dotBunny/GDX)

[![Latest Release](https://img.shields.io/github/release/dotBunny/GDX.svg?logo=github)](https://github.com/dotBunny/GDX/releases)
[![OpenUPM](https://img.shields.io/npm/v/com.dotbunny.gdx?label=openupm&registry_uri=https://package.openupm.com)](https://openupm.com/packages/com.dotbunny.gdx/)
[![Discord Server](https://img.shields.io/discord/582190573897908224.svg?label=discord&logo=discord&color=informational)](https://discord.gg/EcceFGAuJs)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://github.com/dotBunny/GDX/blob/main/LICENSE)

Game Development Extensions, a battle-tested library of game-ready high-performance C# code.

Documentation available at [https://gdx.dotbunny.com/](https://gdx.dotbunny.com/).

## Usage
Add `com.dotbunny.gdx` as a dependency to the project `Packages/manifest.json` file:

```
{
  "dependencies": {
    "com.dotbunny.gdx": "https://github.com/dotBunny/GDX.git",
  }
}
```
> For a full breakdown of all installation methods please refer to the [Installation](https://gdx.dotbunny.com/manual/getting-started.html#installation) section of [Getting Started](https://gdx.dotbunny.com/manual/getting-started.html) in the [manual](https://gdx.dotbunny.com/manual/).

## Requirements
The package is designed to be compatible with an _empty project_ created in [Unity](http://unity3d.com).
It uses a preprocessor system, where the assembly definition will define features based on the packages found in the project.
> It is important to note that the GDX_* defines are only valid inside of the GDX assemblies.
### Supported Packages
Package | Minimum Version
:--- | ---
com.unity.addressables | `1.16.0`
com.unity.burst | `1.4.0`
com.unity.jobs | `0.6.9`
com.unity.mathematics | `1.2.1`

## Contributing
GDX is an open-source project and we encourage and welcome contributions.
### Design Guidelines
There is a general effort to follow the [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
set forth by the .NET team. While we **do not** always precisely adhere to them, they serve as a guiding principle.
- Embedded in the project is an [EditorConfig](https://editorconfig.org/), which should standardize much of the formatting.
    - It is based on the .NET Roslyn repositories `.editorconfig`.
    - Warns of non-explicit type definitions everywhere (we're not going to use var to promote better readability).
- [.NET Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) is also a good point of reference.
- Typically sections of classes are ordered alphabetically.
- Preference to expose backing data, indices, etc.

## License
GDX is licensed under the [MIT License](https://choosealicense.com/licenses/mit/).
A copy of this license can be found at the root of the project in the `LICENSE` file.
