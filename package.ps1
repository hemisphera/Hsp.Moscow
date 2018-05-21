if (Test-Path "$PSScriptRoot\Release") {
    Remove-Item "$PSScriptRoot\Release" -Recurse -Force
}

New-Item -ItemType Directory "$PSScriptRoot\Release" -Force | Out-Null
New-Item -ItemType Directory "$PSScriptRoot\Release\Plugins" -Force | Out-Null

Get-ChildItem "$PSScriptRoot\Hsp.Moscow\bin\Release" | Copy-Item -Destination "$PSScriptRoot\Release"
Get-ChildItem "$PSScriptRoot\Hsp.Moscow.Plugins\bin\Release" | Copy-Item -Destination "$PSScriptRoot\Release\Plugins"

Get-ChildItem "$PSScriptRoot\Release" -Filter "*.xml" -Recurse | Remove-Item
Get-ChildItem "$PSScriptRoot\Release" -Filter "*.pdb" -Recurse | Remove-Item

Remove-Item "$PSScriptRoot\Release\Plugins\Hsp.Moscow.Extensibility.dll"