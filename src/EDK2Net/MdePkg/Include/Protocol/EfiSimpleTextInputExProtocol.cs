// SPDX-License-Identifier: MIT
//
// EFI_SIMPLE_TEXT_INPUT_EX_PROTOCOL — UEFI 2.0 extended console input that
// can report modifier-key state, partial keystrokes, and register typed-key
// notifications.
//
// Reference: MdePkg/Include/Protocol/SimpleTextInEx.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiKeyState
{
    public uint KeyShiftState;
    public byte KeyToggleState;
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiKeyData
{
    public EfiInputKey Key;
    public EfiKeyState KeyState;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSimpleTextInputExProtocol
{
    public delegate* unmanaged<EfiSimpleTextInputExProtocol*, byte, EfiStatus>                Reset;
    public delegate* unmanaged<EfiSimpleTextInputExProtocol*, EfiKeyData*, EfiStatus>         ReadKeyStrokeEx;
    public EfiEvent WaitForKeyEx;
    public delegate* unmanaged<EfiSimpleTextInputExProtocol*, EfiKeyToggleState*, EfiStatus>  SetState;
    public delegate* unmanaged<EfiSimpleTextInputExProtocol*, EfiKeyData*, void*, void*, EfiStatus> RegisterKeyNotify;
    public delegate* unmanaged<EfiSimpleTextInputExProtocol*, void*, EfiStatus>               UnregisterKeyNotify;
}

public readonly struct EfiKeyToggleState
{
    public readonly byte Value;
    public EfiKeyToggleState(byte v) => Value = v;
    public static implicit operator byte(EfiKeyToggleState s) => s.Value;
    public static implicit operator EfiKeyToggleState(byte v) => new(v);
}

/// <summary>Bits for <see cref="EfiKeyState.KeyShiftState"/>.</summary>
public static class EfiKeyShiftState
{
    public const uint Valid           = 0x80000000;
    public const uint RightShift      = 0x00000001;
    public const uint LeftShift       = 0x00000002;
    public const uint RightControl    = 0x00000004;
    public const uint LeftControl     = 0x00000008;
    public const uint RightAlt        = 0x00000010;
    public const uint LeftAlt         = 0x00000020;
    public const uint RightLogo       = 0x00000040;
    public const uint LeftLogo        = 0x00000080;
    public const uint Menu            = 0x00000100;
    public const uint SysReq          = 0x00000200;
}

/// <summary>Bits for <see cref="EfiKeyState.KeyToggleState"/>.</summary>
public static class EfiKeyToggleStateBits
{
    public const byte StateValid     = 0x80;
    public const byte KeyStateExposed = 0x40;
    public const byte ScrollLockActive = 0x01;
    public const byte NumLockActive    = 0x02;
    public const byte CapsLockActive   = 0x04;
}
