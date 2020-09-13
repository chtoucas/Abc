# See LICENSE in the project root for license information.

#Requires -Version 7

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false, Position = 0)]
    [ValidateSet('pack', 'push')]
                 [string] $Task = 'pack'
)

. (Join-Path $PSScriptRoot 'common.ps1')

New-Variable PROJECT_NAME 'Abc.Testing' -Scope Script -Option Constant

try {
    pushd $ROOT_DIR

    switch ($Task.ToLowerInvariant()) {
        'pack' {
            $project = Join-Path $SRC_DIR $PROJECT_NAME

            Write-Host "Building ""$PROJECT_NAME""..." -ForegroundColor Yellow
            & dotnet build $project /p:RetailBuild=true
                || die 'Failed to build the project.'

            Write-Host "`nPacking ""$PROJECT_NAME""..." -ForegroundColor Yellow
            & dotnet pack $project --no-build
                || die 'Failed to pack the project.'
        }
        'push' {
            $package = gci (Join-Path $ARTIFACTS_DIR "packages\$PROJECT_NAME.Sources.*.nupkg") `
                | sort LastWriteTime | select -Last 1

            if ($package -eq $null) {
                Write-Host 'There is nothing to be published.' -ForegroundColor Yellow
                exit 0
            }

            Write-Host "Found package ""$package""."

            if ((Read-Host 'Publish package to MyGet?', '[y/N]') -eq 'y') {
                Write-Host 'Pushing package to MyGet...' -ForegroundColor Yellow
                & dotnet nuget push $package --force-english-output -s myget
                    || die 'Failed to push the package to MyGet.'
            }
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
