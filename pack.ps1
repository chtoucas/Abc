#Requires -Version 4.0

<#
.SYNOPSIS
Create a NuGet package.

.PARAMETER Clean
Clean the solution before anything else.

.PARAMETER Test
Run the test suite.
#>
[CmdletBinding()]
param(
  [switch] $Clean,
  [switch] $Test
)

Set-StrictMode -Version Latest

$PKGDIR = "__\packages"     # Relative path

. (join-path $PSScriptRoot "eng\say.ps1")

################################################################################

function run-clean {
  say-loud "Cleaning."

  & dotnet clean -c Release -v minimal --nologo

  if ($LastExitCode -ne 0) { croak "Clean task failed." }
}

function run-test {
  say-loud "Testing."

  & dotnet clean -c Release -v minimal --nologo

  if ($LastExitCode -ne 0) { croak "Test task failed." }
}

function run-pack([string] $proj, [string] $version) {
  say-loud "Packing."

  $pkg = join-path $PKGDIR "$proj.$version.nupkg"

  if (test-path $pkg) {
    carp "A package with the same version ($version) already exists."

    $question = "Do you wish to proceed anyway? [y/n]"
    $continue = read-host $question
    while ($continue -ne "y") {
      if ($continue -eq "n") { exit 0 }
        $continue = read-host $question
      }
  }

  & dotnet pack $proj -c Release --nologo `
    --output $PKGDIR `
    -p:TargetFrameworks='\"netstandard2.0;netstandard2.1;netcoreapp3.1\"' `
    -p:Deterministic=true `
    -p:PackageVersion=$version `
    | out-host

  if ($LastExitCode -ne 0) { croak "Pack task failed." }

  $pkg
}

################################################################################

try {
  pushd $PSScriptRoot

  if ($Clean) { run-clean }
  if ($Test)  { run-test  }

  $pkg = run-pack "Abc.Maybe" "1.0.0-alpha-2"

  confess "To publish the package:"
  confess "> dotnet nuget push .\$pkg -s https://www.nuget.org/ -k MYKEY"
} catch {
  carp ("An unexpected error occured: {0}." -f $_.Exception.Message)
  exit 1
} finally {
  popd
}

################################################################################
