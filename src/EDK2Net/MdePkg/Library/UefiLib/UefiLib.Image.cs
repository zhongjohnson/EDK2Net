// SPDX-License-Identifier: MIT
//
// UefiLib — Image services convenience.
//
// Wraps EFI_BOOT_SERVICES.LoadImage / StartImage / UnloadImage / Exit. Handy
// for chain-loading another .efi from disk or memory.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Protocol;
using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>Load an image from a device path. Returns the new image handle.</summary>
    public static EfiStatus LoadImageFromDevicePath(
        EfiDevicePathProtocol* devicePath,
        bool bootPolicy,
        out EfiHandle imageHandle)
    {
        EfiHandle h;
        var s = BootServices->LoadImage(
            (byte)(bootPolicy ? 1 : 0),
            s_imageHandle,
            devicePath,
            null, 0,
            &h);
        // (devicePath is EfiDevicePathProtocol*; LoadImage takes it as void*).
        imageHandle = h;
        return s;
    }

    /// <summary>Load an image already in memory.</summary>
    public static EfiStatus LoadImageFromBuffer(
        void* buffer, nuint size,
        out EfiHandle imageHandle)
    {
        EfiHandle h;
        var s = BootServices->LoadImage(
            0,
            s_imageHandle,
            null,
            buffer, size,
            &h);
        imageHandle = h;
        return s;
    }

    /// <summary>Start a previously loaded image. Returns the image's exit status.</summary>
    public static EfiStatus StartImage(EfiHandle imageHandle, out nuint exitDataSize, out Char16* exitData)
    {
        nuint sz = 0;
        Char16* data = null;
        var s = BootServices->StartImage(imageHandle, &sz, &data);
        exitDataSize = sz;
        exitData = data;
        return s;
    }

    /// <summary>Start an image and discard its exit data.</summary>
    public static EfiStatus StartImage(EfiHandle imageHandle)
        => StartImage(imageHandle, out _, out _);

    /// <summary>Unload an image that was loaded but not started, or that returned.</summary>
    public static EfiStatus UnloadImage(EfiHandle imageHandle)
        => BootServices->UnloadImage(imageHandle);

    /// <summary>
    /// Exit the current image. Does not return on success — terminates the
    /// running .efi and returns control to whoever started it.
    /// </summary>
    public static void Exit(EfiStatus exitStatus)
        => BootServices->Exit(exitStatus, 0, null);
}
