# See LICENSE in the project root for license information.

#Requires -Version 7

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false, Position = 0)]
    [ValidateSet('test', 'pack')]
                 [string] $Task = 'test',

    [Parameter(Mandatory = $false)]
    [ValidateSet('Debug', 'Release')]
    [Alias('c')] [string] $Configuration = 'Debug',

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [Alias('f')] [string] $Framework
)

function die([string] $message) { Write-Error $message ;  exit 1 }

try {
    $rootDir = (Get-Item $PSScriptRoot).Parent.FullName
    pushd $rootDir

    $args = @("-c:$Configuration")

    switch ($Task.ToLowerInvariant()) {
        'test' {
            $proj     = 'test\Abc.Tests\'
            $format   = 'opencover'
            $outDir   = Join-Path $rootDir "__\tests-abc-$Configuration\".ToLowerInvariant()
            $output   = Join-Path $outDir "$format.xml"
            $rgInput  = Join-Path $outDir "$format.*.xml"
            $rgOutput = Join-Path $outDir 'html'

            if (Test-Path $rgOutput) {
                Remove-Item $rgOutput -Force -Recurse
            }

            if ($Framework) { $args += "-f:$Framework" }

            Write-Host "Building..." -ForegroundColor Yellow
            # To use Coverlet with .NET Framework Full:
            # - Force the portable pdb format.
            # - Do not sign the assembly: System.IO.FileLoadException.
            & dotnet build $proj $args `
                /p:DebugType=portable `
                /p:SignAssembly=false
                || die 'Failed to build the test project.'

            Write-Host "`nTesting..." -ForegroundColor Yellow
            & dotnet test $proj $args `
                --no-build `
                /p:CollectCoverage=true `
                /p:CoverletOutputFormat=$format `
                /p:CoverletOutput=$output `
                /p:Include="[Abc]*" `
                /p:Exclude="[Abc]System.*"
                || die 'Failed to run the test project.'

            Write-Host "Reporting..." -ForegroundColor Yellow
            & dotnet tool run reportgenerator `
                -reporttypes:"Html" `
                -reports:$rgInput `
                -targetdir:$rgOutput
                || die 'Failed to create the reports.'
        }
        'pack' {
            $proj = 'src\Abc\'

            Write-Host "Building..." -ForegroundColor Yellow
            & dotnet build $proj $args /p:ContinuousIntegrationBuild=true
                || die 'Failed to build the project.'

            Write-Host "`nPacking..." -ForegroundColor Yellow
            & dotnet pack $proj $args --no-build
                || die 'Failed to pack the project.'
        }
    }
}
catch {
    Write-Host $_ -Foreground Red
    Write-Host $_.Exception
    Write-Host $_.ScriptStackTrace
    exit 1
}
finally {
    popd
}
