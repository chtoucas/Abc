#Requires -Version 4.0

<#
.SYNOPSIS
    Run the Code Coverage script and build human-readable reports.
.DESCRIPTION
    Run the Code Coverage script w/ either Coverlet (default) or OpenCover, then
    optionally build human-readable reports and badges.

    Prerequesites: NuGet packages and tools must have been restored before.

    OpenCover is slow when compared to Coverlet, but we get risk hotspots (NPath
    complexity, crap score) and the list of unvisited methods. Furthermore, the
    results differ slightly (LINQ and async so far) which makes the two tools
    complementary (line counts may differ too but that's a detail).
.PARAMETER OpenCover
    Use OpenCover instead of Coverlet.
.PARAMETER NoReport
    Dot not build HTML/text reports and badges w/ ReportGenerator.
.INPUTS
    None.
.OUTPUTS
    None.
.EXAMPLE
    PS>cover.ps1
    Run Coverlet then build the human-readable reports.
.EXAMPLE
    PS>cover.ps1 -x
    Run OpenCover then build the human-readable reports.
.EXAMPLE
    PS>cover.ps1 -x -n
    Run OpenCover, do NOT build human-readable reports and badges.
#>
[CmdletBinding()]
param(
    [Alias('x')] [switch] $OpenCover,
    [Alias('n')] [switch] $NoReport
)

Set-StrictMode -Version Latest

trap {
    Write-Host ('An unexpected error occured: {0}' -f $_.Exception.Message) `
        -BackgroundColor Red -ForegroundColor Yellow
    Exit 1
}

. '.\eng\helpers.ps1'

# Note to myself: do not use a separate directory for building.
# Build warnings MSB3277, the problem is that we then build all platforms
# within the same dir.

$artifactsDirName = "__"
$artifactsDir = "$PSScriptRoot\$artifactsDirName"

# Run the Code Coverage tool.
if ($OpenCover) {
    $reportType = 'opencover'
    $reportDir  = "$artifactsDir\$reportType"
    $reportXml  = "$reportDir\$reportType.xml"

    # Find the OpenCover version.
    $proj = [Xml] (Get-Content ".\Abc.Tests\Abc.Tests.csproj")
    $version = $proj | Get-ToolVersion -ToolName "OpenCover"

    if (!(Test-Path $reportDir)) {
        New-Item -ItemType Directory -Force -Path $reportDir | Out-Null
    }

    Write-Host "Running OpenCover v$version." -ForegroundColor Green

    # See https://github.com/opencover/opencover/wiki/Usage
    $openCoverExe = "$env:USERPROFILE\.nuget\packages\opencover\$version\tools\OpenCover.Console.exe"
    $filter = "+[Abc.Maybe]* -[Abc]* -[Abc.Future]* -[Abc.Test*]* -[Abc*]System.Diagnostics.CodeAnalysis.* -[Abc*]System.Runtime.CompilerServices.* -[Abc*]Microsoft.CodeAnalysis.*"
    $targetargs = "test $PSScriptRoot\Abc.Tests\Abc.Tests.csproj -v quiet -c Debug --no-restore /p:DebugType=Full"

    & $openCoverExe -showunvisited -oldStyle -register:user -hideskipped:All `
        "-output:$reportXml" `
        "-target:dotnet.exe" `
        "-targetargs:$targetargs" `
        "-filter:$filter" `
        "-excludebyattribute:*.ExcludeFromCodeCoverageAttribute"
} else {
    $reportType = 'coverlet'
    $reportDir  = "$artifactsDir\$reportType"
    $reportXml  = "$reportDir\$reportType.xml"

    Write-Host "Running Coverlet." -ForegroundColor Green

    # The path is relative to the test project (..\).
    $output = "$PSScriptRoot\$artifactsDirName\$reportType\$reportType.xml"
    $exclude = '\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"'

    & dotnet test -c Debug --no-restore `
        /p:CollectCoverage=true `
        /p:CoverletOutputFormat=opencover `
        /p:CoverletOutput=$output `
        /p:Include="[Abc.Maybe]*" `
        /p:Exclude=$exclude
}

# Build reports and badges.
if ($NoReport) {
    Write-Host "On user request, we do not run ReportGenerator." `
        -ForegroundColor Green
} else {
    Write-Host "Building HTML/text reports and badge w/ ReportGenerator." `
        -ForegroundColor Green

    $args = `
        '-verbosity:Warning',
        '-reporttypes:HtmlInline;Badges;TextSummary',
        "-reports:$reportXml",
        "-targetdir:$reportDir"

    & dotnet tool run reportgenerator $args

    Copy-Item -Force -Path "$reportDir\badge_combined.svg" `
        -Destination "$artifactsDir\$reportType.svg"
    Copy-Item -Force -Path "$reportDir\Summary.txt" `
        -Destination "$artifactsDir\$reportType.txt"

    Write-Host "The HTML report can be found here: '$reportDir'." `
        -ForegroundColor Yellow
}
