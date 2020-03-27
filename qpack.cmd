:: To publish the result:
:: > dotnet nuget push .\Abc.Maybe.XXX -k XXX -s https://www.nuget.org/

@echo off
@setlocal

@set targetFrameworks=\"netstandard2.0;netstandard2.1;netcoreapp3.1\"
@call dotnet pack .\Abc.Maybe\ -c Release -o __packages --include-symbols -p:TargetFrameworks=%targetFrameworks% --version-suffix "alpha-1"

@endlocal
@exit /b %ERRORLEVEL%
