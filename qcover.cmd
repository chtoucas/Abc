:: Will crash if the packages were not restored before.

@echo off
@setlocal

:Settings
@set ReportGeneratorVersion=4.5.3

:CodeCoverage
@call :OnError dotnet test -c Debug --no-restore ^
    /p:CollectCoverage=true ^
    /p:CoverletOutputFormat=opencover ^
    /p:CoverletOutput="..\__coverage\coverlet.xml" ^
    /p:Include="[Abc.Maybe]*" ^
    /p:Exclude=\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"

:Report
@set ReportGenerator=%USERPROFILE%\.nuget\packages\reportgenerator\%ReportGeneratorVersion%\tools\net47\ReportGenerator.exe

@if not exist %ReportGenerator% (
    @echo %ReportGenerator%
    @echo.
    @echo *** Path to ReportGenerator is wrong ***
    @echo.
    @goto ExitOnError
)

@call :OnError %ReportGenerator% -verbosity:Info -reporttypes:Html;Badges ^
    -reports:__coverage\coverlet.xml -targetdir:__coverage

@move /Y __coverage\badge_combined.svg coverage.svg

@endlocal
@exit /b %ERRORLEVEL%

:OnError
%*
if errorlevel 1 pause
@goto ExitOnError

:ExitOnError
@endlocal
@exit /b 1
