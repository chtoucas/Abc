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

### Only import what you really need

```xml
<Import Project="$(PkgAbc_Utilities_Sources__Content)NoContent.targets" />

<ItemGroup>
  <Compile Include="$(PkgAbc_Utilities_Sources__ContentCSharp)ExceptionFactory.cs">
    <Link>Utilities\ExceptionFactory.cs</Link>
  </Compile>
  <Compile Include="$(PkgAbc_Utilities_Sources__ContentCSharp)MathOperations.cs">
    <Link>Utilities\MathOperations.cs</Link>
  </Compile>
  <Compile Include="$(PkgAbc_Utilities_Sources__ContentCSharp)system\NullableAttributes.cs">
    <Visible>false</Visible>
  </Compile>
</ItemGroup>
```

### Extensibility

Instead of static classes, we use abstract partial classes. For instance:
```csharp
//
namespace Abc.Utilities
{
    [DebuggerNonUserCode, _ExcludeFromCodeCoverage]
    internal abstract partial class MathOperations
    {
        protected MathOperations() { }
    }
}
```

```csharp
namespace MyNamespace
{
    internal sealed class MyMathOperations : MathOperations
    {
        public static MyMethod() { }
    }
}
```

### Notes

All classes carry are marked with the attributes `DebuggerNonUserCode`
and `_ExcludeFromCodeCoverage`.

#### NRTs
All files start with `#nullable enable`.

#### Code coverage
Filter out unwanted classes:
- `/p:ExcludeByAttribute=_ExcludeFromCodeCoverage`
- `/p:[MyAssembly]System.*`.

#### Deterministic build

Changelog
---------
