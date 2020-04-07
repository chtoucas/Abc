
function Get-ToolVersion {
  [CmdletBinding()]
  param(
    [Parameter(Mandatory = $true, Position = 0, ValueFromPipeline = $true)]
    [Xml] $Xml,

    [string] $ToolName
  )

  Select-Xml -Xml $Xml -XPath "//Project/ItemGroup/PackageReference[@Include='$ToolName']" `
    | select -ExpandProperty Node | select -First 1 -ExpandProperty Version
}
