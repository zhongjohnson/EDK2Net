// SPDX-License-Identifier: MIT
//
// EFI_SIMPLE_POINTER_PROTOCOL — UEFI pointing-device input (mouse, touchpad).
// Provides relative-motion deltas plus button state, and a WaitForInput event
// usable with BootServices->WaitForEvent.
//
// Reference: MdePkg/Include/Protocol/SimplePointer.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

/// <summary>EFI_SIMPLE_POINTER_MODE — capabilities reported by the device.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct EfiSimplePointerMode
{
    /// <summary>Resolution on the X axis in counts/mm.</summary>
    public ulong ResolutionX;
    /// <summary>Resolution on the Y axis in counts/mm.</summary>
    public ulong ResolutionY;
    /// <summary>Resolution on the Z axis in counts/mm (0 if no Z).</summary>
    public ulong ResolutionZ;
    /// <summary>TRUE (1) if a left button is present.</summary>
    public byte  LeftButton;
    /// <summary>TRUE (1) if a right button is present.</summary>
    public byte  RightButton;
}

/// <summary>EFI_SIMPLE_POINTER_STATE — current relative motion + buttons.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct EfiSimplePointerState
{
    /// <summary>Signed relative X movement since last GetState.</summary>
    public int  RelativeMovementX;
    /// <summary>Signed relative Y movement since last GetState.</summary>
    public int  RelativeMovementY;
    /// <summary>Signed relative Z movement since last GetState.</summary>
    public int  RelativeMovementZ;
    /// <summary>TRUE (1) if the left button is currently pressed.</summary>
    public byte LeftButton;
    /// <summary>TRUE (1) if the right button is currently pressed.</summary>
    public byte RightButton;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSimplePointerProtocol
{
    /// <summary>Reset(this, ExtendedVerification) — re-initialize the device.</summary>
    public delegate* unmanaged<EfiSimplePointerProtocol*, byte, EfiStatus> Reset;

    /// <summary>GetState(this, *State) — returns EFI_NOT_READY if no new data
    /// since last call, otherwise fills <paramref name="State"/>.</summary>
    public delegate* unmanaged<EfiSimplePointerProtocol*, EfiSimplePointerState*, EfiStatus> GetState;

    /// <summary>Event signaled when input is available; usable with WaitForEvent.</summary>
    public EfiEvent WaitForInput;

    /// <summary>Pointer to the device's capability descriptor.</summary>
    public EfiSimplePointerMode* Mode;
}
