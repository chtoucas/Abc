:: Test coverage w/ OpenCover.
:: Beware, will crash if the packages were not restored before.
::
:: Slow when compared to coverlet, but we get:
:: - risk hotspots (NPath complexity, crap score).
:: - list of unvisited methods.
:: More importantly, the results differ sligqhtly (LINQ and async so far) which
:: makes the two tools complementary.
::
:: Note to myself: do not use a separate directory for building.
:: Build warnings w/ OpenCover (MSB3277).
:: The problem is that we would build all platforms within the same dir.

@echo off
@setlocal

@set Version=4.7.922

@set OpenCover=%USERPROFILE%\.nuget\packages\opencover\%Version%\tools\OpenCover.Console.exe
@set filter="+[Abc.Maybe]* -[Abc]* -[Abc.Future]* -[Abc.Test*]* -[Abc*]System.Diagnostics.CodeAnalysis.* -[Abc*]System.Runtime.CompilerServices.* -[Abc*]Microsoft.CodeAnalysis.*"
@set artifacts_dir=%~dp0\__\opencover

@if not exist %artifacts_dir% mkdir %artifacts_dir%

:: See https://github.com/opencover/opencover/wiki/Usage
@call %OpenCover% -showunvisited -oldStyle -register:user -hideskipped:All ^
    -output:"%artifacts_dir%\opencover.xml" ^
    -target:dotnet.exe ^
    -targetargs:"test %~dp0\Abc.Tests\Abc.Tests.csproj -v quiet -c Debug --no-restore /p:DebugType=Full" ^
    -filter:%filter% ^
    -excludebyattribute:*.ExcludeFromCodeCoverageAttribute

@endlocal
@exit /b %ERRORLEVEL%
