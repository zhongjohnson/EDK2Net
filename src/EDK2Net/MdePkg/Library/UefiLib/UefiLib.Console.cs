// SPDX-License-Identifier: MIT
//
// UefiLib — Console (ConOut/ConIn) convenience.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>Set a foreground/background combined attribute on ConOut.</summary>
    public static EfiStatus SetTextAttribute(int attribute)
        => ConOut->SetAttribute(ConOut, (nuint)attribute);

    /// <summary>Reset attribute to LightGray-on-Black (the common UEFI default).</summary>
    public static EfiStatus ResetTextAttribute()
        => ConOut->SetAttribute(ConOut, 0x07);

    /// <summary>Clear the screen (uses current attribute as the fill).</summary>
    public static EfiStatus ClearScreen()
        => ConOut->ClearScreen(ConOut);

    /// <summary>Move the cursor to (column, row) in character cells.</summary>
    public static EfiStatus SetCursorPosition(nuint column, nuint row)
        => ConOut->SetCursorPosition(ConOut, column, row);

    /// <summary>Show or hide the cursor.</summary>
    public static EfiStatus EnableCursor(bool visible)
        => ConOut->EnableCursor(ConOut, (byte)(visible ? 1 : 0));

    /// <summary>Number of (columns, rows) for the current text mode.</summary>
    public static EfiStatus QueryCurrentMode(out nuint columns, out nuint rows)
    {
        nuint c = 0, r = 0;
        var s = ConOut->QueryMode(ConOut, (nuint)ConOut->Mode->Mode, &c, &r);
        columns = c; rows = r;
        return s;
    }

    /// <summary>
    /// Block until a key is pressed and return it. Returns the firmware status
    /// from <c>ReadKeyStroke</c>.
    /// </summary>
    public static EfiStatus ReadKey(out EfiInputKey key)
    {
        var conIn = ConIn;
        var bs    = BootServices;

        EfiEvent ev = conIn->WaitForKey;
        nuint index;
        var s = bs->WaitForEvent(1, &ev, &index);
        if (s.IsError) { key = default; return s; }

        EfiInputKey k;
        s = conIn->ReadKeyStroke(conIn, &k);
        key = k;
        return s;
    }

    /// <summary>
    /// Non-blocking poll. Returns <see cref="EfiStatus.NotReady"/> when no key
    /// is buffered.
    /// </summary>
    public static EfiStatus PollKey(out EfiInputKey key)
    {
        EfiInputKey k;
        var s = ConIn->ReadKeyStroke(ConIn, &k);
        key = k;
        return s;
    }
}
