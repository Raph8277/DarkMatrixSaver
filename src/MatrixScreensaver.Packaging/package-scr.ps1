$ErrorActionPreference = "Stop"
$root = Resolve-Path "$PSScriptRoot\..\.."
$wpf = Join-Path $root "src\MatrixScreensaver.Wpf"
$out = Join-Path $root "artifacts"
New-Item -ItemType Directory -Force -Path $out | Out-Null

dotnet publish $wpf -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true
$publish = Join-Path $wpf "bin\Release\net10.0-windows\win-x64\publish"
Copy-Item (Join-Path $publish "MatrixScreensaver.exe") (Join-Path $out "MatrixScreensaver.scr") -Force
Copy-Item (Join-Path $root "presets") $out -Recurse -Force
Write-Host "SCR généré : $out\MatrixScreensaver.scr"
