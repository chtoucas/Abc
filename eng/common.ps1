# See LICENSE in the project root for license information.

#Requires -Version 7

New-Variable ROOT_DIR (Get-Item $PSScriptRoot).Parent.FullName -Scope Script -Option Constant
New-Variable SRC_DIR       (Join-Path $ROOT_DIR 'src')  -Scope Script -Option Constant
New-Variable TEST_DIR      (Join-Path $ROOT_DIR 'test') -Scope Script -Option Constant
New-Variable ARTIFACTS_DIR (Join-Path $ROOT_DIR '__')   -Scope Script -Option Constant

# ------------------------------------------------------------------------------

function die([string] $message) { Write-Error $message ;  exit 1 }

# ------------------------------------------------------------------------------

function Invoke-Coverlet {
    [CmdletBinding(PositionalBinding = $false)]
    param(
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $projectName,

        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string] $configuration,

        [Parameter(Mandatory = $false)]
        [string] $framework
    )

    $format   = 'opencover'
    $outDir   = Join-Path $ARTIFACTS_DIR "tests-$projectName-$configuration\".ToLowerInvariant()
    $output   = Join-Path $outDir "$format.xml"
    $rgInput  = Join-Path $outDir "$format.*.xml"
    $rgOutput = Join-Path $outDir 'html'

    if (Test-Path $rgOutput) {
        Remove-Item $rgOutput -Force -Recurse
    }

    $project = Join-Path $TEST_DIR "$projectName.Tests"
    $args    = @("-c:$Configuration")
    if ($framework) { $args += "-f:$Framework" }

    Write-Host "Building test project for ""$projectName""..." -ForegroundColor Yellow
    # To use Coverlet with .NET Framework Full:
    # - Force the portable pdb format.
    # - Do not sign the assembly: System.IO.FileLoadException.
    & dotnet build $project $args `
        /p:DebugType=portable `
        /p:SignAssembly=false
        || die "Failed to build the project: ""$project""."

    Write-Host "`nTesting ""$projectName""..." -ForegroundColor Yellow
    & dotnet test $project $args `
        --no-build `
        /p:CollectCoverage=true `
        /p:CoverletOutputFormat=$format `
        /p:CoverletOutput=$output `
        /p:Include="[$projectName]*" `
        /p:Exclude="[$projectName]System.*"
        || die "Failed to run the project: ""$project""."

    Write-Host "Creating reports..." -ForegroundColor Yellow
    & dotnet tool run reportgenerator `
        -reporttypes:"Html" `
        -reports:$rgInput `
        -targetdir:$rgOutput
        || die 'Failed to create the reports.'
}

# ------------------------------------------------------------------------------
