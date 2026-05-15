// SPDX-License-Identifier: MIT
//
// ACPI 1.0 / 2.0+ structures used by UEFI applications.
// Reference: MdePkg/Include/IndustryStandard/Acpi*.h plus the ACPI Specification.
//
// Layout rule: every ACPI table is byte-packed (Pack = 1, LayoutKind.Sequential).
namespace EDK2Net.MdePkg.IndustryStandard;

using System.Runtime.InteropServices;

// ---------------------------------------------------------------------------
// Common header
// ---------------------------------------------------------------------------

/// <summary>EFI_ACPI_DESCRIPTION_HEADER — common 36-byte header for SDTs.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct AcpiDescriptionHeader
{
    public uint   Signature;
    public uint   Length;
    public byte   Revision;
    public byte   Checksum;
    public fixed byte OemId[6];
    public ulong  OemTableId;
    public uint   OemRevision;
    public uint   CreatorId;
    public uint   CreatorRevision;
}

// ---------------------------------------------------------------------------
// RSDP (Root System Description Pointer) — pointed to by the
// EFI_ACPI_TABLE_GUID / EFI_ACPI_20_TABLE_GUID configuration tables.
// ---------------------------------------------------------------------------

/// <summary>RSDP, ACPI 1.0 (20 bytes).</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 20)]
public unsafe struct AcpiRsdp10
{
    public ulong Signature;          // "RSD PTR "
    public byte  Checksum;
    public fixed byte OemId[6];
    public byte  Revision;           // 0 for ACPI 1.0
    public uint  RsdtAddress;        // 32-bit physical address of RSDT
}

/// <summary>RSDP, ACPI 2.0+ (36 bytes; first 20 bytes match RSDP 1.0).</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 36)]
public unsafe struct AcpiRsdp20
{
    public ulong Signature;
    public byte  Checksum;           // 1.0-portion checksum
    public fixed byte OemId[6];
    public byte  Revision;           // 2 for ACPI 2.0+
    public uint  RsdtAddress;
    public uint  Length;             // total length of RSDP
    public ulong XsdtAddress;        // 64-bit physical address of XSDT
    public byte  ExtendedChecksum;   // checksum of full 36 bytes
    public fixed byte Reserved[3];
}

// ---------------------------------------------------------------------------
// XSDT / RSDT — header followed by an array of pointers.
// We only model the header; consumers iterate the trailing pointer array.
// ---------------------------------------------------------------------------

/// <summary>XSDT — header followed by an array of UINT64 table pointers.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AcpiXsdt
{
    public AcpiDescriptionHeader Header;
    // ulong Entry[]; — number of entries = (Length - sizeof(Header)) / 8
}

/// <summary>RSDT — header followed by an array of UINT32 table pointers.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AcpiRsdt
{
    public AcpiDescriptionHeader Header;
    // uint Entry[]; — number of entries = (Length - sizeof(Header)) / 4
}

// ---------------------------------------------------------------------------
// FADT (Fixed ACPI Description Table) — the wire-up to FACS + DSDT.
// We bind the ACPI 6.x layout, which is upward compatible with older firmware
// (older firmware ignores trailing fields).
// ---------------------------------------------------------------------------

