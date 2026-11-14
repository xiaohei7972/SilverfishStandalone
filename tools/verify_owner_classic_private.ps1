#Requires -Version 7.0
<#
Upload pinned November 2026 classic source to a PRIVATE repository and
dispatch the independent verification workflow. Requires 'gh auth login'.

Example:
pwsh ./tools/verify_owner_classic_private.ps1 -ArchivePath "C:\Downloads\ClassicSilverfish_2026-11-05_阿格拉玛效果已注释_独立源码版.zip"
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$ArchivePath,
    [string]$Tag="classic-source-2026-11-05-private"
)
$ErrorActionPreference='Stop'
$privateRepo='xiaohei7972/Hearthbuddy_Silverfish'
$workflow='verify-classic-2026-11.yml'
$asset='ClassicSilverfish_2026-11-05_阿格拉玛效果已注释_独立源码版.zip'
$expected='4d6944304c0d224d366afda1691f0429c5d3cffbc0daae18259310a7f6f95d92'
$original='1879f784ed5b6854d53e148fc444d0f65bd8afbd52ccc1b3d8cf56e23efb5071'
if (-not (Get-Command gh -ErrorAction SilentlyContinue)) {
    throw 'GitHub CLI not installed. See https://cli.github.com/ and run gh auth login.'
}
$source=(Resolve-Path -LiteralPath $ArchivePath).ProviderPath
$sha=(Get-FileHash -LiteralPath $source -Algorithm SHA256).Hash.ToLowerInvariant()
if ($sha -ne $expected) { throw "Wrong source archive SHA256: $sha" }
$isPrivate=(& gh repo view $privateRepo --json isPrivate --jq '.isPrivate' | Out-String).Trim()
if ($LASTEXITCODE -ne 0 -or $isPrivate -ne 'true') {
    throw "Cannot verify target repository is PRIVATE: $privateRepo"
}
Add-Type -AssemblyName System.IO.Compression
$zip=[System.IO.Compression.ZipFile]::OpenRead($source)
try {
    $item=$zip.GetEntry('classic-import-manifest.json')
    if ($null -eq $item) { throw 'Source manifest missing.' }
    $reader=[System.IO.StreamReader]::new($item.Open())
    try { $manifest=$reader.ReadToEnd() | ConvertFrom-Json }
    finally { $reader.Dispose() }
    if ($manifest.sha256 -ne $original) { throw 'Original 2026-11-05 source hash mismatch.' }
    foreach ($name in @('Sim_TTN_092.cs','Sim_TTN_092t1.cs','Sim_TTN_092t2.cs','Sim_TTN_092t3.cs')) {
        $item=$zip.GetEntry('src/Silverfish.Classic/Imported/cards/1858-泰坦诸神/'+$name)
        if ($null -eq $item) { throw "Aggramar script missing: $name" }
        $reader=[System.IO.StreamReader]::new($item.Open())
        try { $script=$reader.ReadToEnd() }
        finally { $reader.Dispose() }
        if (-not $script.Contains('SILVERFISH_STANDALONE_AGGRAMAR_DISABLED')) {
            throw "Original Aggramar effect not disabled: $name"
        }
    }
}
finally { $zip.Dispose() }
$PSNativeCommandUseErrorActionPreference=$false
$ErrorActionPreference='Continue'
& gh release view $Tag --repo $privateRepo *> $null
$releaseExists=($LASTEXITCODE -eq 0)
$ErrorActionPreference='Stop'
if (-not $releaseExists) {
    & gh release create $Tag --repo $privateRepo --target main --title 'Private Silverfish 2026-11-05 CI fixture' --notes 'Private, pinned classic Silverfish source-only archive.' --prerelease
    if ($LASTEXITCODE -ne 0) { throw 'Private release creation failed.' }
}
& gh release upload $Tag ($source+'#'+$asset) --repo $privateRepo --clobber
if ($LASTEXITCODE -ne 0) { throw 'Private release upload failed.' }
& gh workflow run $workflow --repo $privateRepo --ref main -f ('release_tag='+$Tag) -f ('release_asset='+$asset)
if ($LASTEXITCODE -ne 0) { throw 'Private source uploaded, but workflow dispatch failed.' }
Write-Host "VERIFIED_SOURCE_SHA256=$sha"
Write-Host "Private latest-source CI started: https://github.com/$privateRepo/actions/workflows/$workflow"
