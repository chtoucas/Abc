:: To publish the result:
:: > dotnet nuget push .\Abc.Maybe.XXX -k XXX -s https://www.nuget.org/
::
:: TODO: stop if the package already exists.

@echo off
@setlocal

:Settings
@set Version=1.0.0-alpha-2

:Test
@echo Cleaning...
@call :OnError dotnet clean -c Release -v minimal --nologo
@call :OnError dotnet test .\Abc.Tests\ -c Release

:Pack
@call :OnError dotnet pack .\Abc.Maybe -c Release --nologo ^
    --output __\packages ^
    -p:TargetFrameworks=\"netstandard2.0;netstandard2.1;netcoreapp3.1\" ^
    -p:Deterministic=true ^
    -p:PackageVersion=%Version%

@endlocal
@exit /b %ERRORLEVEL%

:OnError
%*
if errorlevel 1 pause
@endlocal
@exit /b 1
