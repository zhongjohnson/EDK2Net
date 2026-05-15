// SPDX-License-Identifier: MIT
// EFI_HANDLE and EFI_EVENT — opaque pointers.
namespace EDK2Net.MdePkg.Uefi;

using System;

/// <summary>EFI_HANDLE — opaque pointer to a UEFI object/handle database entry.</summary>
public readonly unsafe struct EfiHandle
{
    public readonly void* Value;
    public EfiHandle(void* value) => Value = value;
    public bool IsNull => Value == null;
    public static EfiHandle Null => new(null);
}

/// <summary>EFI_EVENT — opaque pointer used by event services.</summary>
public readonly unsafe struct EfiEvent
{
    public readonly void* Value;
    public EfiEvent(void* value) => Value = value;
    public bool IsNull => Value == null;
    public static EfiEvent Null => new(null);
}
