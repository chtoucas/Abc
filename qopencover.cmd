:: Test coverage w/ OpenCover.
:: Slow when compared to coverlet, but we get:
:: - risk hotspots (NPath complexity, crap score).
:: - list of unvisited methods.
:: More importantly, the results differ slightly (LINQ and async so far) which
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

:: See https://github.com/opencover/opencover/wiki/Usage
@call %OpenCover% -showunvisited -oldStyle -register:user -hideskipped:All ^
    -output:"%~dp0\__\opencover\opencover.xml" ^
    -target:dotnet.exe ^
    -targetargs:"test %~dp0\Abc.Tests\Abc.Tests.csproj -v quiet -c Debug /p:DebugType=Full" ^
    -filter:%filter% ^
    -excludebyattribute:*.ExcludeFromCodeCoverageAttribute

@endlocal
@exit /b %ERRORLEVEL%
