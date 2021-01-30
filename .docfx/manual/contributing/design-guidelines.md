There is a general effort to follow the [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/)
set forth by the .NET team. While we **do not** always precisely adhere to them, they serve as a guiding principle.
- Embedded in the project is an [EditorConfig](https://editorconfig.org/), which should standardize much of the formatting.
    - It is based on the .NET Roslyn repositories `.editorconfig`.
    - Warns of non-explicit type definitions everywhere (we're not going to use var to promote better readability).
- [.NET Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) is also a good point of reference.
- Typically sections of classes are ordered alphabetically.
- Preference to expose backing data, indices, etc.