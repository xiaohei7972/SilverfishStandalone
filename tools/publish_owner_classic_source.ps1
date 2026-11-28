#Requires -Version 7.0
<#
.SYNOPSIS
  Upload the hash-verified 2026-11-05 classic Silverfish source ZIP as a
  GitHub Release asset, then start its independent build.

.EXAMPLE
  pwsh ./tools/publish_owner_classic_source.ps1 -ArchivePath "./ClassicSilverfish_2026-11-05_独立化优先_源码版.zip" -ConfirmPublic

This explicitly publishes the source-only ZIP because the repository is public.
#>
param(
    [Parameter(Mandatory=$true)][string]$ArchivePath,
    [switch]$ConfirmPublic,
    [string]$Tag = "classic-source-2026-11-05"
)

$ErrorActionPreference = "Stop"
$repo = "xiaohei7972/SilverfishStandalone"
$branch = "feat/standalone-foundation"
$asset = "ClassicSilverfish_2026-11-05_独立源码工作包.zip"
$acceptedSha = @(
    "ec1ff768036deee5ebda81274f6e1a61a69c105b0b6cd37b285c2d8dd34f771c"
    "81f6f19e2888ba5158ea194d6bea30d1f2ad90a7e67e7fd8565ffc7d2bff75e7"
    "4d6944304c0d224d366afda1691f0429c5d3cffbc0daae18259310a7f6f95d92"
)

if (-not $ConfirmPublic) {
    throw "The ZIP would be published as a PUBLIC GitHub Release asset. Add -ConfirmPublic only after reviewing its contents."
}
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    throw "GitHub CLI (gh) is required. Install it and run 'gh auth login'."
}
$source = (Resolve-Path -LiteralPath $ArchivePath).ProviderPath
$hash = (Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash.ToLowerInvariant()
if ($hash -notin $acceptedSha) {
    throw "Incorrect source ZIP. Expected one of the pinned November 2026 source hashes, got $hash."
}

Add-Type -AssemblyName System.IO.Compression
$zip = [System.IO.Compression.ZipFile]::OpenRead($source)
try {
    $names = @($zip.Entries | ForEach-Object { $_.FullName })
    if ($names -notcontains "src/Silverfish.Classic/Imported/ai/Ai.cs") {
        throw "Archive is not the verified standalone classic Silverfish source package."
    }
    if ($names -notcontains "src/Silverfish.Classic/Imported/ai/Minion.cs") {
        throw "Archive does not contain Minion.cs."
    }
    if (@($names | Where-Object { $_ -match '(?i)(silverfish_v2|\.exe$|\.dll$)' }).Count -gt 0) {
        throw "Refusing to upload V2 code or game executables."
    }
}
finally { $zip.Dispose() }

& gh auth status
if ($LASTEXITCODE -ne 0) { throw "GitHub CLI is not authenticated." }

& gh release view $Tag --repo $repo 2>$null | Out-Null
if ($LASTEXITCODE -ne 0) {
    & gh release create $Tag --repo $repo --target $branch --title "Classic Silverfish 2026-11-05 source build fixture" --notes "SHA-256-pinned classic Silverfish source-only archive for .NET 8 independent build verification." --prerelease
    if ($LASTEXITCODE -ne 0) { throw "Failed to create release." }
}
& gh release upload $Tag "${source}#${asset}" --repo $repo --clobber
if ($LASTEXITCODE -ne 0) { throw "Failed to upload source ZIP." }

& gh workflow run classic-compile.yml --repo $repo --ref $branch -f source_mode=owner-classic-work-package -f "release_tag=$Tag" -f "release_asset=$asset"
if ($LASTEXITCODE -ne 0) { throw "Release uploaded, but workflow dispatch failed." }

Write-Host "Uploaded verified source ZIP and dispatched the 2026-11-05 compilation job."
Write-Host "https://github.com/$repo/actions/workflows/classic-compile.yml"
