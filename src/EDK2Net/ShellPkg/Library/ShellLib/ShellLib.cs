// SPDX-License-Identifier: MIT
//
// ShellLib — minimal ergonomic layer over EFI_SHELL_PROTOCOL and
// EFI_SHELL_PARAMETERS_PROTOCOL. Counterpart to MdePkg's UefiLib but for the
// ShellPkg surface (env, mappings, current dir, file open, command exec).
//
// All helpers are AOT-safe: no managed allocations, no exceptions; pointers
// returned by the firmware are surfaced as-is so callers can decide whether
// to copy/marshal.
namespace EDK2Net.ShellPkg.Library;

using EDK2Net.MdePkg.Uefi;
using EDK2Net.MdePkg.Library.UefiLib;
using EDK2Net.ShellPkg.Guid;
using EDK2Net.ShellPkg.Protocol;

public static unsafe class ShellLib
{
    private static EfiShellProtocol*           s_shell;
    private static EfiShellParametersProtocol* s_params;
    private static bool s_initialized;

    /// <summary>True if the EFI_SHELL_PROTOCOL was located.</summary>
    public static bool IsAvailable
    {
        get { EnsureInit(); return s_shell != null; }
    }

    /// <summary>EFI_SHELL_PROTOCOL pointer or null when unavailable.</summary>
    public static EfiShellProtocol* Shell
    {
        get { EnsureInit(); return s_shell; }
    }

    /// <summary>EFI_SHELL_PARAMETERS_PROTOCOL pointer for this image, or null.</summary>
    public static EfiShellParametersProtocol* Parameters
    {
        get { EnsureInit(); return s_params; }
    }

    /// <summary>Number of arguments (argc), or 0 when not Shell-launched.</summary>
    public static nuint Argc => Parameters != null ? s_params->Argc : 0;

    /// <summary>Pointer to the argv[] array, or null when not Shell-launched.</summary>
    public static Char16** Argv => Parameters != null ? s_params->Argv : null;

    /// <summary>Argv[i] as raw CHAR16* (no bounds check beyond null guard).</summary>
    public static Char16* GetArg(nuint index)
        => (Parameters != null && index < s_params->Argc) ? s_params->Argv[index] : null;

    private static void EnsureInit()
    {
        if (s_initialized) return;
        s_initialized = true;

        EfiShellProtocol* sh;
        var guid = ShellGuids.Shell;
        if (UefiLib.BootServices->LocateProtocol(&guid, null, (void**)&sh).IsSuccess)
            s_shell = sh;

        s_params = UefiLib.GetShellParameters();
    }

    // -----------------------------------------------------------------------
    // Environment
    // -----------------------------------------------------------------------

    /// <summary>Get a Shell environment variable, or null if missing.</summary>
    public static Char16* GetEnv(Char16* name)
        => IsAvailable ? s_shell->GetEnv(name) : null;

    /// <summary>Set / clear a Shell environment variable.</summary>
    public static EfiStatus SetEnv(Char16* name, Char16* value, bool volatileVar = true)
        => IsAvailable
            ? s_shell->SetEnv(name, value, volatileVar ? (byte)1 : (byte)0)
            : EfiStatus.Unsupported;

    // -----------------------------------------------------------------------
    // Current working directory
    // -----------------------------------------------------------------------

    /// <summary>Get current directory for a given file-system mapping
    /// (e.g. <c>L"FS0:"</c>) or pass null for the active mapping.</summary>
    public static Char16* GetCurDir(Char16* fileSystemMapping = null)
        => IsAvailable ? s_shell->GetCurDir(fileSystemMapping) : null;

    /// <summary>Change current directory.</summary>
    public static EfiStatus SetCurDir(Char16* fileSystemMapping, Char16* dir)
        => IsAvailable ? s_shell->SetCurDir(fileSystemMapping, dir) : EfiStatus.Unsupported;

    // -----------------------------------------------------------------------
    // Mappings <-> device paths <-> file paths
    // -----------------------------------------------------------------------

    public static EfiDevicePathProtocol* GetDevicePathFromMap(Char16* mapping)
        => IsAvailable ? s_shell->GetDevicePathFromMap(mapping) : null;

    public static Char16* GetMapFromDevicePath(EfiDevicePathProtocol* dp)
        => IsAvailable ? s_shell->GetMapFromDevicePath(dp) : null;

    public static EfiDevicePathProtocol* GetDevicePathFromFilePath(Char16* path)
        => IsAvailable ? s_shell->GetDevicePathFromFilePath(path) : null;

    public static Char16* GetFilePathFromDevicePath(EfiDevicePathProtocol* dp)
        => IsAvailable ? s_shell->GetFilePathFromDevicePath(dp) : null;

    // -----------------------------------------------------------------------
    // File operations
    // -----------------------------------------------------------------------

    /// <summary>Open a file by Shell-style path (e.g. <c>L"FS0:\\dir\\file"</c>).</summary>
    public static EfiStatus OpenFileByName(Char16* path, ulong openMode, out EfiFileProtocol* handle)
    {
        handle = null;
        if (!IsAvailable) return EfiStatus.Unsupported;
        EfiFileProtocol* h;
        var status = s_shell->OpenFileByName(path, openMode, &h);
        if (status.IsSuccess) handle = h;
        return status;
    }

    public static EfiStatus CloseFile(EfiFileProtocol* handle)
        => IsAvailable ? s_shell->CloseFile(handle) : EfiStatus.Unsupported;

    public static EfiStatus DeleteFileByName(Char16* path)
        => IsAvailable ? s_shell->DeleteFileByName(path) : EfiStatus.Unsupported;

    /// <summary>Glob-expand a pattern such as <c>L"FS0:\\*.efi"</c>. Caller must
    /// release the returned list with <see cref="FreeFileList"/>.</summary>
    public static EfiStatus FindFiles(Char16* pattern, out EfiShellFileInfo* list)
    {
        list = null;
        if (!IsAvailable) return EfiStatus.Unsupported;
        EfiShellFileInfo* l;
        var status = s_shell->FindFiles(pattern, &l);
        if (status.IsSuccess) list = l;
        return status;
    }

    public static EfiStatus FreeFileList(EfiShellFileInfo* list)
        => IsAvailable ? s_shell->FreeFileList(list) : EfiStatus.Unsupported;

    // -----------------------------------------------------------------------
    // Command execution
    // -----------------------------------------------------------------------

    /// <summary>Execute a Shell command line. Returns the exec status; the
    /// command's own exit status (if available) is written to
    /// <paramref name="commandStatus"/>.</summary>
    public static EfiStatus Execute(Char16* commandLine, out EfiStatus commandStatus, Char16** environment = null)
    {
        commandStatus = EfiStatus.Success;
        if (!IsAvailable) return EfiStatus.Unsupported;
        EfiStatus cs;
        var parent = UefiLib.ImageHandle;
        var status = s_shell->Execute(&parent, commandLine, environment, &cs);
        commandStatus = cs;
        return status;
    }
}
