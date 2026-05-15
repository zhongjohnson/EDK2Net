// SPDX-License-Identifier: MIT
// EFI_SYSTEM_TABLE — root table passed to every UEFI image entry point.
// Layout mirrors EDK2 MdePkg/Include/Uefi/UefiSpec.h.
namespace EDK2Net.MdePkg.Uefi;

using EDK2Net.MdePkg.Protocol;
using EDK2Net.ShellPkg.Protocol;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSystemTable
{
    public EfiTableHeader            Hdr;
    public Char16*                   FirmwareVendor;
    public uint                      FirmwareRevision;
    public EfiHandle                 ConsoleInHandle;
    public EfiSimpleTextInputProtocol*  ConIn;
    public EfiHandle                 ConsoleOutHandle;
    public EfiSimpleTextOutputProtocol* ConOut;
    public EfiHandle                 StandardErrorHandle;
    public EfiSimpleTextOutputProtocol* StdErr;
    public EfiRuntimeServices*       RuntimeServices;
    public EfiBootServices*          BootServices;
    public nuint                     NumberOfTableEntries;
    public EfiConfigurationTable*    ConfigurationTable;
}
