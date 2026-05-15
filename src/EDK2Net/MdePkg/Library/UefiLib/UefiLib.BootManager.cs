// SPDX-License-Identifier: MIT
//
// UefiLib — Boot Manager convenience.
//
// Reads the well-known global variables defined by the UEFI Boot Manager:
//   - BootCurrent   (UINT16)             — the option index that just booted
//   - BootNext      (UINT16)             — option to try once on next boot
//   - BootOrder     (UINT16[])           — boot priority list
//   - Boot####      (EFI_LOAD_OPTION)    — individual boot entries
//
// Helpers also let you parse a Boot#### blob into its description string,
// device-path pointer, and optional-data tail.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Guid;
using EDK2Net.MdePkg.Library.BaseLib;
using EDK2Net.MdePkg.Protocol;
using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>Read <c>BootCurrent</c>.</summary>
    public static EfiStatus GetBootCurrent(out ushort optionIndex)
    {
        ushort v = 0;
        nuint sz = sizeof(ushort);
        var s = GetGlobalVariable("BootCurrent", null, &sz, &v);
        optionIndex = v;
        return s;
    }

    /// <summary>Read <c>BootNext</c>. Returns <see cref="EfiStatus.NotFound"/> when unset.</summary>
    public static EfiStatus GetBootNext(out ushort optionIndex)
    {
        ushort v = 0;
        nuint sz = sizeof(ushort);
        var s = GetGlobalVariable("BootNext", null, &sz, &v);
        optionIndex = v;
        return s;
    }

    /// <summary>Set <c>BootNext</c>. Causes the firmware to try the named option once.</summary>
    public static EfiStatus SetBootNext(ushort optionIndex)
    {
        ushort v = optionIndex;
        return SetVariable("BootNext", EfiGuids.GlobalVariable,
                           EfiVariableNvBsRt, sizeof(ushort), &v);
    }

    /// <summary>Delete <c>BootNext</c>.</summary>
    public static EfiStatus ClearBootNext()
        => DeleteVariable("BootNext", EfiGuids.GlobalVariable);

    /// <summary>
    /// Read <c>BootOrder</c> into a pool-allocated UINT16 array. Free with
    /// <see cref="FreePool"/>. <paramref name="count"/> receives the number
    /// of entries.
    /// </summary>
    public static EfiStatus GetBootOrder(out ushort* order, out nuint count)
    {
        order = null; count = 0;

        nuint size = 0;
        var s = GetGlobalVariable("BootOrder", null, &size, null);
        if (s == EfiStatus.NotFound) return s;
        if (s != EfiStatus.BufferTooSmall && s.IsError) return s;

        void* buf = null;
        s = AllocatePool(size, &buf);
        if (s.IsError) return s;

        s = GetGlobalVariable("BootOrder", null, &size, buf);
        if (s.IsError) { FreePool(buf); return s; }

        order = (ushort*)buf;
        count = size / sizeof(ushort);
        return EfiStatus.Success;
    }

    /// <summary>
    /// Read a Boot#### variable for the given <paramref name="optionIndex"/>.
    /// Buffer is allocated from the pool — caller frees with <see cref="FreePool"/>.
    /// </summary>
    public static EfiStatus GetBootOption(ushort optionIndex, out void* buffer, out nuint size)
    {
        buffer = null; size = 0;

        // Format the variable name: "Boot" + 4 hex digits, uppercase.
        Char16* name = stackalloc Char16[16];
        name[0] = (Char16)'B';
        name[1] = (Char16)'o';
        name[2] = (Char16)'o';
        name[3] = (Char16)'t';
        Char16String.UInt64ToHex(optionIndex, name + 4, 12, uppercase: true, minWidth: 4);
        // ensure NUL after exactly 4 hex digits
        name[8] = (Char16)0;

        // Probe size.
        var vendor = EfiGuids.GlobalVariable;
        nuint sz = 0;
        var s = RuntimeServices->GetVariable(name, &vendor, null, &sz, null);
        if (s == EfiStatus.NotFound) return s;
        if (s != EfiStatus.BufferTooSmall && s.IsError) return s;

        void* buf = null;
        s = AllocatePool(sz, &buf);
        if (s.IsError) return s;

        s = RuntimeServices->GetVariable(name, &vendor, null, &sz, buf);
        if (s.IsError) { FreePool(buf); return s; }

        buffer = buf;
        size   = sz;
        return EfiStatus.Success;
    }

    /// <summary>
    /// Parse a Boot#### blob. <paramref name="description"/> points into the
    /// blob (no copy); <paramref name="filePath"/> points to the embedded
    /// device-path; <paramref name="optionalData"/> / <paramref name="optionalDataSize"/>
    /// describe the trailing app-defined region (may be empty).
    /// </summary>
    public static bool ParseBootOption(
        void* blob, nuint blobSize,
        out uint attributes,
        out Char16* description,
        out EfiDevicePathProtocol* filePath,
        out void* optionalData,
        out nuint optionalDataSize)
    {
        attributes = 0;
        description = null;
        filePath = null;
        optionalData = null;
        optionalDataSize = 0;

        if (blob == null || blobSize < (nuint)sizeof(EfiLoadOption)) return false;

        var hdr = (EfiLoadOption*)blob;
        attributes = hdr->Attributes;

        var p = (byte*)blob + sizeof(EfiLoadOption);
        var end = (byte*)blob + blobSize;

        description = (Char16*)p;
        // Walk to the NUL.
        while (p + 1 < end && (((Char16*)p)->Value != 0))
            p += sizeof(char);
        if (p + 1 >= end) return false;
        p += sizeof(char); // skip NUL

        if (p + hdr->FilePathListLength > end) return false;
        filePath = (EfiDevicePathProtocol*)p;
        p += hdr->FilePathListLength;

        if (p < end)
        {
            optionalData = p;
            optionalDataSize = (nuint)(end - p);
        }
        return true;
    }
}
