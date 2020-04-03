@echo off
@setlocal

:Settings
@set Version=4.5.3

@set ReportType=coverlet
@rem ReportType=opencover

:Report
@set ReportGenerator=%USERPROFILE%\.nuget\packages\reportgenerator\%Version%\tools\net47\ReportGenerator.exe

@echo Building report and badges.
@call :OnError %ReportGenerator% -verbosity:Warning ^
    -reporttypes:HtmlInline;Badges;TextSummary ^
    -reports:__\%ReportType%\%ReportType%.xml -targetdir:__\%ReportType%

@move /Y __\%ReportType%\badge_combined.svg __\%ReportType%.svg > nul
@move /Y __\%ReportType%\Summary.txt __\%ReportType%.txt > nul

@endlocal
@exit /b %ERRORLEVEL%

:OnError
%*
if errorlevel 1 pause
@goto ExitOnError

:ExitOnError
@endlocal
@exit /b 1
