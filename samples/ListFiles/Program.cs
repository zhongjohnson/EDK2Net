// SPDX-License-Identifier: MIT
//
// ListFiles — enumerate the root directory of the volume this image was
// loaded from. Demonstrates the typical UEFI sequence:
//
//   ImageHandle
//     -> EFI_LOADED_IMAGE_PROTOCOL          (HandleProtocol on this image)
//        .DeviceHandle
//          -> EFI_SIMPLE_FILE_SYSTEM_PROTOCOL (HandleProtocol on the device)
//             .OpenVolume()
//               -> EFI_FILE_PROTOCOL (root)
//                  .Read() loop -> EFI_FILE_INFO records
//
// Build:
//   dotnet publish -c Release -r win-x64
// Run:
//   ../../build/run-qemu.ps1 -Image .\samples\ListFiles\bin\Release\net9.0\win-x64\publish\ListFiles.efi

using System.Runtime.InteropServices;
using EDK2Net.MdePkg.Library.UefiLib;
using EDK2Net.MdePkg.Uefi;
using EDK2Net.MdePkg.Guid;
using EDK2Net.ShellPkg.Guid;
using EDK2Net.MdePkg.Protocol;
using EDK2Net.ShellPkg.Protocol;
namespace ListFiles;

public static unsafe class Program
{
    public static int Main() => 0;

    [UnmanagedCallersOnly(EntryPoint = "EfiMain")]
    public static nuint EfiMain(EfiHandle imageHandle, EfiSystemTable* systemTable)
    {
        UefiLib.Initialize(imageHandle, systemTable);
        EDK2Net.Runtime.RuntimeStubs.KeepAlive();

        UefiLib.PrintLine("ListFiles — EDK2Net sample");
        UefiLib.PrintLine("==========================");

        var status = Run();
        if (status.IsError)
        {
            UefiLib.PrintLine("error: see status code above");
        }

        UefiLib.PrintLine("");
        UefiLib.PrintLine("Press any key to exit...");
        UefiLib.WaitForKey();
        return EfiStatus.Success;
    }

    private static EfiStatus Run()
    {
        // 1) Get the LoadedImage protocol on our own image handle.
        EfiLoadedImageProtocol* loadedImage;
        var s = UefiLib.HandleProtocol(
            UefiLib.ImageHandle,
            EfiGuids.LoadedImageProtocol,
            (void**)&loadedImage);
        if (s.IsError) { UefiLib.PrintLine("HandleProtocol(LoadedImage) failed"); return s; }

        // 2) Get SimpleFileSystem on the device that loaded us.
        EfiSimpleFileSystemProtocol* sfs;
        s = UefiLib.HandleProtocol(
            loadedImage->DeviceHandle,
            EfiGuids.SimpleFileSystemProtocol,
            (void**)&sfs);
        if (s.IsError) { UefiLib.PrintLine("HandleProtocol(SimpleFileSystem) failed"); return s; }

        // 3) Open the root directory.
        EfiFileProtocol* root;
        s = sfs->OpenVolume(sfs, &root);
        if (s.IsError) { UefiLib.PrintLine("OpenVolume failed"); return s; }

        UefiLib.PrintLine("Root directory:");

        // 4) Loop reading EFI_FILE_INFO records.
        // EFI_FILE_INFO is variable-length (FileName tail). 1 KiB is enough
        // for typical names; firmware returns BUFFER_TOO_SMALL and the
        // required size if not. We keep this simple by using a fixed buffer.
        const nuint BufferSize = 1024;
        byte* buffer = stackalloc byte[(int)BufferSize];

        while (true)
        {
            nuint size = BufferSize;
            s = root->Read(root, &size, buffer);
            if (s.IsError)
            {
                UefiLib.PrintLine("Read failed");
                root->Close(root);
                return s;
            }
            if (size == 0) break; // end-of-directory

            var info = (EfiFileInfo*)buffer;
            // FileName begins immediately after the EfiFileInfo header.
            var name = (Char16*)((byte*)info + sizeof(EfiFileInfo));

            // [DIR] vs [   ] indicator
            bool isDir = (info->Attribute & EfiFileAttribute.Directory) != 0;
            UefiLib.Print(isDir ? "  [DIR] " : "        ");

            // Print the name (it's already a null-terminated CHAR16 string).
            UefiLib.ConOut->OutputString(UefiLib.ConOut, name);

            UefiLib.PrintLine("");
        }

        root->Close(root);
        return EfiStatus.Success;
    }
}
