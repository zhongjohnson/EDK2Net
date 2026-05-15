// SPDX-License-Identifier: MIT
//
// EFI_LOAD_OPTION — the on-disk layout of a UEFI Boot####/Driver####/etc.
// variable. Defined informally in the UEFI spec (section "Boot Manager").
//
// Variable bytes are laid out as:
//   UINT32  Attributes
//   UINT16  FilePathListLength       // bytes of the device-path blob
//   CHAR16  Description[]            // NUL-terminated
//   EFI_DEVICE_PATH_PROTOCOL FilePathList[FilePathListLength]
//   UINT8   OptionalData[]           // anything trailing
//
// Because the structure is variable-length, this binding only declares the
// fixed prefix; UefiLib provides parse helpers that surface the strings and
// device-path inside.
namespace EDK2Net.MdePkg.Uefi;

using System.Runtime.InteropServices;

/// <summary>Bits for the <c>Attributes</c> field of EFI_LOAD_OPTION.</summary>
public static class EfiLoadOptionAttribute
{
    public const uint Active           = 0x00000001;
    public const uint ForceReconnect   = 0x00000002;
    public const uint Hidden           = 0x00000008;
    public const uint CategoryMask     = 0x00001F00;
    public const uint CategoryBoot     = 0x00000000;
    public const uint CategoryApp      = 0x00000100;
}

/// <summary>Fixed prefix of an EFI_LOAD_OPTION blob.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EfiLoadOption
{
    public uint   Attributes;
    public ushort FilePathListLength;
    // Char16  Description[];           — variable-length, NUL-terminated
    // byte    FilePathList[Length];    — EFI_DEVICE_PATH_PROTOCOL chain
    // byte    OptionalData[];          — application-defined tail
}
