Abc.Utilities
=============

[![MyGet](https://img.shields.io/myget/narvalo-edge/v/Abc.Utilities.Sources.svg)](https://www.myget.org/feed/narvalo-edge/package/nuget/Abc.Utilities.Sources)

- [Usage](#usage)
- [Changelog](#changelog)

Usage
-----

### MSBuild properties

- `PkgAbc_Utilities_Sources__Content`
- `PkgAbc_Utilities_Sources__ContentCSharp`

There is also `PkgAbc_Utilities_Sources` if GeneratePathProperty = true.

### Compiler symbols

`CONTRACTS_FULL`

### Only import what you really need

```xml
<Import Project="$(PkgAbc_Utilities_Sources__Content)NoContent.targets" />

<ItemGroup>
  <Compile Include="$(PkgAbc_Utilities_Sources__ContentCSharp)ExceptionFactory.g.cs">
    <Link>Utilities\ExceptionFactory.g.cs</Link>
  </Compile>
  <Compile Include="$(PkgAbc_Utilities_Sources__ContentCSharp)MathOperations.g.cs">
    <Link>Utilities\MathOperations.g.cs</Link>
  </Compile>
  <Compile Include="$(PkgAbc_Utilities_Sources__ContentCSharp)system\NullableAttributes.g.cs">
    <Visible>false</Visible>
  </Compile>
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
