// SPDX-License-Identifier: MIT
//
// Well-known UEFI Shell protocol GUIDs.
// Reference: ShellPkg/Include/Protocol/EfiShell*.h.
//
// Kept in a separate class from EfiGuids because Shell protocols belong to
// ShellPkg, not MdePkg. Keeping the boundary visible matches EDK2 source.
namespace EDK2Net.ShellPkg.Guid;

public static class ShellGuids
{
    /// <summary>EFI_SHELL_PROTOCOL_GUID (UEFI Shell 2.0+).</summary>
    public static readonly EfiGuid Shell =
        new(0x6302d008, 0x7f9b, 0x4f30, 0x87, 0xac, 0x60, 0xc9, 0xfe, 0xf5, 0xda, 0x4e);

    /// <summary>EFI_SHELL_PARAMETERS_PROTOCOL_GUID — installed on the image
    /// handle when launched from the UEFI Shell so apps can read argv.</summary>
    public static readonly EfiGuid ShellParameters =
        new(0x752f3136, 0x4e16, 0x4fdc, 0xa2, 0x2a, 0xe5, 0xf4, 0x68, 0x12, 0xf4, 0xca);

    /// <summary>EFI_SHELL_DYNAMIC_COMMAND_PROTOCOL_GUID.</summary>
    public static readonly EfiGuid ShellDynamicCommand =
        new(0x3c7200e9, 0x005f, 0x4ea4, 0x87, 0xde, 0xa3, 0xdf, 0xac, 0x8a, 0x27, 0xc3);
}
