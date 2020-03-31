
@echo off
@setlocal

:Versions
@set ReportGeneratorVersion=4.5.3

dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="..\__work\coverlet.xml" /p:Include="[Abc.Maybe]*" /p:Exclude=\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"

:ReportGenerator
@set ReportGenerator=%USERPROFILE%\.nuget\packages\reportgenerator\%ReportGeneratorVersion%\tools\net47\ReportGenerator.exe

@if not exist %ReportGenerator% (
    @echo %ReportGenerator%
    @echo.
    @echo *** Path to ReportGenerator is wrong ***
    @echo.
    @goto Error
)

@call %ReportGenerator% -verbosity:Info -reporttypes:Html -reports:__work\coverlet.xml -targetdir:__work\__ReporGeneratorHtml2

@endlocal
@exit /b %ERRORLEVEL%

:Error
@endlocal
@exit /b 1
