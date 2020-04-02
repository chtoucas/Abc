:: To publish the result:
:: > dotnet nuget push .\Abc.Maybe.XXX -k XXX -s https://www.nuget.org/

@echo off
@setlocal

:Settings
:: Last update 27/03/2020
@set Version=1.0.0-alpha-1

:Test
@echo Cleaning...
@call :OnError dotnet clean -c Release -v minimal
@call :OnError dotnet test .\Abc.Tests\ -c Release --nologo

:Pack
@call :OnError dotnet pack .\Abc.Maybe -c Release --nologo ^
    --output __packages ^
    -p:TargetFrameworks=\"netstandard2.0;netstandard2.1;netcoreapp3.1\" ^
    -p:PackageVersion=%Version%

@endlocal
@exit /b %ERRORLEVEL%

:OnError
%*
if errorlevel 1 pause
@endlocal
@exit /b 1
