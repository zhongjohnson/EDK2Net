// SPDX-License-Identifier: MIT
//
// EFI_SERIAL_IO_PROTOCOL — UEFI 1.x byte-level access to a serial port.
// Reference: MdePkg/Include/Protocol/SerialIo.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

public enum EfiParityType : uint
{
    Default = 0,
    None    = 1,
    Even    = 2,
    Odd     = 3,
    Mark    = 4,
    Space   = 5,
}

public enum EfiStopBitsType : uint
{
    Default      = 0,
    OneStopBit   = 1,
    OneFiveStopBits = 2,
    TwoStopBits  = 3,
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiSerialIoMode
{
    public uint  ControlMask;
    public uint  Timeout;       // microseconds
    public ulong BaudRate;
    public uint  ReceiveFifoDepth;
    public uint  DataBits;
    public uint  Parity;        // EfiParityType
    public uint  StopBits;      // EfiStopBitsType
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSerialIoProtocol
{
    public uint Revision;
    public delegate* unmanaged<EfiSerialIoProtocol*, EfiStatus> Reset;
    public delegate* unmanaged<EfiSerialIoProtocol*, ulong, uint, uint, EfiParityType, byte, EfiStopBitsType, EfiStatus> SetAttributes;
    public delegate* unmanaged<EfiSerialIoProtocol*, uint, EfiStatus> SetControl;
    public delegate* unmanaged<EfiSerialIoProtocol*, uint*, EfiStatus> GetControl;
    public delegate* unmanaged<EfiSerialIoProtocol*, nuint*, void*, EfiStatus> Write;
    public delegate* unmanaged<EfiSerialIoProtocol*, nuint*, void*, EfiStatus> Read;
    public EfiSerialIoMode* Mode;
}

/// <summary>Bits used by SetControl / GetControl on EFI_SERIAL_IO_PROTOCOL.</summary>
public static class EfiSerialControlBits
{
    public const uint DataTerminalReady   = 0x0001;
    public const uint RequestToSend       = 0x0002;
    public const uint ClearToSend         = 0x0010;
    public const uint DataSetReady        = 0x0020;
    public const uint RingIndicate        = 0x0040;
    public const uint CarrierDetect       = 0x0080;
    public const uint InputBufferEmpty    = 0x0100;
    public const uint OutputBufferEmpty   = 0x0200;
    public const uint HardwareLoopback    = 0x1000;
    public const uint SoftwareLoopback    = 0x2000;
    public const uint HardwareFlowControl = 0x4000;
}
