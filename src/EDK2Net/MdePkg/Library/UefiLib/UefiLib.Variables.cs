// SPDX-License-Identifier: MIT
//
// UefiLib — Runtime Services / variable convenience.
//
// Pointer-only API to keep the helpers usable from within stripped-down
// UEFI applications. Mirrors EDK2 GetVariable / SetVariable but accepts a
// managed `string` for the variable name (pinned in place during the call).
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Guid;
using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>
    /// EFI_VARIABLE_NON_VOLATILE | EFI_VARIABLE_BOOTSERVICE_ACCESS | EFI_VARIABLE_RUNTIME_ACCESS
    /// — the typical attribute mask for persistent global variables.
    /// </summary>
    public const uint EfiVariableNvBsRt =
        EfiVariableAttribute.NonVolatile |
        EfiVariableAttribute.BootserviceAccess |
        EfiVariableAttribute.RuntimeAccess;

    /// <summary>
    /// Read a UEFI variable. The caller supplies a pre-sized <paramref name="buffer"/>;
    /// on STATUS_BUFFER_TOO_SMALL the firmware updates <paramref name="size"/> with
    /// the required byte count.
    /// </summary>
    public static EfiStatus GetVariable(
        string name, in EfiGuid vendorGuid,
        uint* attributes, nuint* size, void* buffer)
    {
        fixed (char* nm = name)
        fixed (EfiGuid* g = &vendorGuid)
        {
            return RuntimeServices->GetVariable((Char16*)nm, g, attributes, size, buffer);
        }
    }

    /// <summary>Read a global UEFI variable (uses EFI_GLOBAL_VARIABLE_GUID).</summary>
    public static EfiStatus GetGlobalVariable(string name, uint* attributes, nuint* size, void* buffer)
        => GetVariable(name, EfiGuids.GlobalVariable, attributes, size, buffer);

    /// <summary>
    /// Write a UEFI variable. Pass <paramref name="size"/>=0 with attributes=0 to delete.
    /// </summary>
    public static EfiStatus SetVariable(
        string name, in EfiGuid vendorGuid,
        uint attributes, nuint size, void* data)
    {
        fixed (char* nm = name)
        fixed (EfiGuid* g = &vendorGuid)
        {
            return RuntimeServices->SetVariable((Char16*)nm, g, attributes, size, data);
        }
    }

    /// <summary>Delete a UEFI variable.</summary>
    public static EfiStatus DeleteVariable(string name, in EfiGuid vendorGuid)
        => SetVariable(name, vendorGuid, 0, 0, null);

    /// <summary>Get current platform time via Runtime Services.</summary>
    public static EfiStatus GetTime(out EfiTime time)
    {
        EfiTime t;
        var s = RuntimeServices->GetTime(&t, null);
        time = t;
        return s;
    }

    /// <summary>QueryVariableInfo for a particular attribute mask.</summary>
    public static EfiStatus QueryVariableInfo(
        uint attributes,
        out ulong maxStorageSize,
        out ulong remainingStorageSize,
        out ulong maxVariableSize)
    {
        ulong max = 0, rem = 0, mvs = 0;
        var s = RuntimeServices->QueryVariableInfo(attributes, &max, &rem, &mvs);
        maxStorageSize = max;
        remainingStorageSize = rem;
        maxVariableSize = mvs;
        return s;
    }
}
