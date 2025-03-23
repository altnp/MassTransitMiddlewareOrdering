$targetFolder = "MassTransitMiddlewareOrdering"
$currentLocation = (Get-Location).Path

if ($currentLocation -notmatch [regex]::Escape($targetFolder)) {
  Write-Host "Error: This script must be run from within '$targetFolder'." -ForegroundColor Red
  exit 1
}

dotnet ef database update --project "gateway"
dotnet ef database update --project "profile"
