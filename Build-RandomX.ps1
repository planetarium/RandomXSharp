[CmdletBinding()]
param (
  [string]$RandomXVersion = "v1.1.8"
)

if (Test-Path native\randomx.dll) {
  Write-Warning `
    "native\randomx.dll already exists; this script will do nothing."
  exit 0
}

$url = "https://github.com/tevador/RandomX/archive/$RandomXVersion.zip"
$tmp = New-TemporaryFile
$tmpDir = New-TemporaryFile | %{ rm $_; mkdir $_ }
Invoke-WebRequest -OutFile "$tmp" "$url"
Write-Debug "The archive path: $tmp"
Write-Debug "The temporary directory to expand the archive: $tmpDir"
Expand-Archive -Path "$tmp" -DestinationPath "$tmpDir"
$slnDir = Get-Item $tmpDir\RandomX-*
Write-Debug "The solution directory: $slnDir"

Set-PSRepository -Name 'PSGallery' -InstallationPolicy Trusted
Install-Module `
  -Name Invoke-MsBuild `
  -MinimumVersion 2.6.2 `
  -MaximumVersion 2.6.999 `
  -SkipPublisherCheck `
  -AcceptLicense

$built = Invoke-MsBuild `
  -Path $slnDir\randomx.sln `
  -Parameters "-p:Configuration=Release" `
  -ErrorAction SilentlyContinue
$dllPath = "$slnDir\x64\Release\randomx.dll"
if (-Not (Test-Path $dllPath))
{
  Write-Error "Failed to build the randomx.dll.
See also the build log:
  ${built.BuildLogFilePath}
  ${built.BuildErrorsLogFilePath}"
  exit 1
}

Copy-Item $dllPath -Destination native\
Write-Host "native\randomx.dll has been made; check it out!"
