:: Test coverage w/ OpenCover.

@echo off
@setlocal

@set Version=4.7.922

@rem OpenCover=%USERPROFILE%\.nuget\packages\opencover\%Version%\tools\OpenCover.Console.exe
@set OpenCover=%~dp0\.nuget\packages\OpenCover.%Version%\tools\OpenCover.Console.exe
@set filter="+[Abc.Maybe]* -[Abc]* -[Abc.Future]* -[Abc.Test*]* -[Abc*]System.Diagnostics.CodeAnalysis.* -[Abc*]System.Runtime.CompilerServices.* -[Abc*]Microsoft.CodeAnalysis.*"
@set artifacts_dir=%~dp0\__\opencover

@if not exist %artifacts_dir% mkdir %artifacts_dir%

:: See https://github.com/opencover/opencover/wiki/Usage
@call %OpenCover% -showunvisited -oldStyle -register:user -hideskipped:All ^
    -output:"%artifacts_dir%\opencover.xml" ^
    -target:dotnet.exe ^
    -targetargs:"test %~dp0\Abc.Tests\Abc.Tests.csproj -v quiet -c Debug --no-restore /p:DebugType=Full" ^
    -filter:%filter% ^
    -excludebyattribute:*.ExcludeFromCodeCoverageAttribute

@endlocal
@exit /b %ERRORLEVEL%