[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 12)]
public struct AcpiGenericAddressStructure
{
    public byte AddressSpaceId;      // 0 = system memory, 1 = system I/O, ...
    public byte RegisterBitWidth;
    public byte RegisterBitOffset;
    public byte AccessSize;
    public ulong Address;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct AcpiFadt
{
    public AcpiDescriptionHeader Header;
    public uint  FirmwareCtrl;
    public uint  Dsdt;
    public byte  Reserved0;
    public byte  PreferredPmProfile;
    public ushort SciInt;
    public uint  SmiCmd;
    public byte  AcpiEnable;
    public byte  AcpiDisable;
    public byte  S4BiosReq;
    public byte  PstateCnt;
    public uint  Pm1aEvtBlk;
    public uint  Pm1bEvtBlk;
    public uint  Pm1aCntBlk;
    public uint  Pm1bCntBlk;
    public uint  Pm2CntBlk;
    public uint  PmTmrBlk;
    public uint  Gpe0Blk;
    public uint  Gpe1Blk;
    public byte  Pm1EvtLen;
    public byte  Pm1CntLen;
    public byte  Pm2CntLen;
    public byte  PmTmrLen;
    public byte  Gpe0BlkLen;
    public byte  Gpe1BlkLen;
    public byte  Gpe1Base;
    public byte  CstCnt;
    public ushort PLvl2Lat;
    public ushort PLvl3Lat;
    public ushort FlushSize;
    public ushort FlushStride;
    public byte  DutyOffset;
    public byte  DutyWidth;
    public byte  DayAlrm;
    public byte  MonAlrm;
    public byte  Century;
    public ushort IaPcBootArch;
    public byte  Reserved1;
    public uint  Flags;

    public AcpiGenericAddressStructure ResetReg;
    public byte  ResetValue;
    public ushort ArmBootArch;
    public byte  FadtMinorVersion;
    public ulong XFirmwareCtrl;
    public ulong XDsdt;
    public AcpiGenericAddressStructure XPm1aEvtBlk;
    public AcpiGenericAddressStructure XPm1bEvtBlk;
    public AcpiGenericAddressStructure XPm1aCntBlk;
    public AcpiGenericAddressStructure XPm1bCntBlk;
    public AcpiGenericAddressStructure XPm2CntBlk;
    public AcpiGenericAddressStructure XPmTmrBlk;
    public AcpiGenericAddressStructure XGpe0Blk;
    public AcpiGenericAddressStructure XGpe1Blk;
    public AcpiGenericAddressStructure SleepControlReg;
    public AcpiGenericAddressStructure SleepStatusReg;
    public ulong HypervisorVendorIdentity;
}

// ---------------------------------------------------------------------------
// MADT (Multiple APIC Description Table)
// ---------------------------------------------------------------------------

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AcpiMadt
{
    public AcpiDescriptionHeader Header;
    public uint LocalApicAddress;
    public uint Flags;
    // Followed by a stream of AcpiMadtSubHeader-prefixed records.
}

/// <summary>Header for each interrupt-controller-structure record inside the MADT.</summary>
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct AcpiMadtSubHeader
{
    public byte Type;     // 0=ProcessorLocalApic, 1=IoApic, ...
    public byte Length;   // Length of this entire record
}

public static class AcpiMadtType
{
    public const byte ProcessorLocalApic         = 0x00;
    public const byte IoApic                     = 0x01;
    public const byte InterruptSourceOverride    = 0x02;
    public const byte NonMaskableInterruptSource = 0x03;
    public const byte LocalApicNmi               = 0x04;
    public const byte LocalApicAddressOverride   = 0x05;
    public const byte IoSapic                    = 0x06;
    public const byte LocalSapic                 = 0x07;
    public const byte PlatformInterruptSources   = 0x08;
    public const byte ProcessorLocalX2Apic       = 0x09;
    public const byte LocalX2ApicNmi             = 0x0A;
    public const byte GiccCpuInterface           = 0x0B;
    public const byte GicdInterruptDistributor   = 0x0C;
}

// ---------------------------------------------------------------------------
// Signature constants (4-char ASCII LE; "FACP" -> 'F'|'A'<<8|'C'<<16|'P'<<24)
// ---------------------------------------------------------------------------

public static class AcpiSignature
{
    public const ulong Rsdp = 0x2052545020445352UL; // "RSD PTR "

    public const uint Fadt = 0x50434146; // "FACP"
    public const uint Madt = 0x43495041; // "APIC"
    public const uint Hpet = 0x54455048; // "HPET"
    public const uint Mcfg = 0x4746434D; // "MCFG"
    public const uint Bgrt = 0x54524742; // "BGRT"
    public const uint Tpm2 = 0x324D5054; // "TPM2"
    public const uint Dsdt = 0x54445344; // "DSDT"
    public const uint Ssdt = 0x54445353; // "SSDT"
    public const uint Xsdt = 0x54445358; // "XSDT"
    public const uint Rsdt = 0x54445352; // "RSDT"
    public const uint Facs = 0x53434146; // "FACS"
    public const uint Slit = 0x54494C53; // "SLIT"
    public const uint Srat = 0x54415253; // "SRAT"
    public const uint Dmar = 0x52414D44; // "DMAR"
}

// ---------------------------------------------------------------------------
// Walk helpers
// ---------------------------------------------------------------------------

public static unsafe class Acpi
{
    /// <summary>
    /// Walk an XSDT and return the first entry whose header has the given
    /// signature, or null if not found.
    /// </summary>
    public static AcpiDescriptionHeader* FindInXsdt(AcpiXsdt* xsdt, uint signature)
    {
        if (xsdt == null) return null;
        nuint headerSize = (nuint)sizeof(AcpiDescriptionHeader);
        nuint count = (xsdt->Header.Length - headerSize) / 8;
        ulong* entries = (ulong*)((byte*)xsdt + headerSize);
        for (nuint i = 0; i < count; i++)
        {
            var hdr = (AcpiDescriptionHeader*)(nuint)entries[i];
            if (hdr != null && hdr->Signature == signature) return hdr;
        }
        return null;
    }

    /// <summary>Same as <see cref="FindInXsdt"/> but for legacy 32-bit RSDT pointers.</summary>
    public static AcpiDescriptionHeader* FindInRsdt(AcpiRsdt* rsdt, uint signature)
    {
        if (rsdt == null) return null;
        nuint headerSize = (nuint)sizeof(AcpiDescriptionHeader);
        nuint count = (rsdt->Header.Length - headerSize) / 4;
        uint* entries = (uint*)((byte*)rsdt + headerSize);
        for (nuint i = 0; i < count; i++)
        {
            var hdr = (AcpiDescriptionHeader*)(nuint)entries[i];
            if (hdr != null && hdr->Signature == signature) return hdr;
        }
        return null;
    }
}
