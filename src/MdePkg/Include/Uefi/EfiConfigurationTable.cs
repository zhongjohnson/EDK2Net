// SPDX-License-Identifier: MIT
// EFI_CONFIGURATION_TABLE — entries hung off EfiSystemTable.ConfigurationTable.
// Reference: MdePkg/Include/Uefi/UefiSpec.h.
namespace EDK2Net.MdePkg.Uefi;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiConfigurationTable
{
    public EfiGuid VendorGuid;
    public void*   VendorTable;
}
