// SPDX-License-Identifier: MIT
// CHAR16 — 16-bit UCS-2 character used everywhere in UEFI string APIs.
namespace EDK2Net.MdePkg.Uefi;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, Size = 2)]
public readonly struct Char16
{
    public readonly ushort Value;
    public Char16(ushort value) => Value = value;
    public Char16(char c) => Value = c;

    public static implicit operator Char16(char c) => new(c);
    public static implicit operator char(Char16 c) => (char)c.Value;
    public static implicit operator ushort(Char16 c) => c.Value;

    public static explicit operator Char16(ushort v) => new(v);
    public static explicit operator Char16(int v)    => new((ushort)v);

    public static bool operator ==(Char16 a, Char16 b) => a.Value == b.Value;
    public static bool operator !=(Char16 a, Char16 b) => a.Value != b.Value;
    public override bool Equals(object? obj) => obj is Char16 o && o.Value == Value;
    public override int GetHashCode() => Value;
}
