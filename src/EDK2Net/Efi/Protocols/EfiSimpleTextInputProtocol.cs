// SPDX-License-Identifier: MIT
// EFI_SIMPLE_TEXT_INPUT_PROTOCOL — keyboard input (ConIn).

namespace EDK2Net.Efi.Protocols;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiInputKey
{
    public ushort ScanCode;
    public Char16 UnicodeChar;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSimpleTextInputProtocol
{
    public delegate* unmanaged<EfiSimpleTextInputProtocol*, byte, EfiStatus>           Reset;
    public delegate* unmanaged<EfiSimpleTextInputProtocol*, EfiInputKey*, EfiStatus>   ReadKeyStroke;
    public EfiEvent WaitForKey;
}
