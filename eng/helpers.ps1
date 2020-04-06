
function Say-Loud {
  [CmdletBinding()]
  Param(
    [Parameter(Mandatory=$True, ValueFromPipeline=$False, ValueFromPipelineByPropertyName=$False)]
    [ValidateNotNullOrEmpty()]
    [string] $Message
  )

  Write-Host $Message -BackgroundColor DarkCyan -ForegroundColor Green
}

function Carp {
  [CmdletBinding()]
  Param(
    [Parameter(Mandatory=$True, ValueFromPipeline=$False, ValueFromPipelineByPropertyName=$False)]
    [ValidateNotNullOrEmpty()]
    [string] $Message
  )

  Write-Host $Message -BackgroundColor Red -ForegroundColor Yellow
}

function Confess {
  [CmdletBinding()]
  Param(
    [Parameter(Mandatory=$True, ValueFromPipeline=$False, ValueFromPipelineByPropertyName=$False)]
    [ValidateNotNullOrEmpty()]
    [string] $Message
  )

  Write-Host $Message -ForegroundColor Green
}

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
