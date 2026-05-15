// SPDX-License-Identifier: MIT
//
// MemMap — print the firmware memory map. Demonstrates UefiLib.GetMemoryMap
// (auto-growing buffer) and walks descriptors using the firmware-reported
// DescriptorSize, which may exceed sizeof(EFI_MEMORY_DESCRIPTOR).

using System.Runtime.InteropServices;
using EDK2Net.MdePkg.Library.UefiLib;
using EDK2Net.MdePkg.Uefi;

namespace MemMap;

public static unsafe class Program
{
    public static int Main() => 0;

    [UnmanagedCallersOnly(EntryPoint = "EfiMain")]
    public static nuint EfiMain(EfiHandle imageHandle, EfiSystemTable* systemTable)
    {
        UefiLib.Initialize(imageHandle, systemTable);
        EDK2Net.Runtime.RuntimeStubs.KeepAlive();

        UefiLib.PrintLine("MemMap — EDK2Net sample");
        UefiLib.PrintLine("=======================");

        var s = UefiLib.GetMemoryMap(out var buffer, out var mapSize, out var mapKey,
                                     out var descriptorSize, out var descriptorVersion);
        if (s.IsError || buffer == null)
        {
            UefiLib.PrintLine("GetMemoryMap failed.");
            UefiLib.WaitForKey();
            return EfiStatus.Success;
        }

        try
        {
            nuint count = mapSize / descriptorSize;
            UefiLib.Print("Entries: ");
            UefiLib.PrintDec(count);
            UefiLib.Print("  DescriptorSize: ");
            UefiLib.PrintDec(descriptorSize);
            UefiLib.Print("  Version: ");
            UefiLib.PrintDec(descriptorVersion);
            UefiLib.PrintLine("");
            UefiLib.PrintLine("Type                       PhysStart           Pages       Attr");

            byte* p = (byte*)buffer;
            ulong totalPages = 0;
            for (nuint i = 0; i < count; i++, p += descriptorSize)
            {
                var d = *(EfiMemoryDescriptor*)p;
                totalPages += d.NumberOfPages;

                PrintTypeName((EfiMemoryType)d.Type);
                UefiLib.Print("  ");
                UefiLib.PrintHex0x(d.PhysicalStart);
                UefiLib.Print("  ");
                PrintRightDec(d.NumberOfPages, 10);
                UefiLib.Print("  ");
                UefiLib.PrintHex0x(d.Attribute);
                UefiLib.PrintLine("");
            }

            UefiLib.Print("Total pages: ");
            UefiLib.PrintDec(totalPages);
            UefiLib.Print("  (~");
            UefiLib.PrintDec(totalPages * 4 / 1024); // 4 KiB pages -> MiB
            UefiLib.PrintLine(" MiB)");
        }
        finally
        {
            UefiLib.FreePool(buffer);
        }

        UefiLib.PrintLine("");
        UefiLib.PrintLine("Press any key to exit...");
        UefiLib.WaitForKey();
        return EfiStatus.Success;
    }

    private static void PrintTypeName(EfiMemoryType type)
    {
        string name = type switch
        {
            EfiMemoryType.Reserved                => "Reserved              ",
            EfiMemoryType.LoaderCode              => "LoaderCode            ",
            EfiMemoryType.LoaderData              => "LoaderData            ",
            EfiMemoryType.BootServicesCode        => "BootServicesCode      ",
            EfiMemoryType.BootServicesData        => "BootServicesData      ",
            EfiMemoryType.RuntimeServicesCode     => "RuntimeServicesCode   ",
            EfiMemoryType.RuntimeServicesData     => "RuntimeServicesData   ",
            EfiMemoryType.Conventional            => "Conventional          ",
            EfiMemoryType.Unusable                => "Unusable              ",
            EfiMemoryType.AcpiReclaim             => "AcpiReclaim           ",
            EfiMemoryType.AcpiNvs                 => "AcpiNvs               ",
            EfiMemoryType.MemoryMappedIo          => "MemoryMappedIo        ",
            EfiMemoryType.MemoryMappedIoPortSpace => "MemoryMappedIoPortSpc ",
            EfiMemoryType.PalCode                 => "PalCode               ",
            EfiMemoryType.Persistent              => "Persistent            ",
            _                                     => "Other                 ",
        };
        UefiLib.Print(name);
    }

    private static void PrintRightDec(ulong value, int width)
    {
        // Cheap right-align: count digits, pad with spaces.
        int digits = 1;
        for (ulong v = value; v >= 10; v /= 10) digits++;
        for (int i = digits; i < width; i++) UefiLib.Print(" ");
        UefiLib.PrintDec(value);
    }
}
