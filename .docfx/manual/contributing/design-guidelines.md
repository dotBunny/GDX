# Design Guidelines

There is a general effort to follow the [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/) set forth by the .NET team. While we **do not** always precisely adhere to them, they serve as a guiding principle.

## Coding Conventions
[.NET Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) is also a good point of reference.

## Unit Tests
We are trying to get as much coverage in tests as possible on the package to try and mitigate regressions. Please have a read of [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices).

> Arrange, Act, Assert

## Code Organization
- Typically sections of classes are ordered alphabetically.
- Preference to expose backing data, indices, etc.
- Don't throw exceptions; garbage in, garbage out.

## EditorConfig Support
- Embedded in the project is an [EditorConfig](https://editorconfig.org/), which should standardize much of the formatting.
    - It is based on the .NET Roslyn repositories `.editorconfig`.
    - Warns of non-explicit type definitions everywhere (we're not going to use var to promote better readability).