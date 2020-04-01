:: Will crash if the packages were not restored before.

@echo off
@setlocal

:: The path is relative to the test project (..\).
@call dotnet test -c Debug --no-restore ^
    /p:CollectCoverage=true ^
    /p:CoverletOutputFormat=opencover ^
    /p:CoverletOutput="..\__coverage\coverlet.xml" ^
    /p:Include="[Abc.Maybe]*" ^
    /p:Exclude=\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"

@endlocal
@exit /b %ERRORLEVEL%
