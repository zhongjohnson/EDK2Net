#requires -Version 5.1
<#
.SYNOPSIS
    Boots a built EDK2Net .efi image under QEMU + OVMF.

.DESCRIPTION
    Stages the produced .efi file as EFI/BOOT/BOOTX64.EFI inside a temporary
    FAT-image directory and tells QEMU to use it as a virtual disk along with
    the OVMF firmware. The application's text output appears in QEMU's
    serial console.

.PARAMETER Image
    Path to the published .efi file
    (typically samples/HelloUefi/bin/Release/net9.0/win-x64/publish/HelloUefi.efi).

.PARAMETER Ovmf
    Path to OVMF.fd (or OVMF_CODE.fd). Defaults to $env:OVMF_FD if set.

.PARAMETER Qemu
    qemu-system-x86_64 executable. Defaults to the one on PATH.

.EXAMPLE
    .\build\run-qemu.ps1 -Image .\samples\HelloUefi\bin\Release\net9.0\win-x64\publish\HelloUefi.efi
#>

param(
    [Parameter(Mandatory = $true)]
    [string]$Image,

    [string]$Ovmf = $env:OVMF_FD,

    [string]$Qemu = 'qemu-system-x86_64'
)

if (-not (Test-Path $Image)) {
    throw "EFI image not found: $Image"
}
if (-not $Ovmf -or -not (Test-Path $Ovmf)) {
    throw "OVMF firmware not found. Pass -Ovmf <path-to-OVMF.fd> or set OVMF_FD."
}

$stage = Join-Path ([IO.Path]::GetTempPath()) ("edk2net-" + [guid]::NewGuid())
$boot  = Join-Path $stage 'EFI\BOOT'
New-Item -ItemType Directory -Force -Path $boot | Out-Null
Copy-Item -Force $Image (Join-Path $boot 'BOOTX64.EFI')

Write-Host "Staged image at $stage"
Write-Host "Launching QEMU..."

& $Qemu `
    -machine q35 `
    -m 256 `
    -bios $Ovmf `
    -drive "file=fat:rw:$stage,format=raw,if=ide" `
    -net none `
    -serial stdio
