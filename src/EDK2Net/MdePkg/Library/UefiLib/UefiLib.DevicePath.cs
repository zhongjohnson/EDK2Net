// SPDX-License-Identifier: MIT
//
// UefiLib — Device-path helpers built on EFI_DEVICE_PATH_TO_TEXT_PROTOCOL.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Guid;
using EDK2Net.MdePkg.Protocol;
using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>
    /// Convert a device path to its textual form using
    /// EFI_DEVICE_PATH_TO_TEXT_PROTOCOL. The returned CHAR16* is allocated
    /// from the EFI pool — caller must <see cref="FreePool"/> it.
    /// </summary>
    public static EfiStatus ConvertDevicePathToText(
        EfiDevicePathProtocol* path,
        bool displayOnly,
        bool allowShortcuts,
        out Char16* text)
    {
        text = null;

        EfiDevicePathToTextProtocol* p2t;
        var s = LocateProtocol(EfiGuids.DevicePathToTextProtocol, (void**)&p2t);
        if (s.IsError) return s;

        text = p2t->ConvertDevicePathToText(
            path,
            (byte)(displayOnly    ? 1 : 0),
            (byte)(allowShortcuts ? 1 : 0));

        return text != null ? EfiStatus.Success : EfiStatus.OutOfResources;
    }

    /// <summary>
    /// Print a device path to ConOut and free the temporary buffer.
    /// Returns the underlying status from the conversion or write.
    /// </summary>
    public static EfiStatus PrintDevicePath(EfiDevicePathProtocol* path,
                                            bool displayOnly = true,
                                            bool allowShortcuts = true)
    {
        Char16* text;
        var s = ConvertDevicePathToText(path, displayOnly, allowShortcuts, out text);
        if (s.IsError) return s;
        var rs = ConOut->OutputString(ConOut, text);
        BootServices->FreePool(text);
        return rs;
    }
}
