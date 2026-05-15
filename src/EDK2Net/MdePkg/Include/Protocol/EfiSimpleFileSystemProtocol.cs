// SPDX-License-Identifier: MIT
//
// EFI_SIMPLE_FILE_SYSTEM_PROTOCOL + EFI_FILE_PROTOCOL + EFI_FILE_INFO.
// Reference: MdePkg/Include/Protocol/SimpleFileSystem.h
//            MdePkg/Include/Guid/FileInfo.h, FileSystemInfo.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiSimpleFileSystemProtocol
{
    public ulong Revision;
    public delegate* unmanaged<EfiSimpleFileSystemProtocol*, EfiFileProtocol**, EfiStatus> OpenVolume;

    public const ulong RevisionValue = 0x00010000;
}

/// <summary>
/// EFI_FILE_PROTOCOL — UEFI 2.0 ("Revision 2") layout. Older firmware may
/// leave OpenEx/ReadEx/WriteEx/FlushEx slots NULL.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiFileProtocol
{
    public ulong Revision;

    public delegate* unmanaged<EfiFileProtocol*, EfiFileProtocol**, Char16*, ulong, ulong, EfiStatus> Open;
    public delegate* unmanaged<EfiFileProtocol*, EfiStatus>                                           Close;
    public delegate* unmanaged<EfiFileProtocol*, EfiStatus>                                           Delete;
    public delegate* unmanaged<EfiFileProtocol*, nuint*, void*, EfiStatus>                            Read;
    public delegate* unmanaged<EfiFileProtocol*, nuint*, void*, EfiStatus>                            Write;
    public delegate* unmanaged<EfiFileProtocol*, ulong*, EfiStatus>                                   GetPosition;
    public delegate* unmanaged<EfiFileProtocol*, ulong, EfiStatus>                                    SetPosition;
    public delegate* unmanaged<EfiFileProtocol*, EfiGuid*, nuint*, void*, EfiStatus>                  GetInfo;
    public delegate* unmanaged<EfiFileProtocol*, EfiGuid*, nuint, void*, EfiStatus>                   SetInfo;
    public delegate* unmanaged<EfiFileProtocol*, EfiStatus>                                           Flush;

    // Revision 2 extensions
    public delegate* unmanaged<EfiFileProtocol*, EfiFileProtocol**, Char16*, ulong, ulong, void*, EfiStatus> OpenEx;
    public delegate* unmanaged<EfiFileProtocol*, void*, EfiStatus>                                   ReadEx;
    public delegate* unmanaged<EfiFileProtocol*, void*, EfiStatus>                                   WriteEx;
    public delegate* unmanaged<EfiFileProtocol*, void*, EfiStatus>                                   FlushEx;

    public const ulong Revision1 = 0x00010000;
    public const ulong Revision2 = 0x00020000;
}

/// <summary>EFI_FILE_PROTOCOL.Open() mode bits.</summary>
public static class EfiFileMode
{
    public const ulong Read   = 0x0000000000000001;
    public const ulong Write  = 0x0000000000000002;
    public const ulong Create = 0x8000000000000000;
}

/// <summary>EFI_FILE_PROTOCOL attribute bits (also returned in EfiFileInfo).</summary>
public static class EfiFileAttribute
{
    public const ulong ReadOnly  = 0x0000000000000001;
    public const ulong Hidden    = 0x0000000000000002;
    public const ulong System    = 0x0000000000000004;
    public const ulong Reserved  = 0x0000000000000008;
    public const ulong Directory = 0x0000000000000010;
    public const ulong Archive   = 0x0000000000000020;
    public const ulong ValidAttr = 0x0000000000000037;
}

/// <summary>
/// EFI_FILE_INFO — variable-length struct returned by GetInfo(EFI_FILE_INFO_ID).
/// FileName is a CHAR16 string laid out immediately after this header.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct EfiFileInfo
{
    public ulong Size;          // Size of this structure incl. file name
    public ulong FileSize;
    public ulong PhysicalSize;
    public EfiTime CreateTime;
    public EfiTime LastAccessTime;
    public EfiTime ModificationTime;
    public ulong Attribute;
    // Char16 FileName[]; — variable-length tail
}

/// <summary>
/// EFI_FILE_SYSTEM_INFO — variable-length struct returned by
/// GetInfo(EFI_FILE_SYSTEM_INFO_ID). VolumeLabel is a CHAR16 string after the header.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public struct EfiFileSystemInfo
{
    public ulong Size;
    public byte  ReadOnly;
    public ulong VolumeSize;
    public ulong FreeSpace;
    public uint  BlockSize;
    // Char16 VolumeLabel[];
}
