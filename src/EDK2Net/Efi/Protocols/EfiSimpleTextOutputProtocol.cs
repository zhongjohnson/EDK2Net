// SPDX-License-Identifier: MIT
// EFI_SIMPLE_TEXT_OUTPUT_PROTOCOL — text console output (ConOut / StdErr).

namespace EDK2Net.Efi.Protocols;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiSimpleTextOutputMode
{
    public int    MaxMode;
    public int    Mode;
    public int    Attribute;
    public int    CursorColumn;
    public int    CursorRow;
    public byte   CursorVisible;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSimpleTextOutputProtocol
{
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, byte, EfiStatus>           Reset;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, Char16*, EfiStatus>        OutputString;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, Char16*, EfiStatus>        TestString;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, nuint, nuint*, nuint*, EfiStatus> QueryMode;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, nuint, EfiStatus>          SetMode;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, nuint, EfiStatus>          SetAttribute;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, EfiStatus>                 ClearScreen;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, nuint, nuint, EfiStatus>   SetCursorPosition;
    public delegate* unmanaged<EfiSimpleTextOutputProtocol*, byte, EfiStatus>           EnableCursor;
    public EfiSimpleTextOutputMode* Mode;
}

/// <summary>Foreground/background text attribute helpers.</summary>
public static class EfiTextAttribute
{
    public const int Black        = 0x00;
    public const int Blue         = 0x01;
    public const int Green        = 0x02;
    public const int Cyan         = 0x03;
    public const int Red          = 0x04;
    public const int Magenta      = 0x05;
    public const int Brown        = 0x06;
    public const int LightGray    = 0x07;
    public const int DarkGray     = 0x08;
    public const int LightBlue    = 0x09;
    public const int LightGreen   = 0x0A;
    public const int LightCyan    = 0x0B;
    public const int LightRed     = 0x0C;
    public const int LightMagenta = 0x0D;
    public const int Yellow       = 0x0E;
    public const int White        = 0x0F;

    public static int Background(int color) => (color & 0x7) << 4;
}
