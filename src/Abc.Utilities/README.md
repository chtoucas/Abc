Abc.Utilities
=============

[![MyGet](https://img.shields.io/myget/narvalo-edge/v/Abc.Utilities.Sources.svg)](https://www.myget.org/feed/narvalo-edge/package/nuget/Abc.Utilities.Sources)

MSBuild properties:
- `PkgAbc_Utilities_Sources` (even if GeneratePathProperty != true)
- `PkgAbc_Utilities_Sources__CompileRoot`
- `PkgAbc_Utilities_Sources__CompileItem`
- `PkgAbc_Utilities_Sources__ImportTfr`

Compiler symbols unless `PkgAbc_Utilities_Sources__ImportTfr` is set to `false`:
- `NETSTANDARD1_x`
- `NETSTANDARD2_x`
- `NETCOREAPP1_x`
- `NETCOREAPP2_x`
- `NETCOREAPP3_x`
- `NETCOREAPP5_x`

Only import what you really need
```xml
<ItemGroup>
  <PkgAbc_Utilities_Sources__CompileItem Include="MathOperations.g.cs" />
  <PkgAbc_Utilities_Sources__CompileItem Include="system\NullableAttributes.g.cs" />
</ItemGroup>
```

All static classes are partial. For instance:
```csharp
// In MathOperations.g.cs, we have:
namespace Abc.Utilities
{
    internal static partial class MathOperations
    {
        protected MathOperations() { }
    }
}

//
namespace Abc.Utilities
{
    internal partial class MathOperations
    {
        public static MyMethod() { }
    }
}
```

NRTs

Filter out unwanted code in code coverage:
- `/p:ExcludeByAttribute=DebuggerNonUserCode`
- `/p:[Abc.?*]System.*`.

Deterministic build

Pure, `CONTRACTS_FULL`
