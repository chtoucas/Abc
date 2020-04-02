@echo off
@setlocal

:Settings
@set ReportGeneratorVersion=4.5.3

@set ReportType=coverlet
@rem @set ReportType=opencover

:Report
@set ReportGenerator=%USERPROFILE%\.nuget\packages\reportgenerator\%ReportGeneratorVersion%\tools\net47\ReportGenerator.exe

@if not exist %ReportGenerator% (
    @echo %ReportGenerator%
    @echo.
    @echo *** Path to ReportGenerator is wrong ***
    @echo.
    @goto ExitOnError
)

@echo Building report and badges.
@call :OnError %ReportGenerator% -verbosity:Warning ^
    -reporttypes:HtmlInline;Badges;TextSummary ^
    -reports:__coverage\%ReportType%.xml -targetdir:__coverage

@move /Y __coverage\badge_combined.svg %ReportType%.svg > nul
@move /Y __coverage\Summary.txt %ReportType%.txt > nul

@endlocal
@exit /b %ERRORLEVEL%

:OnError
%*
if errorlevel 1 pause
@goto ExitOnError

:ExitOnError
@endlocal
@exit /b 1
