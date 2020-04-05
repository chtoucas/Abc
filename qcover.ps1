#Requires -Version 4.0

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false, Position = 0)]
    [ValidateSet('coverlet', 'opencover')]
    [Alias('t')] [string] $Tool = 'coverlet',

    [Alias('r')] [switch] $Report
)

Set-StrictMode -Version Latest

trap {
    Write-Host ('An unexpected error occured: {0}' -f $_.Exception.Message) `
        -BackgroundColor Red -ForegroundColor Yellow

    Exit 1
}

Write-Host "Code Coverage w/ $Tool.`n"

. '.\eng\helpers.ps1'

$proj = [Xml] (Get-Content ".\Abc.Tests\Abc.Tests.csproj")
$packages = $proj.Project.ItemGroup

Write-Host $packages

Exit 0

# The path is relative to the test project (..\).
$output = "..\__\$Tool\$Tool.xml"
$exclude = '\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"'

& dotnet test -c Debug --no-restore `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=opencover `
    /p:CoverletOutput=$output `
    /p:Include="[Abc.Maybe]*" `
    /p:Exclude=$exclude

# Write-Host "The report is here: ..." -BackgroundColor 'DarkGreen' -ForegroundColor Yellow
