#Requires -Version 4.0

<#
.SYNOPSIS
    Run the Code Coverage script and optionally build human-readable reports.
.DESCRIPTION
    Run the Code Coverage script w/ either Coverlet (default) or OpenCover,
    then optionally build human-readable reports.

    Prerequesites: NuGet packages and tools must have been restored before.
.PARAMETER OpenCover
    Use OpenCover instead of Coverlet.
.PARAMETER NoReport
    Dot not build HTML/text reports and badges w/ ReportGenerator.
.INPUTS
    None.
.OUTPUTS
    None.
.EXAMPLE
    PS>qcover.ps1
    Run Coverlet then build the human-readable reports.
.EXAMPLE
    PS>qcover.ps1 -x
    Run OpenCover then build the human-readable reports.
.EXAMPLE
    PS>qcover.ps1 -x -n
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

$artifactsPath = "__"

# Run the Code Coverage tool.
if ($OpenCover) {
    $reportType = 'opencover'
    $reportDir  = "$artifactsPath\$reportType"
    $reportXml  = "$reportDir\$reportType.xml"

    # Find the OpenCover version.
    $proj = [Xml] (Get-Content ".\Abc.Tests\Abc.Tests.csproj")
    $version = $proj | Get-ToolVersion -ToolName "OpenCover"

    Write-Host "Running OpenCover v$version." -ForegroundColor Green
    Write-Host "Not done yet." -ForegroundColor Red
} else {
    $reportType = 'coverlet'
    $reportDir  = "$artifactsPath\$reportType"
    $reportXml  = "$reportDir\$reportType.xml"

    Write-Host "Running Coverlet." -ForegroundColor Green

    # The path is relative to the test project (..\).
    $output = "..\$reportXml"
    $exclude = '\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"'

    & dotnet test -c Debug --no-restore `
        /p:CollectCoverage=true `
        /p:CoverletOutputFormat=opencover `
        /p:CoverletOutput=$output `
        /p:Include="[Abc.Maybe]*" `
        /p:Exclude=$exclude
}

# Build report and badges.
if ($NoReport) {
    Write-Host "On user request, no human-readable reports will be generated." `
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

    Move-Item -Force -Path "$reportDir\badge_combined.svg" `
        -Destination "$artifactsPath\$reportType.svg"
    Move-Item -Force -Path "$reportDir\Summary.txt" `
        -Destination "$artifactsPath\$reportType.txt"

    Write-Host "The HTML report can be found here: '$reportDir'." `
        -ForegroundColor Yellow
}
