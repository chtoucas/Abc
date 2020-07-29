# See LICENSE in the project root for license information.

#Requires -Version 7

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false, Position = 0)]
    [ValidateSet('test', 'pack', 'push')]
                 [string] $Task = 'test',

    [Parameter(Mandatory = $false)]
    [ValidateSet('Debug', 'Release')]
    [Alias('c')] [string] $Configuration = 'Debug',

    [Parameter(Mandatory = $false)]
    [ValidateNotNullOrEmpty()]
    [Alias('f')] [string] $Framework
)

. (Join-Path $PSScriptRoot 'common.ps1')

New-Variable PROJECT_NAME 'Abc.Utilities' -Scope Script -Option Constant

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

            Write-Host "Building ""$PROJECT_NAME""..." -ForegroundColor Yellow
            & dotnet build $project -c $Configuration /p:FatBuild=true /p:DisableCodeCoverage=true
                || die 'Failed to build the project.'

            Write-Host "`nPacking ""$PROJECT_NAME""..." -ForegroundColor Yellow
            & dotnet pack $project -c $Configuration --no-build
                || die 'Failed to pack the project.'
        }
        'push' {
            $package = gci (Join-Path $ARTIFACTS_DIR "packages\$PROJECT_NAME.Sources.*.nupkg") `
                | sort LastWriteTime | select -Last 1

            if ($package -eq $null) {
                Write-Host "There is nothing to be published." -ForegroundColor Yellow
                exit 0
            }

            $answer = (Read-Host "Publish package: ""$package""?", "[y/N]")
            if ($answer -eq "" -or $answer -eq "n") {
                Write-Host "Discarded on your request." -ForegroundColor DarkCyan
                exit 0
            }

            # TODO: apikey warning
            # https://github.community/t/github-package-registry-not-compatible-with-dotnet-nuget-client/14392/6
            Write-Host "Pushing package to GitHub..." -ForegroundColor Yellow
            & dotnet nuget push $package -s github --force-english-output
                || die 'Failed to push the package.'
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
