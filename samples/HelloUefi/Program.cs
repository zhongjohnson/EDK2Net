// SPDX-License-Identifier: MIT
//
// HelloUefi — minimal C# UEFI application.
//
// Build:
//   dotnet publish -c Release -r win-x64
// Run under QEMU + OVMF:
//   ../../build/run-qemu.ps1 -Image bin/Release/net9.0/win-x64/publish/HelloUefi.efi

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EDK2Net.Efi;
using EDK2Net.Efi.Protocols;

namespace HelloUefi;

public static unsafe class Program
{
    // The C# compiler requires an entry-point method when OutputType=Exe.
    // The linker switch /ENTRY:EfiMain overrides which symbol the PE header
    // points at, so this Main is never actually invoked — it just exists to
    // satisfy the language rule.
    public static int Main() => 0;

    // System table cached so helpers can reuse it.
    private static EfiSystemTable* s_systemTable;
    private static EfiHandle       s_imageHandle;

    /// <summary>
    /// UEFI entry point. The name must match the linker /ENTRY:EfiMain switch.
    /// Signature is the standard EFI_IMAGE_ENTRY_POINT.
    /// </summary>
    [UnmanagedCallersOnly(EntryPoint = "EfiMain")]
    public static nuint EfiMain(EfiHandle imageHandle, EfiSystemTable* systemTable)
    {
        s_imageHandle = imageHandle;
        s_systemTable = systemTable;

        // Reference the runtime stubs so the linker keeps them.
        EDK2Net.Runtime.RuntimeStubs.KeepAlive();

        Print("Hello, UEFI from C#!\r\n");
        Print("EDK2Net is up and running.\r\n");

        // Wait for a keystroke so the message stays visible.
        WaitForKey();

        return EfiStatus.Success;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void Print(string text)
    {
        // String literals in .NET are UTF-16 already, which is what UEFI
        // expects (CHAR16 / null-terminated). We just need a pinned pointer.
        fixed (char* p = text)
        {
            s_systemTable->ConOut->OutputString(s_systemTable->ConOut, (Char16*)p);
        }
    }

    private static void WaitForKey()
    {
        var conIn = s_systemTable->ConIn;
        var bs    = s_systemTable->BootServices;

        // Block on the WaitForKey event provided by ConIn.
        EfiEvent ev = conIn->WaitForKey;
        nuint index;
        bs->WaitForEvent(1, &ev, &index);

        EfiInputKey key;
        conIn->ReadKeyStroke(conIn, &key);
    }
}
