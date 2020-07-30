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

Optional properties.
- `PkgAbc_Utilities_Sources__EnableCodeCoverage`

There is also `PkgAbc_Utilities_Sources` if GeneratePathProperty = true.

### Compiler symbols

- `ABC_UTILITIES_ENABLE_CODE_COVERAGE`

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
    [DebuggerNonUserCode]
    internal abstract partial class MathOperations
    {
        [ExcludeFromCodeCoverage]
        protected MathOperations() { }
    }
}
```

```csharp
// First option.
namespace Abc.Utilities
{
    internal partial class MathOperations
    {
        public static MyMethod() { }
    }
}

// Second option.
namespace MyNamespace
{
    internal sealed class MyMathOperations : MathOperations
    {
        public static MyMethod() { }
    }
}
```

### Notes

All classes carry are marked with the attribute `DebuggerNonUserCode`.

#### NRTs
All files start with `#nullable enable`.

#### Code coverage
Unless `ABC_UTILITIES_ENABLE_CODE_COVERAGE` is defined, all codes are
marked with the attribute `ExcludeFromCodeCoverage`.
Filter out "System" classes: `[Abc.Utilities]System.*`.

#### Deterministic build

Changelog
---------
