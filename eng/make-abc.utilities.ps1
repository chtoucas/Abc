# See LICENSE in the project root for license information.

#Requires -Version 7

[CmdletBinding()]
param(
    [Parameter(Mandatory = $false, Position = 0)]
    [ValidateSet('test', 'pack', 'push')]
                 [string] $Task = 'test',

    # Ignored when $Task = 'pack'.
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
                -Framework     $Framework `
                -IsTestAssembly
        }
        'pack' {
            $project = Join-Path $SRC_DIR $PROJECT_NAME

            Write-Host "Building ""$PROJECT_NAME""..." -ForegroundColor Yellow
            & dotnet build $project /p:RetailBuild=true /p:FatBuild=true
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

            # TODO: apikey warning
            # Please use the --api-key option when publishing to GitHub Packages
            # https://github.community/t/github-package-registry-not-compatible-with-dotnet-nuget-client/14392/6
            if ((Read-Host 'Publish package to GitHub?', '[y/N]') -eq 'y') {
                # We use nuget.exe instead of dotnet since the latter does not have
                # an option to specify a custom config.
                $nuget = Join-Path $PSScriptRoot 'nuget.exe' -Resolve
                $configFile = Join-Path $env:AppData '\NuGet\NuGet.Config' -Resolve

                Write-Host 'Pushing package to GitHub...' -ForegroundColor Yellow
                & $nuget push $package -ForceEnglishOutput -Source github `
                    -ConfigFile $configFile
                    || die 'Failed to push the package to GitHub.'
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
