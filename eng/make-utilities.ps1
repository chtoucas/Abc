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
    [Alias('x')] [string] $XPlat
)

try {
    $rootDir = (Get-Item $PSScriptRoot).Parent.FullName
    pushd $rootDir

    $args = @("-c:$Configuration")

    switch ($Task.ToLowerInvariant()) {
        'test' {
            $outDir   = join-path $rootDir '__\tests\'
            $coverXml = join-path $outDir 'opencover.xml'
            $rgXml    = join-path $outDir "opencover.$XPlat.xml"
            $rgDir    = Join-Path $outDir "html-$XPlat"

            $args +=
                '/p:SignAssembly=false', # Necessary for .NET Framework Full
                "-f:$XPlat",
                '/p:CollectCoverage=true',
                '/p:CoverletOutputFormat=opencover',
                "/p:CoverletOutput=$coverXml",
                '/p:Include="[Abc.Utilities]*"',
                '/p:Exclude="[Abc.Utilities]System.*"'

            & dotnet test 'test\Abc.Utilities.Tests\' $args

            & dotnet tool run reportgenerator `
                -verbosity:Warning `
                -reporttypes:"HtmlInline" `
                -reports:$rgXml `
                -targetdir:$rgDir
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
