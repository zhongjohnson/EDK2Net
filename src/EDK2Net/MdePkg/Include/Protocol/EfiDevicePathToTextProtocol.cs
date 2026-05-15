// SPDX-License-Identifier: MIT
//
// EFI_DEVICE_PATH_TO_TEXT_PROTOCOL — converts a device path (or a single
// device-path node) into a NUL-terminated CHAR16 string allocated from
// the EFI pool. Caller must FreePool() the returned buffer.
//
// Reference: MdePkg/Include/Protocol/DevicePathToText.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiDevicePathToTextProtocol
{
    /// <summary>
    /// Convert a single device-path node to text.
    /// Signature: CHAR16* (CONST EFI_DEVICE_PATH_PROTOCOL* node, BOOLEAN displayOnly, BOOLEAN allowShortcuts).
    /// </summary>
    public delegate* unmanaged<EfiDevicePathProtocol*, byte, byte, Char16*> ConvertDeviceNodeToText;

    /// <summary>
    /// Convert a full device path to text.
    /// Signature: CHAR16* (CONST EFI_DEVICE_PATH_PROTOCOL* path, BOOLEAN displayOnly, BOOLEAN allowShortcuts).
    /// </summary>
    public delegate* unmanaged<EfiDevicePathProtocol*, byte, byte, Char16*> ConvertDevicePathToText;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiDevicePathFromTextProtocol
{
    /// <summary>CHAR16* -> EFI_DEVICE_PATH_PROTOCOL* (single node).</summary>
    public delegate* unmanaged<Char16*, EfiDevicePathProtocol*> ConvertTextToDeviceNode;

    /// <summary>CHAR16* -> EFI_DEVICE_PATH_PROTOCOL* (full path).</summary>
    public delegate* unmanaged<Char16*, EfiDevicePathProtocol*> ConvertTextToDevicePath;
}
