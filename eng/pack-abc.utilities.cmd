:: Test harness (plain and simple).

@echo off
@setlocal

@call dotnet build %~dp0\..\src\Abc.Utilities\ -c Release /p:FatBuild=true

@call dotnet test %~dp0\..\test\Abc.Utilities.Tests\ -c Release

@call dotnet pack %~dp0\..\src\Abc.Utilities\ -c Release /p:NoBuild=true

@endlocal
@exit /b %ERRORLEVEL%
