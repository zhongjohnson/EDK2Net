// SPDX-License-Identifier: MIT
//
// EFI_ABSOLUTE_POINTER_PROTOCOL — UEFI 2.x absolute pointing-device input
// (touchscreens, digitizers, light pens). Unlike EFI_SIMPLE_POINTER, which
// reports relative deltas, this protocol reports an absolute coordinate
// within the device's bounding box.
//
// Reference: MdePkg/Include/Protocol/AbsolutePointer.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

/// <summary>EFI_ABSOLUTE_POINTER_MODE — device capability descriptor.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct EfiAbsolutePointerMode
{
    /// <summary>Smallest X coordinate the device reports (typically 0).</summary>
    public ulong AbsoluteMinX;
    /// <summary>Smallest Y coordinate the device reports (typically 0).</summary>
    public ulong AbsoluteMinY;
    /// <summary>Smallest Z coordinate (pressure/depth); 0 if not supported.</summary>
    public ulong AbsoluteMinZ;
    /// <summary>Largest X coordinate the device reports (e.g. screen width − 1).</summary>
    public ulong AbsoluteMaxX;
    /// <summary>Largest Y coordinate.</summary>
    public ulong AbsoluteMaxY;
    /// <summary>Largest Z coordinate (pressure/depth); 0 if not supported.</summary>
    public ulong AbsoluteMaxZ;
    /// <summary>Bitmask of <see cref="EfiAbsolutePointerAttributes"/> bits.</summary>
    public uint  Attributes;
}

/// <summary>Bits for <see cref="EfiAbsolutePointerMode.Attributes"/> and
/// <see cref="EfiAbsolutePointerState.ActiveButtons"/>.</summary>
public static class EfiAbsolutePointerAttributes
{
    /// <summary>Device supports an "alternate active" button (e.g. stylus barrel).</summary>
    public const uint SupportsAltActive   = 0x00000001;
    /// <summary>Device reports pressure (Z) data.</summary>
    public const uint SupportsPressureAsZ = 0x00000002;

    // ActiveButtons bits.
    /// <summary>Touch contact / primary button is currently active.</summary>
    public const uint TouchActive = 0x00000001;
    /// <summary>Alternate (e.g. stylus barrel) button is currently active.</summary>
    public const uint AltActive   = 0x00000002;
}

/// <summary>EFI_ABSOLUTE_POINTER_STATE — current absolute coordinates +
/// button/contact state.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct EfiAbsolutePointerState
{
    public ulong CurrentX;
    public ulong CurrentY;
    /// <summary>Current Z value (pressure or depth) when supported.</summary>
    public ulong CurrentZ;
    /// <summary>Bitmask of active buttons; see
    /// <see cref="EfiAbsolutePointerAttributes.TouchActive"/>,
    /// <see cref="EfiAbsolutePointerAttributes.AltActive"/>.</summary>
    public uint  ActiveButtons;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiAbsolutePointerProtocol
{
    /// <summary>Reset(this, ExtendedVerification) — re-initialize the device.</summary>
    public delegate* unmanaged<EfiAbsolutePointerProtocol*, byte, EfiStatus> Reset;

    /// <summary>GetState(this, *State) — returns EFI_NOT_READY if no new data
    /// since the last call, otherwise fills <paramref name="State"/>.</summary>
    public delegate* unmanaged<EfiAbsolutePointerProtocol*, EfiAbsolutePointerState*, EfiStatus> GetState;

    /// <summary>Event signaled when input is available; usable with WaitForEvent.</summary>
    public EfiEvent WaitForInput;

    /// <summary>Pointer to the device's capability descriptor.</summary>
    public EfiAbsolutePointerMode* Mode;
}
