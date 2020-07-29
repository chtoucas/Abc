
## Automatic MSBuild properties

- `PkgAbc_Utilities_Sources__Content`
- `PkgAbc_Utilities_Sources__ContentCSharp`

There is also `PkgAbc_Utilities_Sources` if GeneratePathProperty = true.

## Compiler symbols

- `ABC_UTILITIES_ENABLE_CODE_COVERAGE`

## Selective imports

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

## Extensibility

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

## Miscs

### NRTs

### Code coverage

Filter out "System" classes: [Abc.Utilities]System.*

### Deterministic build
