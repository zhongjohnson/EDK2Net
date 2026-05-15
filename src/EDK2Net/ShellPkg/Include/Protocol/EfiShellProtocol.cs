// SPDX-License-Identifier: MIT
//
// EFI_SHELL_PROTOCOL — UEFI Shell 2.0+ programmatic interface. Provides the
// Shell's view of the environment: file system mappings, current directory,
// environment variables, command execution, file open by path, etc.
//
// We bind the most commonly used members; the remainder are kept as opaque
// function pointers (`void*`) to preserve correct vtable offsets without
// committing to every type up front.
//
// Reference: ShellPkg/Include/Protocol/EfiShell.h (UEFI Shell Spec 2.2).
namespace EDK2Net.ShellPkg.Protocol;

using System.Runtime.InteropServices;

/// <summary>EFI_SHELL_DEVICE_NAME_FLAGS bits.</summary>
public static class EfiShellDeviceNameFlags
{
    public const uint UseComponentName = 0x00000001;
    public const uint UseDevicePath    = 0x00000002;
}

/// <summary>EFI_SHELL_FILE_INFO — a node in the linked list returned by
/// <c>OpenFileList</c> / <c>FindFiles</c>. Caller frees with
/// <c>FreeFileList</c>.</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiShellFileInfo
{
    public Link        Link;        // LIST_ENTRY (Fwd, Bwd)
    public EfiStatus   Status;
    public Char16*     FullName;
    public Char16*     FileName;
    public EfiFileProtocol* Handle;
    public EfiFileInfo* Info;
}

/// <summary>EDK2 LIST_ENTRY — embedded doubly-linked-list node.</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct Link
{
    public Link* Fwd;
    public Link* Bwd;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiShellProtocol
{
    // Environment
    public delegate* unmanaged<Char16*, Char16*>                                   GetEnv;
    public delegate* unmanaged<Char16*, Char16*, byte, EfiStatus>                  SetEnv;
    public delegate* unmanaged<Char16*, byte, EfiStatus>                           GetAlias;
    public delegate* unmanaged<Char16*, Char16*, byte, byte, EfiStatus>            SetAlias;

    // Help / version
    public delegate* unmanaged<Char16*, Char16*, Char16***, EfiStatus>             GetHelpText;

    // Device path / mapping
    public delegate* unmanaged<Char16*, EfiDevicePathProtocol*>                    GetDevicePathFromMap;
    public delegate* unmanaged<EfiDevicePathProtocol*, Char16*>                    GetMapFromDevicePath;
    public delegate* unmanaged<Char16*, EfiDevicePathProtocol*>                    GetDevicePathFromFilePath;
    public delegate* unmanaged<EfiDevicePathProtocol*, Char16*>                    GetFilePathFromDevicePath;
    public delegate* unmanaged<EfiDevicePathProtocol*, Char16*, EfiStatus>         SetMap;

    // Current working directory
    public delegate* unmanaged<Char16*, Char16*>                                   GetCurDir;
    public delegate* unmanaged<Char16*, Char16*, EfiStatus>                        SetCurDir;

    // File operations
    public delegate* unmanaged<Char16*, EfiShellFileInfo**, EfiStatus>             OpenFileList;
    public delegate* unmanaged<EfiShellFileInfo*, EfiStatus>                       FreeFileList;
    public delegate* unmanaged<Char16*, EfiShellFileInfo**, EfiStatus>             RemoveDupInFileList;

    public delegate* unmanaged<Char16*, byte>                                      BatchIsActive;
    public delegate* unmanaged<Char16*, byte>                                      IsRootShell;

    public delegate* unmanaged<Char16*, byte*, ulong, EfiStatus>                   EnablePageBreak; // misc
    public delegate* unmanaged<Char16*, EfiStatus>                                 DisablePageBreak;
    public delegate* unmanaged<byte>                                               GetPageBreak;
    public delegate* unmanaged<EfiHandle, byte*, EfiStatus>                        GetDeviceName;

    public delegate* unmanaged<EfiFileProtocol*, ulong, EfiStatus>                 GetFileSize;        // (handle, *size)
    public delegate* unmanaged<EfiFileProtocol*, EfiStatus>                        GetFilePosition;
    public delegate* unmanaged<EfiFileProtocol*, ulong, EfiStatus>                 SetFilePosition;
    public delegate* unmanaged<EfiFileProtocol*, nuint*, void*, EfiStatus>         ReadFile;
    public delegate* unmanaged<EfiFileProtocol*, nuint*, void*, EfiStatus>         WriteFile;
    public delegate* unmanaged<EfiFileProtocol*, EfiStatus>                        CloseFile;
    public delegate* unmanaged<EfiFileProtocol*, EfiStatus>                        DeleteFile;
    public delegate* unmanaged<Char16*, EfiStatus>                                 DeleteFileByName;

    public delegate* unmanaged<Char16*, ulong, EfiFileProtocol**, EfiStatus>       OpenFileByName;     // (path, mode, *handle)
    public delegate* unmanaged<Char16*, EfiShellFileInfo**, EfiStatus>             FindFiles;
    public delegate* unmanaged<EfiFileProtocol*, EfiShellFileInfo**, EfiStatus>    FindFilesInDir;

    public delegate* unmanaged<Char16*, EfiFileProtocol**, ulong, EfiStatus>       OpenRoot;           // (path, *handle, attribs)
    public delegate* unmanaged<EfiHandle, EfiFileProtocol**, EfiStatus>            OpenRootByHandle;

    public EfiEvent ExecutionBreak;

    public uint MajorVersion;
    public uint MinorVersion;

    public delegate* unmanaged<EfiHandle*, Char16*, Char16**, EfiStatus*, EfiStatus> Execute;          // (parentImage, command, env, *status)
    public delegate* unmanaged<Char16*, Char16**, byte>                            GetEnvEx;

    public delegate* unmanaged<EfiHandle, EfiStatus>                               RegisterGuidName;   // simplified
    public delegate* unmanaged<EfiGuid*, Char16**, EfiStatus>                      GetGuidName;
    public delegate* unmanaged<Char16*, EfiGuid*, EfiStatus>                       GetGuidFromName;
}
