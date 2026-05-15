// SPDX-License-Identifier: MIT
//
// UefiLib — formatting helpers built on Char16String. Useful for printing
// numbers from a UEFI app without pulling in the managed string formatter
// (which would drag globalization, ICU, etc. into the published image).
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Library.BaseLib;
using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>Print an unsigned 64-bit value in decimal.</summary>
    public static EfiStatus PrintDec(ulong value)
    {
        Char16* buf = stackalloc Char16[24];
        Char16String.UInt64ToDec(value, buf, 24);
        return ConOut->OutputString(ConOut, buf);
    }

    /// <summary>Print a signed 64-bit value in decimal (with leading '-').</summary>
    public static EfiStatus PrintDec(long value)
    {
        if (value >= 0) return PrintDec((ulong)value);
        var s = Print("-");
        if (s.IsError) return s;
        // -long.MinValue overflows; cast through ulong to handle it.
        ulong abs = unchecked((ulong)(-value));
        return PrintDec(abs);
    }

    /// <summary>Print an unsigned value in hex (no 0x prefix; min width pad with '0').</summary>
    public static EfiStatus PrintHex(ulong value, int minWidth = 0, bool uppercase = true)
    {
        Char16* buf = stackalloc Char16[20];
        Char16String.UInt64ToHex(value, buf, 20, uppercase, minWidth);
        return ConOut->OutputString(ConOut, buf);
    }

    /// <summary>Print "0x" followed by hex digits.</summary>
    public static EfiStatus PrintHex0x(ulong value, int minWidth = 0, bool uppercase = true)
    {
        var s = Print("0x");
        if (s.IsError) return s;
        return PrintHex(value, minWidth, uppercase);
    }

    /// <summary>Print a NUL-terminated CHAR16* (raw pointer).</summary>
    public static EfiStatus Print(Char16* unicode)
    {
        if (unicode == null) return EfiStatus.Success;
        return ConOut->OutputString(ConOut, unicode);
    }

    /// <summary>
    /// Print a NUL-terminated ASCII (CHAR8) string by widening it through
    /// a small stack buffer in chunks. Avoids any heap allocation.
    /// </summary>
    public static EfiStatus PrintAscii(byte* ascii)
    {
        if (ascii == null) return EfiStatus.Success;
        const int Chunk = 64;
        Char16* buf = stackalloc Char16[Chunk];
        while (*ascii != 0)
        {
            int i = 0;
            while (i < Chunk - 1 && *ascii != 0)
            {
                buf[i++] = (Char16)(*ascii++);
            }
            buf[i] = (Char16)0;
            var s = ConOut->OutputString(ConOut, buf);
            if (s.IsError) return s;
        }
        return EfiStatus.Success;
    }

    /// <summary>Format a GUID like "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx".</summary>
    public static EfiStatus PrintGuid(in EfiGuid guid)
    {
        Char16* buf = stackalloc Char16[40];
        nuint w = 0;

        fixed (EfiGuid* g = &guid)
        {
            w += Char16String.UInt64ToHex(g->Data1, buf + w, 40 - w, true, 8);
            buf[w++] = (Char16)'-';
            w += Char16String.UInt64ToHex(g->Data2, buf + w, 40 - w, true, 4);
            buf[w++] = (Char16)'-';
            w += Char16String.UInt64ToHex(g->Data3, buf + w, 40 - w, true, 4);
            buf[w++] = (Char16)'-';

            byte* d4 = (byte*)&g->Data4_0;
            for (int i = 0; i < 8; i++)
            {
                w += Char16String.UInt64ToHex(d4[i], buf + w, 40 - w, true, 2);
                if (i == 1) buf[w++] = (Char16)'-';
            }
            buf[w] = (Char16)0;
        }
        return ConOut->OutputString(ConOut, buf);
    }
}
