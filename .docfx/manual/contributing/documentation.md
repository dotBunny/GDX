# Assisting with Documentation

> [!IMPORTANT]
> A midlevel developer given `GDX` should be able to pick it up and use it effectively just by reading the inline documentation.

A pretty ambitious goal; that should set the tone for what we are aiming to do when it comes to documentation.

## Local Generation

The documentation is statically generated using the [DocFx](https://dotnet.github.io/docfx/) project file located at `.docfx/docfx.json` with the help of an additional tool [Dox](https://github.com/dotBunny/GDX.Dox).

By downloading and building [Dox](https://github.com/dotBunny/GDX.Dox), _anyone_ can build and host locally the documentation for `GDX` with the following command:

```bash
Dox.exe --input <absolute path to gdx package folder>
```

> [!NOTE]
> Some of the documentation is generated through automation in TeamCity and as such, in locally generated situations stubs will be present. Some images, particularily to do with code coverage will not appear correctly.
