// SPDX-License-Identifier: MIT
//
// HelloUefi — minimal C# UEFI application using the EDK2Net.UefiLib helpers.
//
// Build:
//   dotnet publish -c Release -r win-x64
// Run under QEMU + OVMF:
//   ../../build/run-qemu.ps1 -Image bin/Release/net9.0/win-x64/publish/HelloUefi.efi

using System.Runtime.InteropServices;
using EDK2Net.MdePkg.Library.UefiLib;
using EDK2Net.MdePkg.Uefi;
using EDK2Net.MdePkg.Guid;
using EDK2Net.ShellPkg.Guid;
using EDK2Net.MdePkg.Protocol;
using EDK2Net.ShellPkg.Protocol;
namespace HelloUefi;

public static unsafe class Program
{
    // The C# compiler requires an entry-point method when OutputType=Exe.
    // The linker's /ENTRY:EfiMain overrides which symbol the PE header points
    // at, so this Main is never actually invoked.
    public static int Main() => 0;

    /// <summary>
    /// UEFI entry point. The name must match the linker /ENTRY:EfiMain switch.
    /// Signature is the standard EFI_IMAGE_ENTRY_POINT.
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "EfiMain")]
    public static nuint EfiMain(EfiHandle imageHandle, EfiSystemTable* systemTable)
    {
        UefiLib.Initialize(imageHandle, systemTable);

        // Reference the runtime stubs so the linker keeps them.
        EDK2Net.Runtime.RuntimeStubs.KeepAlive();

        UefiLib.PrintLine("Hello, UEFI from C#!");
        UefiLib.PrintLine("EDK2Net is up and running.");

        // Demo: locate the LoadedImage protocol on this image's handle.
        EfiLoadedImageProtocol* loadedImage;
        var s = UefiLib.HandleProtocol(
            imageHandle,
            EfiGuids.LoadedImageProtocol,
            (void**)&loadedImage);

        if (s.IsSuccess)
        {
            UefiLib.PrintLine("Got EFI_LOADED_IMAGE_PROTOCOL.");
        }

        UefiLib.PrintLine("Press any key to exit...");
        UefiLib.WaitForKey();

        return EfiStatus.Success;
    }
}
