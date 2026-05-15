// SPDX-License-Identifier: MIT
//
// SMBIOS 2.x / 3.x structures used by UEFI applications.
// Reference: MdePkg/Include/IndustryStandard/SmBios.h plus the DMTF SMBIOS spec.
//
// Layout rule: SMBIOS uses byte-packed little-endian; we set Pack = 1 throughout.
namespace EDK2Net.MdePkg.IndustryStandard;

using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------
// Entry points (the "anchors" pointed to by the SMBIOS configuration table)
// ---------------------------------------------------------------------------

/// <summary>SMBIOS 2.x entry point — anchor "_SM_".</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct SmBios2EntryPoint
{
    public fixed byte AnchorString[4];          // "_SM_"
    public byte  EntryPointStructureChecksum;
    public byte  EntryPointLength;
    public byte  MajorVersion;
    public byte  MinorVersion;
    public ushort MaxStructureSize;
    public byte  EntryPointRevision;
    public fixed byte FormattedArea[5];
    public fixed byte IntermediateAnchorString[5]; // "_DMI_"
    public byte  IntermediateChecksum;
    public ushort TableLength;
    public uint  TableAddress;                   // 32-bit physical
    public ushort NumberOfSmbiosStructures;
    public byte  SmbiosBcdRevision;
}

/// <summary>SMBIOS 3.x entry point — anchor "_SM3_".</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct SmBios3EntryPoint
{
    public fixed byte AnchorString[5];           // "_SM3_"
    public byte  EntryPointStructureChecksum;
    public byte  EntryPointLength;
    public byte  MajorVersion;
    public byte  MinorVersion;
    public byte  DocRev;
    public byte  EntryPointRevision;
    public byte  Reserved;
    public uint  TableMaximumSize;
    public ulong TableAddress;                   // 64-bit physical
}

// ---------------------------------------------------------------------------
// Common record header
// ---------------------------------------------------------------------------

/// <summary>SMBIOS_STRUCTURE — header of every SMBIOS record.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SmBiosStructure
{
    public byte   Type;
    public byte   Length;       // length of the formatted (binary) section, excluding strings
    public ushort Handle;
    // Formatted section follows, then a packed list of NUL-terminated ASCII
    // strings, terminated by an extra NUL (i.e. "..\0..\0\0"). If a record
    // has zero strings the formatted section is followed directly by "\0\0".
}

public static class SmBiosType
{
    public const byte BiosInformation             = 0;
    public const byte SystemInformation           = 1;
    public const byte BaseboardInformation        = 2;
    public const byte SystemEnclosure             = 3;
    public const byte ProcessorInformation        = 4;
    public const byte CacheInformation            = 7;
    public const byte PortConnectorInformation    = 8;
    public const byte SystemSlots                 = 9;
    public const byte OemStrings                  = 11;
    public const byte SystemConfigurationOptions  = 12;
    public const byte BiosLanguageInformation     = 13;
    public const byte GroupAssociations           = 14;
    public const byte PhysicalMemoryArray         = 16;
    public const byte MemoryDevice                = 17;
    public const byte MemoryArrayMappedAddress    = 19;
    public const byte SystemBootInformation       = 32;
    public const byte ManagementDevice            = 34;
    public const byte EndOfTable                  = 127;
}

// ---------------------------------------------------------------------------
// Common record bodies — Type 0 (BIOS) and Type 1 (System).
// String fields here are 1-based indices into the trailing string area.
// ---------------------------------------------------------------------------

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct SmBiosType0BiosInformation
{
    public SmBiosStructure Hdr;
    public byte   Vendor;             // string index
    public byte   BiosVersion;        // string index
    public ushort BiosSegment;
    public byte   BiosReleaseDate;    // string index
    public byte   BiosSize;
    public ulong  BiosCharacteristics;
    public byte   BiosCharacteristicsExtensionByte1;
    public byte   BiosCharacteristicsExtensionByte2;
    public byte   SystemBiosMajorRelease;
    public byte   SystemBiosMinorRelease;
    public byte   EmbeddedControllerFirmwareMajorRelease;
    public byte   EmbeddedControllerFirmwareMinorRelease;
    public ushort ExtendedBiosSize;   // 3.1+
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct SmBiosType1SystemInformation
{
    public SmBiosStructure Hdr;
    public byte  Manufacturer;        // string index
    public byte  ProductName;         // string index
    public byte  Version;             // string index
    public byte  SerialNumber;        // string index
    public fixed byte Uuid[16];
    public byte  WakeUpType;
    public byte  SkuNumber;           // string index
    public byte  Family;              // string index
}

// ---------------------------------------------------------------------------
// Walk / string helpers
// ---------------------------------------------------------------------------

public static unsafe class SmBios
{
    /// <summary>
    /// Return a pointer to the structure following <paramref name="record"/>.
    /// Skips the formatted section + the trailing "..\0..\0\0" string area.
    /// </summary>
    public static SmBiosStructure* Next(SmBiosStructure* record)
    {
        byte* p = (byte*)record + record->Length;
        // Walk strings: each terminated by NUL; double-NUL ends the area.
        while (true)
        {
            if (p[0] == 0 && p[1] == 0)
            {
                p += 2;
                break;
            }
            p++;
        }
        return (SmBiosStructure*)p;
    }

    /// <summary>
    /// Locate the first record of the given <paramref name="type"/> within an
    /// SMBIOS table region (anchor + length comes from the entry point).
    /// </summary>
    public static SmBiosStructure* Find(void* tableAddress, nuint tableLength, byte type)
    {
        if (tableAddress == null) return null;
        var p   = (SmBiosStructure*)tableAddress;
        var end = (byte*)tableAddress + tableLength;
        while ((byte*)p < end && p->Type != SmBiosType.EndOfTable)
        {
            if (p->Type == type) return p;
            p = Next(p);
        }
        return null;
    }

    /// <summary>
    /// Get the i'th (1-based) ASCII string from a record's trailing string area.
    /// Returns <c>null</c> if the index is 0 or out of range. The returned
    /// pointer references memory inside the SMBIOS table (do not free).
    /// </summary>
    public static byte* GetString(SmBiosStructure* record, byte index)
    {
        if (index == 0) return null;
        byte* p = (byte*)record + record->Length;
        for (byte i = 1; ; i++)
        {
            if (*p == 0) return null; // ran past the string area
            if (i == index) return p;
            // skip to the next NUL
            while (*p != 0) p++;
            p++;
        }
    }
}
