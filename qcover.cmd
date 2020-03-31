:: Will crash if the packages were not restored before.

@echo off
@setlocal

:Versions
@set OpenCoverVersion=4.7.922
@set ReportGeneratorVersion=4.5.3

@set outdir=%~dp0\__work
@if not exist %outdir% mkdir %outdir%

:Build
:: Not necessary, but seems to speed up the whole process and might prevent
:: random crashes w/ OpenCover.
@rem @call dotnet build -c Debug /p:DebugType=Full -p:TargetFrameworks=netcoreapp3.1

:OpenCover
@set OpenCover=%USERPROFILE%\.nuget\packages\opencover\%OpenCoverVersion%\tools\OpenCover.Console.exe

@if not exist %OpenCover% (
    @echo.
    @echo *** Path to OpenCover.Console.exe is wrong ***
    @echo.
    @goto Error
)

@rem @set target="C:/Program Files/dotnet/dotnet.exe"
@set target="dotnet.exe"
@set proj="%~dp0\Abc.Tests\Abc.Tests.csproj"
@set targetargs="test %proj% -v quiet -c Debug /p:DebugType=Full"

@set opencover_xml=%outdir%\opencover.xml

@rem Voir https://github.com/opencover/opencover/wiki/Usage

:: Only Abc.Maybe.
@set filter="+[Abc.Maybe]* -[Abc*]System.Diagnostics.CodeAnalysis.* -[Abc*]System.Runtime.CompilerServices.* -[Abc*]Microsoft.CodeAnalysis.*"
@call %OpenCover% -showunvisited -oldStyle -register:user -hideskipped:All -output:%opencover_xml% -filter:%filter% -target:%target% -targetargs:%targetargs% -excludebyattribute:*.ExcludeFromCodeCoverageAttribute

:ReportGenerator
@set ReportGenerator=%USERPROFILE%\.nuget\packages\reportgenerator\%ReportGeneratorVersion%\tools\net47\ReportGenerator.exe

@if not exist %ReportGenerator% (
    @echo %ReportGenerator%
    @echo.
    @echo *** Path to ReportGenerator is wrong ***
    @echo.
    @goto Error
)

@call %ReportGenerator% -verbosity:Info -reporttypes:Html -reports:%opencover_xml% -targetdir:%outdir%\__ReporGeneratorHtml

@endlocal
@exit /b %ERRORLEVEL%

:Error
@endlocal
@exit /b 1
