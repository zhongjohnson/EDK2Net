// SPDX-License-Identifier: MIT
// EFI_BOOT_SERVICES — skeleton matching UEFI 2.x layout. Only the fields most
// commonly used by C# UEFI applications are typed; the remainder are kept as
// opaque function pointers to preserve the correct offsets.
//
// Reference: MdePkg/Include/Uefi/UefiSpec.h, struct _EFI_BOOT_SERVICES.
namespace EDK2Net.MdePkg.Uefi;

using System.Runtime.InteropServices;

public enum EfiAllocateType : uint
{
    AnyPages    = 0,
    MaxAddress  = 1,
    Address     = 2,
}

public enum EfiMemoryType : uint
{
    Reserved              = 0,
    LoaderCode            = 1,
    LoaderData            = 2,
    BootServicesCode      = 3,
    BootServicesData      = 4,
    RuntimeServicesCode   = 5,
    RuntimeServicesData   = 6,
    Conventional          = 7,
    Unusable              = 8,
    AcpiReclaim           = 9,
    AcpiNvs               = 10,
    MemoryMappedIo        = 11,
    MemoryMappedIoPortSpace = 12,
    PalCode               = 13,
    Persistent            = 14,
}

public enum EfiLocateSearchType : uint
{
    AllHandles  = 0,
    ByRegisterNotify = 1,
    ByProtocol  = 2,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiBootServices
{
    public EfiTableHeader Hdr;

    // ---- Task Priority Services ------------------------------------------
    public delegate* unmanaged<nuint, nuint> RaiseTpl;
    public delegate* unmanaged<nuint, void>  RestoreTpl;

    // ---- Memory Services -------------------------------------------------
    public delegate* unmanaged<EfiAllocateType, EfiMemoryType, nuint, ulong*, EfiStatus> AllocatePages;
    public delegate* unmanaged<ulong, nuint, EfiStatus>                                    FreePages;
    public delegate* unmanaged<nuint*, void*, nuint*, nuint*, uint*, EfiStatus>            GetMemoryMap;
    public delegate* unmanaged<EfiMemoryType, nuint, void**, EfiStatus>                    AllocatePool;
    public delegate* unmanaged<void*, EfiStatus>                                           FreePool;

    // ---- Event & Timer Services -----------------------------------------
    public delegate* unmanaged<uint, nuint, void*, void*, EfiEvent*, EfiStatus> CreateEvent;
    public delegate* unmanaged<EfiEvent, uint, ulong, EfiStatus>                SetTimer;
    public delegate* unmanaged<nuint, EfiEvent*, nuint*, EfiStatus>             WaitForEvent;
    public delegate* unmanaged<EfiEvent, EfiStatus>                             SignalEvent;
    public delegate* unmanaged<EfiEvent, EfiStatus>                             CloseEvent;
    public delegate* unmanaged<EfiEvent, EfiStatus>                             CheckEvent;

    // ---- Protocol Handler Services --------------------------------------
    public delegate* unmanaged<EfiHandle*, EfiGuid*, uint, void*, EfiStatus>        InstallProtocolInterface;
    public delegate* unmanaged<EfiHandle, EfiGuid*, void*, void*, EfiStatus>        ReinstallProtocolInterface;
    public delegate* unmanaged<EfiHandle, EfiGuid*, void*, EfiStatus>               UninstallProtocolInterface;
    public delegate* unmanaged<EfiHandle, EfiGuid*, void**, EfiStatus>              HandleProtocol;
    public void* Reserved;
    public delegate* unmanaged<EfiGuid*, EfiEvent, void**, EfiStatus>               RegisterProtocolNotify;
    public delegate* unmanaged<EfiLocateSearchType, EfiGuid*, void*, nuint*, EfiHandle*, EfiStatus> LocateHandle;
    public delegate* unmanaged<EfiGuid*, void*, void*, EfiStatus>                   LocateDevicePath;
    public delegate* unmanaged<EfiGuid*, void*, EfiStatus>                          InstallConfigurationTable;

    // ---- Image Services -------------------------------------------------
    public delegate* unmanaged<byte, EfiHandle, void*, void*, nuint, EfiHandle*, EfiStatus> LoadImage;
    public delegate* unmanaged<EfiHandle, nuint*, Char16**, EfiStatus>                       StartImage;
    public delegate* unmanaged<EfiStatus, nuint, Char16*, void> Exit; // does not return on success
    public delegate* unmanaged<EfiHandle, EfiStatus>                                          UnloadImage;
    public delegate* unmanaged<EfiHandle, nuint, EfiStatus>                                   ExitBootServices;

    // ---- Miscellaneous Services -----------------------------------------
    public delegate* unmanaged<ulong*, EfiStatus>           GetNextMonotonicCount;
    public delegate* unmanaged<nuint, EfiStatus>            Stall; // microseconds
    public delegate* unmanaged<nuint, ulong, nuint, Char16*, EfiStatus> SetWatchdogTimer;

    // ---- DriverSupport Services (UEFI 1.1+) -----------------------------
    public delegate* unmanaged<EfiHandle, EfiHandle*, EfiHandle, byte, EfiStatus> ConnectController;
    public delegate* unmanaged<EfiHandle, EfiHandle, EfiHandle, EfiStatus>        DisconnectController;

    // ---- Open and Close Protocol Services -------------------------------
    public delegate* unmanaged<EfiHandle, EfiGuid*, void**, EfiHandle, EfiHandle, uint, EfiStatus> OpenProtocol;
    public delegate* unmanaged<EfiHandle, EfiGuid*, EfiHandle, EfiHandle, EfiStatus>               CloseProtocol;
    public delegate* unmanaged<EfiHandle, EfiGuid*, void*, nuint*, EfiStatus>                      OpenProtocolInformation;

    // ---- Library Services -----------------------------------------------
    public delegate* unmanaged<EfiHandle, EfiGuid***, nuint*, EfiStatus>                          ProtocolsPerHandle;
    public delegate* unmanaged<EfiLocateSearchType, EfiGuid*, void*, nuint*, EfiHandle**, EfiStatus> LocateHandleBuffer;
    public delegate* unmanaged<EfiGuid*, void*, void**, EfiStatus>                                LocateProtocol;
    public delegate* unmanaged<EfiHandle*, EfiStatus> InstallMultipleProtocolInterfaces;   // variadic in C
    public delegate* unmanaged<EfiHandle,  EfiStatus> UninstallMultipleProtocolInterfaces; // variadic in C

    // ---- 32-bit CRC Services --------------------------------------------
    public delegate* unmanaged<void*, nuint, uint*, EfiStatus> CalculateCrc32;

    // ---- Misc Services --------------------------------------------------
    public delegate* unmanaged<void*, void*, nuint, void> CopyMem;
    public delegate* unmanaged<void*, nuint, byte,  void> SetMem;
    public delegate* unmanaged<uint, nuint, void*, void*, EfiEvent*, EfiStatus> CreateEventEx;
}
