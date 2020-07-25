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

    [Alias('x')] [switch] $XPlat
)

try {
    pushd (Get-Item $PSScriptRoot).Parent.FullName

    $args = @("-c:$Configuration")

    switch ($Task.ToLowerInvariant()) {
        'test' {
            if ($XPlat) {
                $args += '--collect:"XPlat Code Coverage"', '-f:netcoreapp3.1'
            }
            
            $outDir = '__\tests\'
            if ($XPlat -and (Test-Path $outDir)) {
                Remove-Item -LiteralPath $outDir -Force -Recurse
            }

            & dotnet test 'test\Abc.Utilities.Tests\' $args

            if ($XPlat) {
                $reports   = Get-ChildItem $outDir -File -Recurse -Filter "*.opencover.xml"
                $targetDir = Join-Path $outDir "html"

                & dotnet tool run reportgenerator `
                    -verbosity:Warning `
                    -reporttypes:"HtmlInline" `
                    -reports:$reports `
                    -targetdir:$targetDir
            }
        }
        'pack' {
            & dotnet build 'src\Abc.Utilities\' $args /p:FatBuild=true
            & dotnet pack  'src\Abc.Utilities\' $args /p:NoBuild=true
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
