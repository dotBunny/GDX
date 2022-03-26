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

## Pragmas
WIP

### Compiler
|Code|Description|Level|
|:--|---|:--|
| [CS0168](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0168) |The variable 'var' is declared but never used. | 3 |
| [CS0169](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0169) | The private field 'class member' is never used. | 3 |
| [CS0414](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0414) | The private field 'field' is assigned but its value is never used. | 3 |
| [CS0429](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/compiler-messages/cs0429) | Unreachable expression code detected. | 4 |
| [CS0649](https://docs.microsoft.com/en-us/dotnet/csharp/misc/cs0649) | Field 'field' is never assigned to, and will always have its default value 'value'. | 4 |

### Mappings 
| Pragma | Resharper | Description|
|:--|:--|---|
| [IDE0052](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0052) | UnusedMember.Local | Remove unread private member. |
| [IDE0090](https://docs.microsoft.com/en-us/dotnet/fundamentals/code-analysis/style-rules/ide0090) | ArrangeObjectCreationWhenTypeEvident | Simplify new expression. |

## EditorConfig Support
- Embedded in the project is an [EditorConfig](https://editorconfig.org/), which should standardize much of the formatting.
    - It is based on the .NET Roslyn repositories `.editorconfig`.
    - Warns of non-explicit type definitions everywhere (we're not going to use var to promote better readability).