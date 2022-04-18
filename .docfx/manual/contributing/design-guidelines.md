# Design Guidelines

There is a general effort to follow the [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/) set forth by the .NET team. While we **do not** always precisely adhere to them, they serve as a guiding principle.

## Coding Conventions

[.NET Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) is also a good point of reference.

## Unit Tests

We are trying to get as much coverage in tests as possible on the package to try and mitigate regressions. Please have a read of [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices).

> Arrange, Act, Assert

## Coding Style

- Preference to expose backing data, indices, etc.
- Don't throw exceptions; garbage in, garbage out.
- Always prefer to use structs over classes.

## Code Organization

- Typically sections of classes are ordered alphabetically.

## Code Inspection

We strive to not add to the problem of noise in code inspection, thus we try our best to markup our code in a manner which will let code analyzers know specifically that we have made choices about the way something is written and that it is by design.

For example, in the `GDX.Core` we deploy a pattern to create a dispose sentinal pattern allowing for a destruction like behaviour to happen with a static class.

```csharp
#pragma warning disable IDE0052, IDE0090
        // ReSharper disable UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
        static readonly CoreSentinel k_DisposeSentinel = new CoreSentinel();
        // ReSharper restore UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
#pragma warning restore IDE0052, IDE0090
```

Below is a list and explanation of some of the markup used in `GDX`.

### Compiler Warnings

|Code|Description|Level|
|:--|---|:--|
| [CS0168](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0168) |The variable 'var' is declared but never used. | 3 |
| [CS0169](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0169) | The private field 'class member' is never used. | 3 |
| [CS0414](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0414) | The private field 'field' is assigned but its value is never used. | 3 |
| [CS0429](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0429) | Unreachable expression code detected. | 4 |
| [CS0649](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0649) | Field 'field' is never assigned to, and will always have its default value 'value'. | 4 |
### Code Quality

### Code Style

| Pragma | Resharper | Description|
|:--|:--|---|
| [IDE1006](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide1006) | InconsistentNaming | Naming rule violation. |
| [IDE0051](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0052) | ?  | Remove unused private member. |
| [IDE0052](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0052) | UnusedMember.Local | Remove unread private member. |
| [IDE0090](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0090) | ArrangeObjectCreationWhenTypeEvident | Simplify new expression. |

## EditorConfig Support

- Embedded in the project is an [EditorConfig](https://editorconfig.org/), which should standardize much of the formatting.
  - It is based on the .NET Roslyn repositories `.editorconfig`.
  - Don't use anything that would break Unity 2020.3 (hybrid C# 8)
  - Warns of non-explicit type definitions everywhere (we're not going to use var to promote better readability).

## Naming Convensions

Just like every developer in history, the hardest part of our jobs is naming conventions. There are some basic conventions taht we try to use which make naming ever so slightly easier.

| Suffix | Description |
| --- | :-- |
| Extensions | A reserved name for base classes in GDX which build upon a concenpt. TBD |
| Generator | Something which produces consumable content. This varies in degree from things like file content to UI panels. These are meant to be single time usages ?|
| Provider | TBD |
