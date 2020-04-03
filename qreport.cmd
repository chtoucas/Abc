@echo off
@setlocal

:Settings
@set Version=4.5.3

@set ReportType=coverlet
@rem @set ReportType=opencover

:Report
@set ReportGenerator=%USERPROFILE%\.nuget\packages\reportgenerator\%Version%\tools\net47\ReportGenerator.exe

@echo Building report and badges.
@call :OnError %ReportGenerator% -verbosity:Warning ^
    -reporttypes:HtmlInline;Badges;TextSummary ^
    -reports:__\coverage\%ReportType%.xml -targetdir:__\coverage

@move /Y __\coverage\badge_combined.svg %ReportType%.svg > nul
@move /Y __\coverage\Summary.txt %ReportType%.txt > nul

@endlocal
@exit /b %ERRORLEVEL%

:OnError
%*
if errorlevel 1 pause
@goto ExitOnError

:ExitOnError
@endlocal
@exit /b 1
