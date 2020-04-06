#Requires -Version 4.0

<#
.SYNOPSIS
Run the Code Coverage script and build human-readable reports.

.DESCRIPTION
Run the Code Coverage script w/ either Coverlet (default) or OpenCover,
then optionally build human-readable reports and badges.

Prerequesites: NuGet packages and tools must have been restored before.

OpenCover is slow when compared to Coverlet, but we get risk hotspots
(NPath complexity, crap score) and the list of unvisited methods.
Furthermore, the results differ slightly (LINQ and async so far)
which makes the two tools complementary --- line counts may differ too
but that's a detail.

.PARAMETER OpenCover
Use OpenCover instead of Coverlet.

.PARAMETER NoReport
Dot not build HTML/text reports and badges w/ ReportGenerator.

.PARAMETER ReportOnly
Dot not run the Code Coverage tool.

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
    [switch] $NoReport,
    [switch] $ReportOnly
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

$reportType = if ($OpenCover) { 'opencover' } else { 'coverlet' }
$artifactsDir = "__"
$outdir = "$PSScriptRoot\$artifactsDir\$reportType"
$outxml = "$outdir\$reportType.xml"

# Run the Code Coverage tool.
if (!$ReportOnly) {
  if ($OpenCover) {
      Write-Host "Running OpenCover." -BackgroundColor DarkCyan -ForegroundColor Green

      # Find the OpenCover version.
      $proj = "$PSScriptRoot\Abc.Tests\Abc.Tests.csproj"
      $version = [Xml] (Get-Content $proj) | Get-ToolVersion -ToolName 'OpenCover'

      if (!(Test-Path $outdir)) {
          New-Item -ItemType Directory -Force -Path $outdir | Out-Null
      }

      $openCoverExe = "$env:USERPROFILE\.nuget\packages\opencover\$version\tools\OpenCover.Console.exe"
      # See https://github.com/opencover/opencover/wiki/Usage
      $filter = "+[Abc.Maybe]* -[Abc]* -[Abc.Future]* -[Abc.Test*]* -[Abc*]System.Diagnostics.CodeAnalysis.* -[Abc*]System.Runtime.CompilerServices.* -[Abc*]Microsoft.CodeAnalysis.*"
      $targetargs = "test $proj -v quiet -c Debug --no-restore /p:DebugType=Full"

      & $openCoverExe -showunvisited -oldStyle -register:user -hideskipped:All `
          "-output:$outxml" `
          -target:dotnet.exe `
          "-targetargs:$targetargs" `
          "-filter:$filter" `
          -excludebyattribute:*.ExcludeFromCodeCoverageAttribute
  } else {
      Write-Host "Running Coverlet." -BackgroundColor DarkCyan -ForegroundColor Green

      # The path is relative to the test project (..\).
      $output = "$PSScriptRoot\$artifactsDir\$reportType\$reportType.xml"
      $exclude = '\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"'

      & dotnet test -c Debug --no-restore `
          /p:CollectCoverage=true `
          /p:CoverletOutputFormat=opencover `
          /p:CoverletOutput=$output `
          /p:Include="[Abc.Maybe]*" `
          /p:Exclude=$exclude
  }
}

# Build reports and badges.
if ($NoReport) {
    Write-Host "On user request, we do not run ReportGenerator." `
         -BackgroundColor DarkCyan -ForegroundColor Green
} else {
    Write-Host "Building HTML/text reports and badge w/ ReportGenerator." `
         -BackgroundColor DarkCyan -ForegroundColor Green

    $args = `
        '-verbosity:Warning',
        '-reporttypes:HtmlInline;Badges;TextSummary',
        "-reports:$outxml",
        "-targetdir:$outdir"

    & dotnet tool run reportgenerator $args

    Push-Location $outdir

    try {
        Copy-Item -Force -Path badge_combined.svg -Destination "..\$reportType.svg"
        Copy-Item -Force -Path Summary.txt -Destination "..\$reportType.txt"
    } finally {
        Pop-Location
    }

    Write-Host "The HTML report can be found here: '$outdir'." `
        -ForegroundColor Yellow
}
