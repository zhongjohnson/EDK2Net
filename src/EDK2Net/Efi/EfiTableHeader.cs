// SPDX-License-Identifier: MIT
// EFI_TABLE_HEADER — common header for SystemTable / BootServices / RuntimeServices.

namespace EDK2Net.Efi;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiTableHeader
{
    public ulong Signature;
    public uint  Revision;
    public uint  HeaderSize;
    public uint  Crc32;
    public uint  Reserved;
}
