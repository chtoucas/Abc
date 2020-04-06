@echo off
@setlocal

@if [%1] == [] (@call set ReportType=coverlet) else (@call set ReportType=%1)

@if not exist __\%ReportType% (
    @echo.
    @echo *** File __\%ReportType%\%ReportType%.xml does not exist ***
    @echo.
    @goto ExitOnError
)

@echo Building HTML report for '%ReportType%'.

@call dotnet tool run reportgenerator -verbosity:Warning ^
    -reporttypes:HtmlInline ^
    -reports:__\%ReportType%\%ReportType%.xml -targetdir:__\%ReportType%

@endlocal
@exit /b %ERRORLEVEL%
