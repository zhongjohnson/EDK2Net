// SPDX-License-Identifier: MIT
// EFI_BLOCK_IO_PROTOCOL — block-level read/write/flush.
// Reference: MdePkg/Include/Protocol/BlockIo.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiBlockIoMedia
{
    public uint   MediaId;
    public byte   RemovableMedia;
    public byte   MediaPresent;
    public byte   LogicalPartition;
    public byte   ReadOnly;
    public byte   WriteCaching;
    // padding to align next field
    public byte   Pad0;
    public byte   Pad1;
    public byte   Pad2;
    public uint   BlockSize;
    public uint   IoAlign;
    public EfiLba LastBlock;

    // Revision 2 additions (UEFI 2.x)
    public EfiLba LowestAlignedLba;
    public uint   LogicalBlocksPerPhysicalBlock;
    public uint   OptimalTransferLengthGranularity;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiBlockIoProtocol
{
    public ulong Revision;
    public EfiBlockIoMedia* Media;

    public delegate* unmanaged<EfiBlockIoProtocol*, byte, EfiStatus>                            Reset;
    public delegate* unmanaged<EfiBlockIoProtocol*, uint, EfiLba, nuint, void*, EfiStatus>      ReadBlocks;
    public delegate* unmanaged<EfiBlockIoProtocol*, uint, EfiLba, nuint, void*, EfiStatus>      WriteBlocks;
    public delegate* unmanaged<EfiBlockIoProtocol*, EfiStatus>                                  FlushBlocks;

    public const ulong Revision1 = 0x00010000;
    public const ulong Revision2 = 0x00020001;
    public const ulong Revision3 = 0x00020031;
}
