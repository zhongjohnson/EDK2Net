// SPDX-License-Identifier: MIT
//
// EFI_COMPONENT_NAME2_PROTOCOL — UEFI 2.0+ replacement for EFI_COMPONENT_NAME.
// Lets the platform fetch human-readable driver/controller names in a given
// RFC 4646 language code.
//
// Reference: MdePkg/Include/Protocol/ComponentName2.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiComponentName2Protocol
{
    /// <summary>(this, language CHAR8*, out CHAR16** driverName).</summary>
    public delegate* unmanaged<EfiComponentName2Protocol*, byte*, Char16**, EfiStatus> GetDriverName;

    /// <summary>(this, controllerHandle, childHandle (or NULL), language, out CHAR16** controllerName).</summary>
    public delegate* unmanaged<EfiComponentName2Protocol*, EfiHandle, EfiHandle, byte*, Char16**, EfiStatus> GetControllerName;

    /// <summary>NUL-terminated semicolon-separated list of supported RFC 4646 languages (CHAR8*).</summary>
    public byte* SupportedLanguages;
}
