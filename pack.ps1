#Requires -Version 4.0

[CmdletBinding()]
param(
    [switch] $Clean,
    [switch] $Test
)

Set-StrictMode -Version Latest

trap {
    Write-Host ('An unexpected error occured: {0}.' -f $_.Exception.Message) `
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
    Write-Host "Cleaning." -BackgroundColor DarkCyan -ForegroundColor Green
    & dotnet clean -c Release -v minimal --nologo
}

if ($Test) {
    Write-Host "Testing." -BackgroundColor DarkCyan -ForegroundColor Green
    & dotnet test .\Abc.Tests -c Release -v minimal --nologo
}

Write-Host "Packing." -BackgroundColor DarkCyan -ForegroundColor Green
& dotnet pack .\Abc.Maybe -c Release --nologo `
    --output $outdir `
    -p:TargetFrameworks='\"netstandard2.0;netstandard2.1;netcoreapp3.1\"' `
    -p:Deterministic=true `
    -p:PackageVersion=$version

Write-Host "To publish the package:" -ForegroundColor Green
Write-Host "  dotnet nuget push .\__\packages\Abc.Maybe.$version.nupkg -s https://www.nuget.org/ -k KEY" `
    -ForegroundColor Yellow
