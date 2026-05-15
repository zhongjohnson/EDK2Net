// SPDX-License-Identifier: MIT
//
// UefiLib
// =======
// Thin ergonomic layer over the raw Boot Services / System Table pointers.
// Mirrors the spirit of EDK2 MdePkg/Library/UefiLib + UefiBootServicesTableLib:
// initialize once with the (ImageHandle, SystemTable) pair from EfiMain, then
// call the helpers from anywhere without threading those pointers through
// every function.
//
// All helpers are AOT/UEFI-safe: no managed allocations, no reflection, no
// exceptions. They return EfiStatus directly.
namespace EDK2Net.MdePkg.Library.UefiLib;

using System.Runtime.CompilerServices;
using EDK2Net.MdePkg.Uefi;
using EDK2Net.MdePkg.Guid;
using EDK2Net.ShellPkg.Guid;
using EDK2Net.MdePkg.Protocol;
using EDK2Net.ShellPkg.Protocol;
using EDK2Net.MdePkg.IndustryStandard;
public static unsafe partial class UefiLib
{
    private static EfiSystemTable*    s_st;
    private static EfiHandle          s_imageHandle;

    public static EfiSystemTable*     SystemTable     => s_st;
    public static EfiBootServices*    BootServices    => s_st->BootServices;
    public static EfiRuntimeServices* RuntimeServices => s_st->RuntimeServices;
    public static EfiSimpleTextOutputProtocol* ConOut => s_st->ConOut;
    public static EfiSimpleTextInputProtocol*  ConIn  => s_st->ConIn;
    public static EfiHandle           ImageHandle     => s_imageHandle;

    /// <summary>Call once at the start of EfiMain.</summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Initialize(EfiHandle imageHandle, EfiSystemTable* systemTable)
    {
        s_imageHandle = imageHandle;
        s_st = systemTable;
    }

    // -----------------------------------------------------------------------
    // Console
    // -----------------------------------------------------------------------

    /// <summary>Print a UTF-16 string (no allocation; pins the literal/string).</summary>
    public static EfiStatus Print(string text)
    {
        fixed (char* p = text)
        {
            return ConOut->OutputString(ConOut, (Char16*)p);
        }
    }

    /// <summary>Print + CR/LF.</summary>
    public static EfiStatus PrintLine(string text)
    {
        var s = Print(text);
        if (s.IsError) return s;
        return Print("\r\n");
    }

    // -----------------------------------------------------------------------
    // Protocol location
    // -----------------------------------------------------------------------

    /// <summary>BootServices->LocateProtocol with a managed-friendly signature.</summary>
    public static EfiStatus LocateProtocol(in EfiGuid protocolGuid, void** outInterface)
    {
        fixed (EfiGuid* g = &protocolGuid)
        {
            return BootServices->LocateProtocol(g, null, outInterface);
        }
    }

    /// <summary>BootServices->HandleProtocol with a managed-friendly signature.</summary>
    public static EfiStatus HandleProtocol(EfiHandle handle, in EfiGuid protocolGuid, void** outInterface)
    {
        fixed (EfiGuid* g = &protocolGuid)
        {
            return BootServices->HandleProtocol(handle, g, outInterface);
        }
    }

    // -----------------------------------------------------------------------
    // Misc Boot Services convenience
    // -----------------------------------------------------------------------

    /// <summary>Block for <paramref name="microseconds"/> using BootServices->Stall.</summary>
    public static EfiStatus Stall(nuint microseconds)
        => BootServices->Stall(microseconds);

    /// <summary>Allocate a pool of <see cref="EfiMemoryType.LoaderData"/>.</summary>
    public static EfiStatus AllocatePool(nuint size, void** outBuffer)
        => BootServices->AllocatePool(EfiMemoryType.LoaderData, size, outBuffer);

    public static EfiStatus FreePool(void* buffer)
        => BootServices->FreePool(buffer);

    /// <summary>Block until any key is pressed.</summary>
    public static void WaitForKey()
    {
        var conIn = ConIn;
        var bs    = BootServices;

        EfiEvent ev = conIn->WaitForKey;
        nuint index;
        bs->WaitForEvent(1, &ev, &index);

        EfiInputKey key;
        conIn->ReadKeyStroke(conIn, &key);
    }

    // -----------------------------------------------------------------------
    // Runtime Services convenience
    // -----------------------------------------------------------------------

    /// <summary>Reset the platform. Does not return on success.</summary>
    public static void ResetSystem(EfiResetType resetType, EfiStatus status)
        => RuntimeServices->ResetSystem(resetType, status, 0, null);

    // -----------------------------------------------------------------------
    // Configuration tables
    // -----------------------------------------------------------------------

    /// <summary>Find a configuration table by GUID. Returns null if absent.</summary>
    public static void* GetConfigurationTable(in EfiGuid vendorGuid)
    {
        var n = s_st->NumberOfTableEntries;
        var t = s_st->ConfigurationTable;
        for (nuint i = 0; i < n; i++)
        {
            if (GuidEquals(t[i].VendorGuid, vendorGuid))
                return t[i].VendorTable;
        }
        return null;
    }

    private static bool GuidEquals(in EfiGuid a, in EfiGuid b)
    {
        // Two 16-byte blobs — compare as two ulongs.
        fixed (EfiGuid* pa = &a)
        fixed (EfiGuid* pb = &b)
        {
            ulong* la = (ulong*)pa;
            ulong* lb = (ulong*)pb;
            return la[0] == lb[0] && la[1] == lb[1];
        }
    }

    // -----------------------------------------------------------------------
    // Industry-standard table convenience
    // -----------------------------------------------------------------------

    /// <summary>Locate the ACPI 2.0+ RSDP via EFI_ACPI_20_TABLE_GUID.</summary>
    public static AcpiRsdp20* GetAcpi20Rsdp()
        => (AcpiRsdp20*)GetConfigurationTable(EfiGuids.Acpi20Table);

    /// <summary>Locate the legacy ACPI 1.0 RSDP via EFI_ACPI_TABLE_GUID.</summary>
    public static AcpiRsdp10* GetAcpi10Rsdp()
        => (AcpiRsdp10*)GetConfigurationTable(EfiGuids.Acpi10Table);

    /// <summary>Locate the SMBIOS 3.x entry point via SMBIOS3_TABLE_GUID.</summary>
    public static SmBios3EntryPoint* GetSmbios3EntryPoint()
        => (SmBios3EntryPoint*)GetConfigurationTable(EfiGuids.Smbios3Table);

    /// <summary>Locate the SMBIOS 2.x entry point via SMBIOS_TABLE_GUID.</summary>
    public static SmBios2EntryPoint* GetSmbios2EntryPoint()
        => (SmBios2EntryPoint*)GetConfigurationTable(EfiGuids.SmbiosTable);

    // -----------------------------------------------------------------------
    // Shell convenience
    // -----------------------------------------------------------------------

    /// <summary>
    /// Try to get the EFI_SHELL_PARAMETERS_PROTOCOL installed by the UEFI
    /// Shell on this image's handle. Returns null when not launched from
    /// the Shell.
    /// </summary>
    public static EfiShellParametersProtocol* GetShellParameters()
    {
        EfiShellParametersProtocol* p;
        var s = HandleProtocol(s_imageHandle, ShellGuids.ShellParameters, (void**)&p);
        return s.IsSuccess ? p : null;
    }
}
