# Developer Notes

vNEXT
-----

Version 1.0.0-alpha-2
- Remove async methods.

Towards version 1.0.0
- Add overloads w/ `IEqualityComparer<T>`? Ops `Maybe<T> == T`?
- Simplify project layout (move part of "play" to a separate project).
- Test NuGet package.
- Multitargeting:
  * Tests.
  * How to handle PublicAPI.XXX.txt?
  * Clean up `CustomAfter.props`, [Compiler symbols](https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries)
  * [NetStandardImplicitPackageVersion](https://docs.microsoft.com/en-us/dotnet/core/packages)
- Strong name? How to avoid binding redirects? Prerequesite: assembly version.
  MUST be done before v1.0.0.
  * [Strong Name Signing](https://github.com/dotnet/runtime/blob/master/docs/project/strong-name-signing.md)
  * [Strong naming](https://docs.microsoft.com/en-gb/dotnet/standard/library-guidance/strong-naming)
- Publish symbols? SourceLink?
  * `<AllowedOutputExtensionsInPackageBuildOutputFolder>$(AllowedOutputExtensionsInPackageBuildOutputFolder);.pdb</AllowedOutputExtensionsInPackageBuildOutputFolder>`
  * [NuGet#4142](https://github.com/NuGet/Home/issues/4142)
- MSBuild:
  * Improve NuGet package description.
  * `Deterministic`, `Title`, `AssemblyTitle`, `VersionPrefix`.

TODOs
-----

See `FIXME`, `TODO` and `REVIEW` within the code.
- LINQ: optimize, more ops.
- Async methods?
  * Eager validation.
  * Microsoft.Bcl.AsyncInterfaces
  * Async enumerables.
- .NET Standard 2.1 & .NET Core 3.1
  * May helpers w/ `Span<T>`
  * `System.HashCode`

Operations:
- Tests. Currently, there are many cases where we don't go beyond smoke tests.
- Code coverage w/ coverlet.
- XML comments with complete examples.
- Perf tool (LINQ, nulls).

### Issues

- MSBuild [MSB3277](https://github.com/microsoft/msbuild/issues/608)
- Scripts: stop on first error (rewrite in PowerShell?).

Guidelines
----------

- Methods that return something should have the attribure `Pure`. It is not
  mandatory but it clearly states that the result should not be ignored.
  It might seem superfluous, but "ça ne mange pas de pain".
- Add _nullable annotations_ whenever necessary.
- _Seal classes_ unless they are designed with extensibility in mind.
- Tag any use of the null-forgiving operator (!) with `BONSANG!`.

Future plans?
-------------

[Narvalo.NET](https://github.com/chtoucas/Narvalo.NET)
- utilities to write code in the ROP style (Railway Oriented Programming).
- a fast Gregorian date type
- country codes
- currency codes
- simple money type
- BIC and IBAN
- roman numerals
- string builder & math extensions

Numerics:
- port of hypercalc to .NET (WinUI)
- big decimal
- [Integer](https://en.wikipedia.org/wiki/Integer_(computer_science))
- [Minifloat](https://en.wikipedia.org/wiki/Minifloat)
- [Signed number representations](https://en.wikipedia.org/wiki/Signed_number_representations)
- [Binary-coded decimal](https://en.wikipedia.org/wiki/Binary-coded_decimal)