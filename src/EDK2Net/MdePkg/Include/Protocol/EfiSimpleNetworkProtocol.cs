// SPDX-License-Identifier: MIT
//
// EFI_SIMPLE_NETWORK_PROTOCOL — UEFI 2.x raw-Ethernet NIC abstraction.
// Reference: MdePkg/Include/Protocol/SimpleNetwork.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

public enum EfiSimpleNetworkState : uint
{
    Stopped     = 0,
    Started     = 1,
    Initialized = 2,
    Max         = 3,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSimpleNetworkMode
{
    public uint  State;                  // EfiSimpleNetworkState
    public uint  HwAddressSize;
    public uint  MediaHeaderSize;
    public uint  MaxPacketSize;
    public uint  NvRamSize;
    public uint  NvRamAccessSize;
    public uint  ReceiveFilterMask;
    public uint  ReceiveFilterSetting;
    public uint  MaxMCastFilterCount;
    public uint  MCastFilterCount;
    public fixed byte MCastFilter[16 * 32];
    public EfiMacAddress CurrentAddress;
    public EfiMacAddress BroadcastAddress;
    public EfiMacAddress PermanentAddress;
    public byte  IfType;
    public byte  MacAddressChangeable;
    public byte  MultipleTxSupported;
    public byte  MediaPresentSupported;
    public byte  MediaPresent;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiNetworkStatistics
{
    public ulong RxTotalFrames;
    public ulong RxGoodFrames;
    public ulong RxUndersizeFrames;
    public ulong RxOversizeFrames;
    public ulong RxDroppedFrames;
    public ulong RxUnicastFrames;
    public ulong RxBroadcastFrames;
    public ulong RxMulticastFrames;
    public ulong RxCrcErrorFrames;
    public ulong RxTotalBytes;
    public ulong TxTotalFrames;
    public ulong TxGoodFrames;
    public ulong TxUndersizeFrames;
    public ulong TxOversizeFrames;
    public ulong TxDroppedFrames;
    public ulong TxUnicastFrames;
    public ulong TxBroadcastFrames;
    public ulong TxMulticastFrames;
    public ulong TxCrcErrorFrames;
    public ulong TxTotalBytes;
    public ulong Collisions;
    public ulong UnsupportedProtocol;
    public ulong RxDuplicatedFrames;
    public ulong RxDecryptErrorFrames;
    public ulong TxErrorFrames;
    public ulong TxRetryFrames;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSimpleNetworkProtocol
{
    public ulong Revision;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, EfiStatus>                                 Start;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, EfiStatus>                                 Stop;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, nuint, nuint, EfiStatus>                   Initialize;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, byte, EfiStatus>                           Reset;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, EfiStatus>                                 Shutdown;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, uint, uint, byte, nuint, EfiMacAddress*, EfiStatus> ReceiveFilters;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, byte, EfiMacAddress*, EfiStatus>           StationAddress;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, byte, nuint*, EfiNetworkStatistics*, EfiStatus> Statistics;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, byte, EfiIPv4Address*, EfiMacAddress*, EfiStatus> McastIpToMac;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, byte, byte, nuint, nuint, void*, EfiStatus> NvData;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, uint*, void**, EfiStatus>                  GetStatus;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, nuint, nuint, void*, EfiMacAddress*, EfiMacAddress*, ushort*, EfiStatus> Transmit;
    public delegate* unmanaged<EfiSimpleNetworkProtocol*, nuint*, nuint*, void*, EfiMacAddress*, EfiMacAddress*, ushort*, EfiStatus> Receive;
    public EfiEvent WaitForPacket;
    public EfiSimpleNetworkMode* Mode;

    public const ulong RevisionValue = 0x00010000;
}

/// <summary>Bits used by ReceiveFilters / Mode->ReceiveFilterMask.</summary>
public static class EfiSimpleNetworkReceiveFilter
{
    public const uint Unicast              = 0x01;
    public const uint Multicast            = 0x02;
    public const uint Broadcast            = 0x04;
    public const uint Promiscuous          = 0x08;
    public const uint PromiscuousMulticast = 0x10;
}

/// <summary>Bits returned by GetStatus.InterruptStatus.</summary>
public static class EfiSimpleNetworkInterrupt
{
    public const uint ReceiveInterrupt  = 0x01;
    public const uint TransmitInterrupt = 0x02;
    public const uint CommandInterrupt  = 0x04;
    public const uint SoftwareInterrupt = 0x08;
}
