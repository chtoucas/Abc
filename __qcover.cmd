:: Will crash if the packages were not restored before.

@echo off
@setlocal

@set OpenCoverVersion=4.7.922

@set OpenCover=%USERPROFILE%\.nuget\packages\opencover\%OpenCoverVersion%\tools\OpenCover.Console.exe

@if not exist %OpenCover% (
    @echo.
    @echo *** Path to OpenCover.Console.exe is wrong ***
    @echo.
    @endlocal
    @exit /b 1
)

:: ??? --no-restore -p:TargetFrameworks=netcoreapp3.1 ???
@set target="dotnet.exe"
@set proj="%~dp0\Abc.Tests\Abc.Tests.csproj"
@set targetargs="test %proj% -v quiet -c Debug /p:DebugType=Full"

@set opencover_xml="%~dp0\__coverage\opencover.xml"

@set filter="+[Abc.Maybe]* -[Abc*]System.Diagnostics.CodeAnalysis.* -[Abc*]System.Runtime.CompilerServices.* -[Abc*]Microsoft.CodeAnalysis.*"

@rem Voir https://github.com/opencover/opencover/wiki/Usage
@call %OpenCover% -showunvisited -oldStyle -register:user -hideskipped:All ^
    -output:%opencover_xml% ^
    -filter:%filter% -target:%target% ^
    -targetargs:%targetargs% -excludebyattribute:*.ExcludeFromCodeCoverageAttribute

@endlocal
@exit /b %ERRORLEVEL%
