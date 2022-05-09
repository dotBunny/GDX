# Design Guidelines

There is a general effort to follow the [Framework Design Guidelines](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/) set forth by the .NET team. While we **do not** always precisely adhere to them, they serve as a guiding principle. [.NET Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions) is also a good point of reference from a conventions standpoint.

## Coding Style

There are some basic pillars we have when thinking about `GDX` code:

- Expose backing data, indices, etc.
- Don't throw exceptions; garbage in, garbage out.
- Always prefer to use structs over classes.
- Order alphabetically where appropritate.

### Naming Convensions

Just like every developer in history, the hardest part of our jobs is naming conventions. There are some basic conventions that we make use of that make naming ever so slightly easier.

Suffix | Description
--- | :--
Extensions | A reserved name for base classes in GDX which build upon a concept.
Generator | Something which produces consumable content. This varies in degree from things like file content to UI panels.
Provider | These are almost like service wrappers, where we use the word service lightly.

### EditorConfig Support

Embedded in the project is an [EditorConfig](https://editorconfig.org/), which should standardize much of the formatting.

- It is based on the .NET Roslyn repositories `.editorconfig`.
- Don't use anything that would break Unity 2020.3 (hybrid C# 8)
- Warns of non-explicit type definitions everywhere (we're not going to use var to promote better readability).
- Includes some additioanl inspection rules for use with `ReSharper` based inspection.

## Unit Tests

We are trying to get as much coverage in tests as possible on the package to try and mitigate regressions. Please have a read of [Unit Testing Best Practices](https://docs.microsoft.com/en-us/dotnet/core/testing/unit-testing-best-practices).

> Arrange, Act, Assert

## Static Analysis

We strive to not add to the problem of noise in code inspection. We try our best to markup `GDX` code in a manner which will let code analyzers know specifically that we have made choices about the way something is written and that it is by design.

We also understand that scenarios come up where the naming conventions need a little bit of breathing room. The following appears throughout the codebase to temporarily resolve warnings which should be muted in that case.

````csharp
#pragma warning disable once IDE1006
    // ReSharper disable once InconsistentNaming
````

This is not an isolated instance, and more complex scenarios exist.

```csharp
#pragma warning disable IDE0052, IDE0090
        // ReSharper disable UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
        static readonly CoreSentinel k_DisposeSentinel = new CoreSentinel();
        // ReSharper restore UnusedMember.Local, ArrangeObjectCreationWhenTypeEvident
#pragma warning restore IDE0052, IDE0090
```

In each spot we try to craft a minimal set of exclusions that most static analysis will be able to understand. It is important to match `pragma` and `ReSharper` markup.

### Ignore Codes

Here is a collection of linked `pragma` to `ReSharper` representations that appear throughout the `GDX` codebase.

#### Compiler Warnings

|Code|Description|Level|
|:--|---|:--|
| [CS0168](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0168) |The variable 'var' is declared but never used. | 3 |
| [CS0169](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0169) | The private field 'class member' is never used. | 3 |
| [CS0414](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0414) | The private field 'field' is assigned but its value is never used. | 3 |
| [CS0429](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0429) | Unreachable expression code detected. | 4 |
| [CS0649](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0649) | Field 'field' is never assigned to, and will always have its default value 'value'. | 4 |

#### Code Style

| Pragma | Resharper | Description|
|:--|:--|---|
| [IDE1006](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide1006) | InconsistentNaming | Naming rule violation. |
| [IDE0051](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0052) | ?  | Remove unused private member. |
| [IDE0052](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0052) | UnusedMember.Local | Remove unread private member. |
| [IDE0090](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0090) | ArrangeObjectCreationWhenTypeEvident | Simplify new expression. |