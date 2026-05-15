// SPDX-License-Identifier: MIT
//
// ShellArgs — print the argv this app was launched with from the UEFI Shell,
// then show the current directory, an environment variable, and a SHA-256
// hash of a small literal string. Exercises ShellLib + UefiLib.Hash.
//
// Run from a UEFI Shell prompt, e.g.:
//   FS0:\> ShellArgs.efi hello world

using System.Runtime.InteropServices;
using EDK2Net.MdePkg.Library.UefiLib;
using EDK2Net.MdePkg.Uefi;
using EDK2Net.ShellPkg.Library;

namespace ShellArgs;

public static unsafe class Program
{
    public static int Main() => 0;

    [UnmanagedCallersOnly(EntryPoint = "EfiMain")]
    public static nuint EfiMain(EfiHandle imageHandle, EfiSystemTable* systemTable)
    {
        UefiLib.Initialize(imageHandle, systemTable);
        EDK2Net.Runtime.RuntimeStubs.KeepAlive();

        UefiLib.PrintLine("ShellArgs — EDK2Net sample");
        UefiLib.PrintLine("==========================");

        if (ShellLib.Parameters == null)
        {
            UefiLib.PrintLine("Not launched from the UEFI Shell (no EFI_SHELL_PARAMETERS_PROTOCOL).");
        }
        else
        {
            UefiLib.Print("argc = ");
            UefiLib.PrintDec((ulong)ShellLib.Argc);
            UefiLib.PrintLine("");

            for (nuint i = 0; i < ShellLib.Argc; i++)
            {
                UefiLib.Print("  argv[");
                UefiLib.PrintDec(i);
                UefiLib.Print("] = ");
                var a = ShellLib.GetArg(i);
                if (a != null) UefiLib.Print(a);
                UefiLib.PrintLine("");
            }
        }

        if (ShellLib.IsAvailable)
        {
            var cwd = ShellLib.GetCurDir(null);
            UefiLib.Print("cwd = ");
            if (cwd != null) UefiLib.Print(cwd);
            UefiLib.PrintLine("");

            // Print the standard "path" environment variable if present.
            fixed (char* pName = "path")
            {
                var v = ShellLib.GetEnv((Char16*)pName);
                UefiLib.Print("path = ");
                if (v != null) UefiLib.Print(v);
                UefiLib.PrintLine("");
            }
        }

        // SHA-256 of a literal byte sequence to demonstrate the Hash2 helper.
        // Bytes for "EDK2Net".
        byte* message = stackalloc byte[7] { 0x45, 0x44, 0x4b, 0x32, 0x4e, 0x65, 0x74 };
        byte* digest  = stackalloc byte[32];
        var s = UefiLib.Sha256(message, 7, digest);
        UefiLib.Print("SHA-256(\"EDK2Net\") = ");
        if (s.IsError)
        {
            UefiLib.PrintLine("(EFI_HASH2 unavailable)");
        }
        else
        {
            for (int i = 0; i < 32; i++) UefiLib.PrintHex(digest[i]);
            UefiLib.PrintLine("");
        }

        UefiLib.PrintLine("");
        UefiLib.PrintLine("Press any key to exit...");
        UefiLib.WaitForKey();
        return EfiStatus.Success;
    }
}
