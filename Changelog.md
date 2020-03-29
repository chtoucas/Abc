# Changelog

### Next

Internal Changes.
- New project `Narvalo.Future` for experimental code.
- Clean up project files.
  * No longer define compiler symbols `NETSTANDARD2_0` and `NETSTANDARD2_1` as
    this is already done by MSBuild;
    see [here](https://docs.microsoft.com/en-us/dotnet/core/tutorials/libraries).
  * Use `.editorconfig` instead of MSBuild to configure language conventions
    inside the editor (warnings IDEnnnn).

### 2020-03-27, version 1.0.0-alpha-1

First public release.

### 2017-04-07, version 0.28.0

Still available on nuget.org but under a different
([name](https://www.nuget.org/packages/Narvalo.Fx/)),
now deprecated. It is broken, please do NOT use.
