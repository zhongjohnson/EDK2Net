// SPDX-License-Identifier: MIT
//
// EFI_SHELL_PARAMETERS_PROTOCOL — installed on an image's handle when the
// UEFI Shell launches it. Lets the application read its argv[] and grab the
// I/O handles the Shell wired up.
//
// Reference: ShellPkg/Include/Protocol/EfiShellParameters.h.
namespace EDK2Net.ShellPkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiShellParametersProtocol
{
    /// <summary>Pointer to an array of <see cref="Argc"/> CHAR16* strings.</summary>
    public Char16** Argv;

    /// <summary>Number of entries in <see cref="Argv"/>.</summary>
    public nuint Argc;

    /// <summary>EFI_FILE_PROTOCOL* for the Shell's stdin (may be null).</summary>
    public EfiFileProtocol* StdIn;
    /// <summary>EFI_FILE_PROTOCOL* for the Shell's stdout (may be null).</summary>
    public EfiFileProtocol* StdOut;
    /// <summary>EFI_FILE_PROTOCOL* for the Shell's stderr (may be null).</summary>
    public EfiFileProtocol* StdErr;
}
