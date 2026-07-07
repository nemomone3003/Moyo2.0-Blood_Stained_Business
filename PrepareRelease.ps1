Set-Location $PSScriptRoot
Write-Host "Removing .gitattributes"
Remove-Item ./.gitattributes

Write-Host "Removing .gitignore"
Remove-Item ./.gitignore

Write-Host "Removing .git"
Remove-Item -Recurse -Force ./.git

Write-Host "Removing all source folders"
Get-ChildItem -Path $PSScriptRoot -Directory -Recurse -Filter Source |
    Remove-Item -Recurse -Force

Write-Host "Attempting to delete self"
$scriptPath = $MyInvocation.MyCommand.Path
Start-Process powershell -WindowStyle Hidden -ArgumentList @(
    '-NoProfile',
    '-Command',
    "Start-Sleep -Seconds 1; Remove-Item -LiteralPath '$scriptPath' -Force"
)
exit