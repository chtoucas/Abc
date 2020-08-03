Abc.Utilities
=============

[![MyGet](https://img.shields.io/myget/narvalo-edge/v/Abc.Utilities.Sources.svg)](https://www.myget.org/feed/narvalo-edge/package/nuget/Abc.Utilities.Sources)

- [Usage](#usage)
- [Changelog](#changelog)

Usage
-----

### MSBuild properties

- `PkgAbc_Utilities_Sources` even if GeneratePathProperty != true
- `PkgAbc_Utilities_Sources__CSharpFiles`

### Compiler symbols

`CONTRACTS_FULL`

### Only import what you really need

```xml
<ItemGroup>
  <PkgAbc_Utilities_Sources__Compile Include="MathOperations.g.cs" />
  <PkgAbc_Utilities_Sources__Compile Include="system\NullableAttributes.g.cs" />
</ItemGroup>
```

### Extensibility

All static classes are partial. For instance:
```csharp
//
namespace Abc.Utilities
{
    internal static partial class MathOperations
    {
        protected MathOperations() { }
    }
}
```

```csharp
namespace Abc.Utilities
{
    internal partial class MathOperations
    {
        public static MyMethod() { }
    }
}
```

### Notes

#### NRTs

#### Code coverage
Filter out unwanted code in code coverage:
- `/p:ExcludeByAttribute=DebuggerNonUserCode`
- `/p:[Abc.?*]System.*`.

#### Deterministic build

Changelog
---------
