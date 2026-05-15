// SPDX-License-Identifier: MIT
//
// EDK2 MdePkg/Include/Uefi/UefiBaseType.h, UefiSpec.h scalars and bit constants.
namespace EDK2Net.MdePkg.Uefi;

using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------
// Numeric typedefs
// ---------------------------------------------------------------------------

/// <summary>EFI_LBA — Logical Block Address (UINT64).</summary>
public readonly struct EfiLba
{
    public readonly ulong Value;
    public EfiLba(ulong value) => Value = value;
    public static implicit operator ulong(EfiLba l) => l.Value;
    public static implicit operator EfiLba(ulong v) => new(v);
}

/// <summary>EFI_PHYSICAL_ADDRESS (UINT64).</summary>
public readonly struct EfiPhysicalAddress
{
    public readonly ulong Value;
    public EfiPhysicalAddress(ulong value) => Value = value;
    public static implicit operator ulong(EfiPhysicalAddress a) => a.Value;
    public static implicit operator EfiPhysicalAddress(ulong v) => new(v);
}

/// <summary>EFI_VIRTUAL_ADDRESS (UINT64).</summary>
public readonly struct EfiVirtualAddress
{
    public readonly ulong Value;
    public EfiVirtualAddress(ulong value) => Value = value;
    public static implicit operator ulong(EfiVirtualAddress a) => a.Value;
    public static implicit operator EfiVirtualAddress(ulong v) => new(v);
}

/// <summary>EFI_TPL — Task Priority Level (UINTN).</summary>
public readonly struct EfiTpl
{
    public readonly nuint Value;
    public EfiTpl(nuint value) => Value = value;
    public static implicit operator nuint(EfiTpl t) => t.Value;
    public static implicit operator EfiTpl(nuint v) => new(v);
}

/// <summary>Standard TPL levels from UefiSpec.h.</summary>
public static class EfiTplLevel
{
    public const nuint Application  = 4;
    public const nuint Callback     = 8;
    public const nuint Notify       = 16;
    public const nuint HighLevel    = 31;
}

// ---------------------------------------------------------------------------
// Network address types — used by NIC / IP / DNS protocols
// ---------------------------------------------------------------------------

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct EfiIPv4Address
{
    [FieldOffset(0)] public byte B0;
    [FieldOffset(1)] public byte B1;
    [FieldOffset(2)] public byte B2;
    [FieldOffset(3)] public byte B3;
}

[StructLayout(LayoutKind.Sequential, Size = 16)]
public unsafe struct EfiIPv6Address
{
    public fixed byte Bytes[16];
}

[StructLayout(LayoutKind.Sequential, Size = 32)]
public unsafe struct EfiMacAddress
{
    public fixed byte Bytes[32];
}

// ---------------------------------------------------------------------------
// Memory descriptor — used by GetMemoryMap / SetVirtualAddressMap
// ---------------------------------------------------------------------------

[StructLayout(LayoutKind.Sequential)]
public struct EfiMemoryDescriptor
{
    public uint                Type;            // EfiMemoryType
    public uint                Pad;             // alignment
    public EfiPhysicalAddress  PhysicalStart;
    public EfiVirtualAddress   VirtualStart;
    public ulong               NumberOfPages;
    public ulong               Attribute;       // EfiMemoryAttribute bits
}

/// <summary>Bits for <see cref="EfiMemoryDescriptor.Attribute"/>.</summary>
public static class EfiMemoryAttribute
{
    public const ulong Uc      = 0x0000000000000001;  // not cacheable
    public const ulong Wc      = 0x0000000000000002;  // write combining
    public const ulong Wt      = 0x0000000000000004;  // write through
    public const ulong Wb      = 0x0000000000000008;  // write back
    public const ulong UceUc   = 0x0000000000000010;
    public const ulong Wp      = 0x0000000000001000;  // write protected
    public const ulong Rp      = 0x0000000000002000;  // read protected
    public const ulong Xp      = 0x0000000000004000;  // execute protected
    public const ulong Nv      = 0x0000000000008000;  // non-volatile
    public const ulong MoreReliable = 0x0000000000010000;
    public const ulong Ro      = 0x0000000000020000;
    public const ulong SpecialPurpose = 0x0000000000040000;
    public const ulong CpuCrypto = 0x0000000000080000;
    public const ulong Runtime = 0x8000000000000000;
}

// ---------------------------------------------------------------------------
// CreateEvent flags
// ---------------------------------------------------------------------------

public static class EfiEventType
{
    public const uint Timer                       = 0x80000000;
    public const uint Runtime                     = 0x40000000;
    public const uint NotifyWait                  = 0x00000100;
    public const uint NotifySignal                = 0x00000200;
    public const uint SignalExitBootServices      = 0x00000201;
    public const uint SignalVirtualAddressChange  = 0x60000202;
}

public static class EfiTimerDelay
{
    public const uint Cancel   = 0;
    public const uint Periodic = 1;
    public const uint Relative = 2;
}

// ---------------------------------------------------------------------------
// OpenProtocol attribute bits
// ---------------------------------------------------------------------------

public static class EfiOpenProtocolAttributes
{
    public const uint ByHandleProtocol  = 0x00000001;
    public const uint GetProtocol       = 0x00000002;
    public const uint TestProtocol      = 0x00000004;
    public const uint ByChildController = 0x00000008;
    public const uint ByDriver          = 0x00000010;
    public const uint Exclusive         = 0x00000020;
}

// ---------------------------------------------------------------------------
// Variable attribute bits (used by GetVariable / SetVariable)
// ---------------------------------------------------------------------------

public static class EfiVariableAttribute
{
    public const uint NonVolatile                          = 0x00000001;
    public const uint BootserviceAccess                    = 0x00000002;
    public const uint RuntimeAccess                        = 0x00000004;
    public const uint HardwareErrorRecord                  = 0x00000008;
    public const uint AuthenticatedWriteAccess             = 0x00000010;  // deprecated
    public const uint TimeBasedAuthenticatedWriteAccess    = 0x00000020;
    public const uint AppendWrite                          = 0x00000040;
    public const uint EnhancedAuthenticatedAccess          = 0x00000080;
}

// ---------------------------------------------------------------------------
// SimpleTextInput / Output scan codes
// ---------------------------------------------------------------------------

public static class EfiScanCode
{
    public const ushort Null      = 0x0000;
    public const ushort Up        = 0x0001;
    public const ushort Down      = 0x0002;
    public const ushort Right     = 0x0003;
    public const ushort Left      = 0x0004;
    public const ushort Home      = 0x0005;
    public const ushort End       = 0x0006;
    public const ushort Insert    = 0x0007;
    public const ushort Delete    = 0x0008;
    public const ushort PageUp    = 0x0009;
    public const ushort PageDown  = 0x000A;
    public const ushort F1        = 0x000B;
    public const ushort F2        = 0x000C;
    public const ushort F3        = 0x000D;
    public const ushort F4        = 0x000E;
    public const ushort F5        = 0x000F;
    public const ushort F6        = 0x0010;
    public const ushort F7        = 0x0011;
    public const ushort F8        = 0x0012;
    public const ushort F9        = 0x0013;
    public const ushort F10       = 0x0014;
    public const ushort F11       = 0x0015;
    public const ushort F12       = 0x0016;
    public const ushort Escape    = 0x0017;
}

public static class EfiChar
{
    public const char Null       = '\0';
    public const char Backspace  = '\b';
    public const char Tab        = '\t';
    public const char LineFeed   = '\n';
    public const char CarriageReturn = '\r';
}
