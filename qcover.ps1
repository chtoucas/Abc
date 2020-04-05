#Requires -Version 4.0

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false, Position = 0)]
    [ValidateSet('coverlet', 'opencover')]
    [Alias('t')] [string] $ToolName = 'coverlet',

    [Alias('r')] [switch] $Report,

    [switch] $NoLogo,
    [Alias('q')] [switch] $Quiet
)

Set-StrictMode -Version Latest

# ------------------------------------------------------------------------------

trap {
    Write-Host ('An unexpected error occured: {0}' -f $_.Exception.Message) `
        -BackgroundColor Red -ForegroundColor Yellow

    Exit 1
}

# ------------------------------------------------------------------------------

if ($Quiet) { $NoLogo = $true }

if (!$NoLogo) {
    Write-Host "Code Coverage  w/ $ToolName.`n"
}

. '.\eng\helpers.ps1'

# ------------------------------------------------------------------------------

# The path is relative to the test project (..\).
$output = "..\__\coverlet\coverlet.xml"
$exclude = '\"[Abc*]System.Diagnostics.CodeAnalysis.*,[Abc*]System.Runtime.CompilerServices.*,[Abc*]Microsoft.CodeAnalysis.*\"'

. dotnet test -c Debug --no-restore `
    /p:CollectCoverage=true `
    /p:CoverletOutputFormat=opencover `
    /p:CoverletOutput=$output `
    /p:Include="[Abc.Maybe]*" `
    /p:Exclude=$exclude

# Write-Host "The report is here: ..." -BackgroundColor 'DarkGreen' -ForegroundColor Yellow

# ------------------------------------------------------------------------------
