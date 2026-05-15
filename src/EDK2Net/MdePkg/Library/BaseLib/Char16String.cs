// SPDX-License-Identifier: MIT
//
// Char16String
// ============
// AOT/UEFI-safe helpers for working with raw NUL-terminated CHAR16* strings,
// in the spirit of MdePkg/Library/BaseLib (StrLen/StrCmp/StrCpyS/...) plus a
// few formatting helpers (decimal/hex) that don't allocate.
//
// All routines are pointer-only — no `string` / `Span` / `StringBuilder` —
// which keeps them usable from UEFI applications that have stripped most of
// the managed BCL.
namespace EDK2Net.MdePkg.Library.BaseLib;

using System.Runtime.CompilerServices;

public static unsafe class Char16String
{
    /// <summary>Number of CHAR16 units before the terminating NUL.</summary>
    public static nuint StrLen(Char16* s)
    {
        if (s == null) return 0;
        nuint n = 0;
        while (s[n].Value != 0) n++;
        return n;
    }

    /// <summary>Length in bytes including the trailing NUL CHAR16.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static nuint StrSize(Char16* s) => (StrLen(s) + 1) * sizeof(char);

    /// <summary>Lexicographic compare (UTF-16 code-unit ordinal).</summary>
    public static int StrCmp(Char16* a, Char16* b)
    {
        if (a == b) return 0;
        if (a == null) return -1;
        if (b == null) return  1;
        while (true)
        {
            int ca = a->Value;
            int cb = b->Value;
            if (ca != cb) return ca - cb;
            if (ca == 0)  return 0;
            a++; b++;
        }
    }

    /// <summary>Compare at most <paramref name="length"/> CHAR16 units.</summary>
    public static int StrnCmp(Char16* a, Char16* b, nuint length)
    {
        for (nuint i = 0; i < length; i++)
        {
            int ca = a[i].Value;
            int cb = b[i].Value;
            if (ca != cb) return ca - cb;
            if (ca == 0)  return 0;
        }
        return 0;
    }

    /// <summary>Copy src into dst (must hold at least StrLen(src)+1 chars).</summary>
    public static void StrCpy(Char16* dst, Char16* src)
    {
        nuint i = 0;
        while ((dst[i] = src[i]).Value != 0) i++;
    }

    /// <summary>Append src to the NUL-terminated dst (caller sized).</summary>
    public static void StrCat(Char16* dst, Char16* src)
    {
        StrCpy(dst + StrLen(dst), src);
    }

    // ----------------------------------------------------------------------
    // Formatters — write into a caller-provided CHAR16 buffer and NUL-term.
    // Return number of CHAR16 units written (excluding the terminating NUL).
    // ----------------------------------------------------------------------

    /// <summary>Format <paramref name="value"/> as unsigned decimal.</summary>
    public static nuint UInt64ToDec(ulong value, Char16* buffer, nuint bufferChars)
    {
        if (bufferChars == 0) return 0;
        if (value == 0)
        {
            if (bufferChars < 2) { buffer[0] = (Char16)0; return 0; }
            buffer[0] = (Char16)'0';
            buffer[1] = (Char16)0;
            return 1;
        }

        // Build digits backwards into a small stack array (max 20 digits for ulong).
        Char16* tmp = stackalloc Char16[20];
        int t = 0;
        while (value != 0)
        {
            tmp[t++] = (Char16)('0' + (int)(value % 10));
            value  /= 10;
        }

        nuint w = 0;
        while (t > 0 && w + 1 < bufferChars)
            buffer[w++] = tmp[--t];
        buffer[w] = (Char16)0;
        return w;
    }

    /// <summary>Format <paramref name="value"/> as unsigned hex (no 0x prefix).</summary>
    public static nuint UInt64ToHex(ulong value, Char16* buffer, nuint bufferChars,
                                    bool uppercase = true, int minWidth = 0)
    {
        if (bufferChars == 0) return 0;

        Char16* tmp = stackalloc Char16[16];
        int t = 0;
        if (value == 0)
        {
            tmp[t++] = (Char16)'0';
        }
        else
        {
            while (value != 0)
            {
                int nibble = (int)(value & 0xF);
                tmp[t++] = (Char16)(nibble < 10
                    ? '0' + nibble
                    : (uppercase ? 'A' : 'a') + (nibble - 10));
                value >>= 4;
            }
        }
        // Pad with leading zeros to minWidth.
        while (t < minWidth && t < 16) tmp[t++] = (Char16)'0';

        nuint w = 0;
        while (t > 0 && w + 1 < bufferChars)
            buffer[w++] = tmp[--t];
        buffer[w] = (Char16)0;
        return w;
    }

    /// <summary>
    /// Widen a NUL-terminated ASCII (CHAR8) string into a caller-provided
    /// CHAR16 buffer. Returns CHAR16 count written excluding NUL.
    /// </summary>
    public static nuint AsciiToUnicode(byte* ascii, Char16* buffer, nuint bufferChars)
    {
        if (bufferChars == 0) return 0;
        nuint w = 0;
        while (w + 1 < bufferChars && ascii[w] != 0)
        {
            buffer[w] = (Char16)ascii[w];
            w++;
        }
        buffer[w] = (Char16)0;
        return w;
    }
}
