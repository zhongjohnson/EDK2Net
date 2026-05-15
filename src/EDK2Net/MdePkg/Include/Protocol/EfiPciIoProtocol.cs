// SPDX-License-Identifier: MIT
//
// EFI_PCI_IO_PROTOCOL — UEFI access to a PCI device's BAR I/O, config space,
// DMA, and interrupt acknowledgment. Bound by the PCI bus driver onto every
// discovered PCI function handle.
//
// Reference: MdePkg/Include/Protocol/PciIo.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

public enum EfiPciIoWidth : uint
{
    Uint8       = 0,
    Uint16      = 1,
    Uint32      = 2,
    Uint64      = 3,
    FifoUint8   = 4,
    FifoUint16  = 5,
    FifoUint32  = 6,
    FifoUint64  = 7,
    FillUint8   = 8,
    FillUint16  = 9,
    FillUint32  = 10,
    FillUint64  = 11,
}

public enum EfiPciIoOperation : uint
{
    BusMasterRead   = 0,
    BusMasterWrite  = 1,
    BusMasterCommonBuffer = 2,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiPciIoAccess
{
    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoWidth, byte, ulong, nuint, void*, EfiStatus> Read;
    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoWidth, byte, ulong, nuint, void*, EfiStatus> Write;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiPciIoConfigAccess
{
    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoWidth, uint, nuint, void*, EfiStatus> Read;
    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoWidth, uint, nuint, void*, EfiStatus> Write;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiPciIoProtocol
{
    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoWidth, byte, ulong, nuint, void*, void*, EfiStatus> PollMem;
    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoWidth, byte, ulong, nuint, void*, void*, EfiStatus> PollIo;

    public EfiPciIoAccess        Mem;
    public EfiPciIoAccess        Io;
    public EfiPciIoConfigAccess  Pci;

    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoWidth, byte, ulong, byte, ulong, nuint, EfiStatus> CopyMem;
    public delegate* unmanaged<EfiPciIoProtocol*, EfiPciIoOperation, void*, nuint, ulong*, nuint*, void**, EfiStatus> Map;
    public delegate* unmanaged<EfiPciIoProtocol*, void*, EfiStatus> Unmap;
    public delegate* unmanaged<EfiPciIoProtocol*, EfiMemoryType, nuint, void**, ulong, EfiStatus> AllocateBuffer;
    public delegate* unmanaged<EfiPciIoProtocol*, nuint, void*, EfiStatus> FreeBuffer;
    public delegate* unmanaged<EfiPciIoProtocol*, EfiStatus> Flush;
    public delegate* unmanaged<EfiPciIoProtocol*, nuint*, nuint*, nuint*, nuint*, EfiStatus> GetLocation;
    public delegate* unmanaged<EfiPciIoProtocol*, uint, byte, EfiStatus> Attributes;
    public delegate* unmanaged<EfiPciIoProtocol*, byte, ulong*, ulong*, EfiStatus> GetBarAttributes;
    public delegate* unmanaged<EfiPciIoProtocol*, ulong, byte, ulong*, ulong*, EfiStatus> SetBarAttributes;

    public ulong RomSize;
    public void* RomImage;
}

/// <summary>EFI_PCI_IO_ATTRIBUTE_OPERATION values.</summary>
public static class EfiPciIoAttributeOperation
{
    public const uint Get        = 0;
    public const uint Set        = 1;
    public const uint Enable     = 2;
    public const uint Disable    = 3;
    public const uint Supported  = 4;
}
