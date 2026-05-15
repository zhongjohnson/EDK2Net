// SPDX-License-Identifier: MIT
//
// EFI_TIMESTAMP_PROTOCOL — UEFI 2.5+ high-resolution monotonic counter.
// Backed by HPET / TSC / generic timer depending on platform.
//
// Reference: MdePkg/Include/Protocol/Timestamp.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiTimestampProperties
{
    /// <summary>Counter ticks per second.</summary>
    public ulong Frequency;
    /// <summary>Largest counter value before wrap-around (e.g. 0xFFFFFFFFFFFFFFFF for 64-bit).</summary>
    public ulong EndValue;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiTimestampProtocol
{
    /// <summary>Read the current free-running counter value.</summary>
    public delegate* unmanaged<ulong> GetTimestamp;

    /// <summary>Get counter frequency / wrap value.</summary>
    public delegate* unmanaged<EfiTimestampProperties*, EfiStatus> GetProperties;
}
