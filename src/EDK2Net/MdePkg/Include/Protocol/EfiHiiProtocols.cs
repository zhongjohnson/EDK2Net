// SPDX-License-Identifier: MIT
//
// EFI_HII_DATABASE_PROTOCOL and EFI_HII_STRING_PROTOCOL — minimal bindings
// covering the most common HII operations:
//   * package-list registration (NewPackageList / RemovePackageList /
//     UpdatePackageList / ListPackageLists / ExportPackageLists)
//   * string lookups across handles (GetString / SetString / GetLanguages /
//     GetSecondaryLanguages / NewString)
//
// Less common members (font/image/keyboard registration, registered-callback
// helpers, etc.) are kept as opaque function pointers (`void*`) so the vtable
// layout matches firmware while we only commit to types we actually use.
//
// Reference: MdePkg/Include/Protocol/HiiDatabase.h, HiiString.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

/// <summary>EFI_HII_HANDLE — opaque handle to a registered HII package list.</summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct EfiHiiHandle
{
    public readonly nint Value;
    public EfiHiiHandle(nint value) => Value = value;
    public bool IsNull => Value == 0;
}

/// <summary>EFI_STRING_ID — 16-bit identifier into a HII string package.</summary>
public readonly struct EfiStringId
{
    public readonly ushort Value;
    public EfiStringId(ushort value) => Value = value;
    public static implicit operator ushort(EfiStringId id) => id.Value;
    public static implicit operator EfiStringId(ushort v) => new(v);
}

/// <summary>EFI_HII_PACKAGE_LIST_HEADER — leading header for a package list blob.</summary>
[StructLayout(LayoutKind.Sequential)]
public struct EfiHiiPackageListHeader
{
    public EfiGuid PackageListGuid;
    public uint    PackageLength;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiHiiDatabaseProtocol
{
    public delegate* unmanaged<EfiHiiDatabaseProtocol*, EfiHiiPackageListHeader*, EfiHandle, EfiHiiHandle*, EfiStatus> NewPackageList;
    public delegate* unmanaged<EfiHiiDatabaseProtocol*, EfiHiiHandle, EfiStatus> RemovePackageList;
    public delegate* unmanaged<EfiHiiDatabaseProtocol*, EfiHiiHandle, EfiHiiPackageListHeader*, EfiStatus> UpdatePackageList;
    public delegate* unmanaged<EfiHiiDatabaseProtocol*, byte, EfiGuid*, nuint*, EfiHiiHandle*, EfiStatus> ListPackageLists;
    public delegate* unmanaged<EfiHiiDatabaseProtocol*, EfiHiiHandle, nuint*, EfiHiiPackageListHeader*, EfiStatus> ExportPackageLists;

    // Callback / font / image / keyboard layout entries kept opaque so the
    // vtable offsets stay correct without committing to every type yet.
    public void* RegisterPackageNotify;
    public void* UnregisterPackageNotify;
    public void* FindKeyboardLayouts;
    public void* GetKeyboardLayout;
    public void* SetKeyboardLayout;
    public void* GetPackageListHandle;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiHiiStringProtocol
{
    /// <summary>Add a new string to a package list and return its assigned ID.</summary>
    public delegate* unmanaged<EfiHiiStringProtocol*, EfiHiiHandle, EfiStringId*, byte*, Char16*, void*, EfiStatus> NewString;

    /// <summary>Look up a string by handle/language/id. The buffer is CHAR16; the
    /// caller passes the buffer length in bytes (size_t in/out).</summary>
    public delegate* unmanaged<EfiHiiStringProtocol*, byte*, EfiHiiHandle, EfiStringId, Char16*, nuint*, void*, EfiStatus> GetString;

    /// <summary>Replace an existing string.</summary>
    public delegate* unmanaged<EfiHiiStringProtocol*, EfiHiiHandle, EfiStringId, byte*, Char16*, void*, EfiStatus> SetString;

    /// <summary>Enumerate primary languages for a handle (RFC 4646, ASCII).</summary>
    public delegate* unmanaged<EfiHiiStringProtocol*, EfiHiiHandle, byte*, nuint*, EfiStatus> GetLanguages;

    /// <summary>Enumerate secondary languages for a handle/primary-language.</summary>
    public delegate* unmanaged<EfiHiiStringProtocol*, EfiHiiHandle, byte*, byte*, nuint*, EfiStatus> GetSecondaryLanguages;
}
