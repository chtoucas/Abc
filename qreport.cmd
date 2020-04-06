@echo off
@setlocal

@if [%1] == [] (@call set ReportType=coverlet) else (@call set ReportType=%1)

@if not exist __\%ReportType% (
    @echo.
    @echo *** Code Coverage %ReportType% does not exist ***
    @echo.
    @goto ExitOnError
)

@echo Building report and badges for '%ReportType%'.

@call :OnError dotnet tool run reportgenerator -verbosity:Warning ^
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
