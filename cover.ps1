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
Furthermore, the results differ slightly (LINQ and async so far) which
makes the two tools complementary --- line counts may differ too but
that's just a detail.

.PARAMETER OpenCover
Use OpenCover instead of Coverlet.

.PARAMETER NoReport
Dot not build HTML/text reports and badges w/ ReportGenerator.

.PARAMETER ReportOnly
Dot not run any Code Coverage tool.

.EXAMPLE
PS>cover.ps1
Run Coverlet then build the human-readable reports.

.EXAMPLE
PS>cover.ps1 -x
Run OpenCover then build the human-readable reports.

.EXAMPLE
PS>cover.ps1 -OpenCover -NoReport
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
  write-host ("An unexpected error occured: {0}" -f $_.Exception.Message) `
    -BackgroundColor Red -ForegroundColor Yellow
  exit 1
}

. "$PSScriptRoot\eng\say.ps1"
. "$PSScriptRoot\eng\coverage.ps1"

# Note to myself: do not use a separate directory for build.
# Build warnings MSB3277, the problem is that we then build all platforms
# within the same dir.

################################################################################

$tool = if ($OpenCover) { 'opencover' } else { 'coverlet' }
$artifacts = '__'
$outdir = join-path $PSScriptRoot (join-path $artifacts $tool)
$outxml = join-path $outdir "$tool.xml"

# Run the Code Coverage tool.
if ($ReportOnly) {
  carp 'On your request, we do not run the Code Coverage tool.'
} elseif ($OpenCover) {
  say-loud 'Running OpenCover.'

  # Find the OpenCover version.
  $proj = join-path $PSScriptRoot 'Abc.Tests\Abc.Tests.csproj'
  $version = [Xml] (get-content $proj) | Get-ToolVersion -ToolName 'OpenCover'

  if (!(test-path $outdir)) {
      mkdir -Force -Path $outdir | Out-Null
  }

  $openCoverExe = join-path $env:USERPROFILE '\.nuget\packages\opencover\$version\tools\OpenCover.Console.exe'
  # See https://github.com/opencover/opencover/wiki/Usage
  $filter = '+[Abc.Maybe]* -[Abc]* -[Abc.Future]* -[Abc.Test*]* -[Abc*]System.Diagnostics.CodeAnalysis.* -[Abc*]System.Runtime.CompilerServices.* -[Abc*]Microsoft.CodeAnalysis.*'
  $targetargs = "test $proj -v quiet -c Debug --no-restore /p:DebugType=Full"

  & $openCoverExe -showunvisited -oldStyle -register:user -hideskipped:All `
    "-output:$outxml" `
    -target:dotnet.exe `
    "-targetargs:$targetargs" `
    "-filter:$filter" `
    -excludebyattribute:*.ExcludeFromCodeCoverageAttribute
} else {
  say-loud 'Running Coverlet.'

  # The path is relative to the test project (..\).
  $output = "$PSScriptRoot\$artifacts\$tool\$tool.xml"
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
  carp 'On your request, we do not run ReportGenerator.'
} else {
  say-loud 'Running ReportGenerator.'

  $args = `
    '-verbosity:Warning',
    '-reporttypes:HtmlInline;Badges;TextSummary',
    "-reports:$outxml",
    "-targetdir:$outdir"

  & dotnet tool run reportgenerator $args

  try {
    pushd $outdir

    cp -Force -Path badge_combined.svg -Destination (join-path '..' "$tool.svg")
    cp -Force -Path Summary.txt -Destination (join-path '..' "$tool.txt")
  } finally {
    popd
  }

  confess "The HTML report can be found here: '$outdir'."
}

################################################################################
