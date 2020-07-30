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

. (Join-Path $PSScriptRoot 'common.ps1')

New-Variable PROJECT_NAME 'Abc' -Scope Script -Option Constant

try {
    pushd $ROOT_DIR

    switch ($Task.ToLowerInvariant()) {
        'test' {
            Invoke-Coverlet `
                -ProjectName   $PROJECT_NAME `
                -Configuration $Configuration `
                -Framework     $Framework
        }
        'pack' {
            $project = Join-Path $SRC_DIR $PROJECT_NAME

            Write-Host "Packing ""$PROJECT_NAME""..." -ForegroundColor Yellow
            & dotnet pack $project -c $Configuration /p:Retail=true
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
