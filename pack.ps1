#Requires -Version 4.0

<#
.SYNOPSIS
Create a NuGet package.

.PARAMETER Clean
Clean the project before anything else.

.PARAMETER Test
Run the test suite.
#>
[CmdletBinding()]
param(
    [switch] $Clean,
    [switch] $Test
)

Set-StrictMode -Version Latest

trap {
    write-host ("An unexpected error occured: {0}." -f $_.Exception.Message) `
        -BackgroundColor Red -ForegroundColor Yellow
    exit 1
}

. "$PSScriptRoot\eng\helpers.ps1"

$version = '1.0.0-alpha-2'

$outdir  = "$PSScriptRoot\__\packages"

if (test-path "$outdir\Abc.Maybe.$version.nupkg") {
    carp "A package with the same version ($version) already exists."
}

if ($Clean) {
    say-loud 'Cleaning.'
    & dotnet clean -c Release -v minimal --nologo
}

if ($Test) {
    say-loud 'Testing.'
    & dotnet test .\Abc.Tests -c Release -v minimal --nologo
}

say-loud 'Packing.'
& dotnet pack .\Abc.Maybe -c Release --nologo `
    --output $outdir `
    -p:TargetFrameworks='\"netstandard2.0;netstandard2.1;netcoreapp3.1\"' `
    -p:Deterministic=true `
    -p:PackageVersion=$version

confess 'To publish the package:'
confess "> dotnet nuget push .\__\packages\Abc.Maybe.$version.nupkg -s https://www.nuget.org/ -k MYKEY"
