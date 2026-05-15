// SPDX-License-Identifier: MIT
// EFI_GUID — 128-bit GUID matching the EDK2 layout.

namespace EDK2Net.Efi;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
public struct EfiGuid
{
    public uint   Data1;
    public ushort Data2;
    public ushort Data3;
    public byte   Data4_0;
    public byte   Data4_1;
    public byte   Data4_2;
    public byte   Data4_3;
    public byte   Data4_4;
    public byte   Data4_5;
    public byte   Data4_6;
    public byte   Data4_7;

    public EfiGuid(
        uint d1, ushort d2, ushort d3,
        byte b0, byte b1, byte b2, byte b3,
        byte b4, byte b5, byte b6, byte b7)
    {
        Data1 = d1; Data2 = d2; Data3 = d3;
        Data4_0 = b0; Data4_1 = b1; Data4_2 = b2; Data4_3 = b3;
        Data4_4 = b4; Data4_5 = b5; Data4_6 = b6; Data4_7 = b7;
    }
}
