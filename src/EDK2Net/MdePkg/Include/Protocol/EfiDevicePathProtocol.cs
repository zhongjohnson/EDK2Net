// SPDX-License-Identifier: MIT
//
// EFI_DEVICE_PATH_PROTOCOL — variable-length linked list of device-path nodes.
// Reference: MdePkg/Include/Protocol/DevicePath.h.
//
// We bind only the per-node header (Type/SubType/Length). The full taxonomy
// of node bodies (HARDWARE_DEVICE_PATH, ACPI_DEVICE_PATH, MESSAGING_DEVICE_PATH,
// MEDIA_DEVICE_PATH, BBS_DEVICE_PATH ...) can be added incrementally as needed.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EfiDevicePathProtocol
{
    public byte Type;
    public byte SubType;
    /// <summary>Length of this node, in bytes, including this header (LE).</summary>
    public ushort Length;
}

/// <summary>Top-level node types from the UEFI spec.</summary>
public static class EfiDevicePathType
{
    public const byte Hardware  = 0x01;
    public const byte Acpi      = 0x02;
    public const byte Messaging = 0x03;
    public const byte Media     = 0x04;
    public const byte Bbs       = 0x05;
    public const byte EndOfPath = 0x7F;
}

public static class EfiDevicePathSubType
{
    public const byte EndEntireDevicePath   = 0xFF;
    public const byte EndThisInstance       = 0x01;
}

/// <summary>Helpers for walking and measuring device paths.</summary>
public static unsafe class EfiDevicePath
{
    /// <summary>True for END_ENTIRE / END_INSTANCE markers.</summary>
    public static bool IsEnd(EfiDevicePathProtocol* node)
        => node->Type == EfiDevicePathType.EndOfPath;

    /// <summary>True for the END_ENTIRE marker only.</summary>
    public static bool IsEndEntire(EfiDevicePathProtocol* node)
        => node->Type == EfiDevicePathType.EndOfPath
        && node->SubType == EfiDevicePathSubType.EndEntireDevicePath;

    /// <summary>Get the next node in the chain. Caller must check <see cref="IsEndEntire"/> first.</summary>
    public static EfiDevicePathProtocol* Next(EfiDevicePathProtocol* node)
        => (EfiDevicePathProtocol*)((byte*)node + node->Length);

    /// <summary>Total chain length in bytes, including the END_ENTIRE marker.</summary>
    public static nuint TotalSize(EfiDevicePathProtocol* head)
    {
        var n = head;
        while (!IsEndEntire(n)) n = Next(n);
        return (nuint)((byte*)n - (byte*)head) + n->Length;
    }
}
