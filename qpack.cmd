:: To publish the result:
:: > dotnet nuget push .\Abc.Maybe.XXX -k XXX -s https://www.nuget.org/

@echo off
@setlocal

:Settings
:: Last update 27/03/2020
@set Version=1.0.0-alpha-1

:Test
@call dotnet clean -c Release -v minimal
@call dotnet test .\Abc.Tests\ %* -c Release --no-restore --nologo

:Pack
@set targetFrameworks=\"netstandard2.0;netstandard2.1;netcoreapp3.1\"
@call dotnet pack .\Abc.Maybe -c Release --no-build -o __packages --include-symbols -p:TargetFrameworks=%targetFrameworks% -p:PackageVersion=%Version%

@endlocal
@exit /b %ERRORLEVEL%
