@echo off
@setlocal

@call .\nuget.exe restore .\packages.config -PackagesDirectory packages

@endlocal
@exit /b %ERRORLEVEL%
