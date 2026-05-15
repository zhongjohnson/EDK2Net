// SPDX-License-Identifier: MIT
// EFI_RUNTIME_SERVICES — typed skeleton matching UEFI 2.x layout.
namespace EDK2Net.MdePkg.Uefi;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiTime
{
    public ushort Year;
    public byte   Month;
    public byte   Day;
    public byte   Hour;
    public byte   Minute;
    public byte   Second;
    public byte   Pad1;
    public uint   Nanosecond;
    public short  TimeZone;
    public byte   Daylight;
    public byte   Pad2;
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiTimeCapabilities
{
    public uint Resolution;
    public uint Accuracy;
    public byte SetsToZero;
}

public enum EfiResetType : uint
{
    Cold       = 0,
    Warm       = 1,
    Shutdown   = 2,
    PlatformSpecific = 3,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiRuntimeServices
{
    public EfiTableHeader Hdr;

    // ---- Time Services ---------------------------------------------------
    public delegate* unmanaged<EfiTime*, EfiTimeCapabilities*, EfiStatus> GetTime;
    public delegate* unmanaged<EfiTime*, EfiStatus>                       SetTime;
    public delegate* unmanaged<byte*, byte*, EfiTime*, EfiStatus>         GetWakeupTime;
    public delegate* unmanaged<byte, EfiTime*, EfiStatus>                 SetWakeupTime;

    // ---- Virtual Memory Services ----------------------------------------
    public delegate* unmanaged<nuint, nuint, uint, void*, EfiStatus>      SetVirtualAddressMap;
    public delegate* unmanaged<uint, void**, EfiStatus>                   ConvertPointer;

    // ---- Variable Services ----------------------------------------------
    public delegate* unmanaged<Char16*, EfiGuid*, uint*, nuint*, void*, EfiStatus> GetVariable;
    public delegate* unmanaged<nuint*, Char16*, EfiGuid*, EfiStatus>               GetNextVariableName;
    public delegate* unmanaged<Char16*, EfiGuid*, uint, nuint, void*, EfiStatus>   SetVariable;

    // ---- Miscellaneous Services -----------------------------------------
    public delegate* unmanaged<uint*, EfiStatus>                                     GetNextHighMonotonicCount;
    public delegate* unmanaged<EfiResetType, EfiStatus, nuint, void*, void>          ResetSystem;

    // ---- UEFI 2.0+ Capsule Services -------------------------------------
    public delegate* unmanaged<void**, nuint, ulong, EfiStatus>                      UpdateCapsule;
    public delegate* unmanaged<void**, nuint, ulong*, uint*, EfiStatus>              QueryCapsuleCapabilities;
    public delegate* unmanaged<uint, ulong*, ulong*, ulong*, EfiStatus>              QueryVariableInfo;
}
