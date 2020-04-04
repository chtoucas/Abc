:: Quickly run the "perf" program.

@echo off
@setlocal

@call dotnet run -c Release -p .\perf\ -- %*

@endlocal
@exit /b %ERRORLEVEL%
