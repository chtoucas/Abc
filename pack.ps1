#Requires -Version 4.0

[CmdletBinding()]
param(
    [switch] $Clean,
    [switch] $Test
)

Set-StrictMode -Version Latest

trap {
    Write-Host ('An unexpected error occured: {0}' -f $_.Exception.Message) `
        -BackgroundColor Red -ForegroundColor Yellow
    Exit 1
}

$version = '1.0.0-alpha-2'

$outdir  = "$PSScriptRoot\__\packages"

if (Test-Path "$outdir\Abc.Maybe.$version.nupkg") {
    Write-Host "A package with the same version ($version) already exists" `
        -BackgroundColor Red -ForegroundColor Yellow
}

if ($Clean) {
    & dotnet clean -c Release -v minimal --nologo
}

if ($Test) {
    & dotnet test .\Abc.Tests -c Release
}

& dotnet pack .\Abc.Maybe -c Release --nologo `
    --output $outdir `
    -p:TargetFrameworks='\"netstandard2.0;netstandard2.1;netcoreapp3.1\"' `
    -p:Deterministic=true `
    -p:PackageVersion=$version

# To publish the result:
# > dotnet nuget push .\Abc.Maybe.XXX -k XXX -s https://www.nuget.org/