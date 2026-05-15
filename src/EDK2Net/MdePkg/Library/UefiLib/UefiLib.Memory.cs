// SPDX-License-Identifier: MIT
//
// UefiLib — Memory Services convenience.
//
// AllocatePages / FreePages wrappers and a self-growing GetMemoryMap that
// handles the classic "buffer too small" retry loop in one call.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    public const nuint EfiPageSize = 4096;

    /// <summary>EFI_SIZE_TO_PAGES(size) — round up to whole 4 KiB pages.</summary>
    public static nuint SizeToPages(nuint size)
        => (size + (EfiPageSize - 1)) / EfiPageSize;

    /// <summary>EFI_PAGES_TO_SIZE(pages).</summary>
    public static nuint PagesToSize(nuint pages)
        => pages * EfiPageSize;

    /// <summary>Allocate <paramref name="pages"/> 4 KiB pages of LoaderData.</summary>
    public static EfiStatus AllocatePages(nuint pages, out ulong physicalAddress)
    {
        ulong addr = 0;
        var s = BootServices->AllocatePages(EfiAllocateType.AnyPages, EfiMemoryType.LoaderData, pages, &addr);
        physicalAddress = addr;
        return s;
    }

    public static EfiStatus FreePages(ulong physicalAddress, nuint pages)
        => BootServices->FreePages(physicalAddress, pages);

    /// <summary>
    /// GetMemoryMap with automatic buffer growth. On success, <paramref name="buffer"/>
    /// receives a pool-allocated map (free with <see cref="FreePool"/>) and the out
    /// params describe the layout.
    /// </summary>
    public static EfiStatus GetMemoryMap(
        out void* buffer,
        out nuint mapSize,
        out nuint mapKey,
        out nuint descriptorSize,
        out uint  descriptorVersion)
    {
        void* buf = null;
        nuint sz = 0, key = 0, dsz = 0;
        uint  ver = 0;

        // First call: probe the required size.
        var st = BootServices->GetMemoryMap(&sz, buf, &key, &dsz, &ver);
        if (st != EfiStatus.BufferTooSmall && st.IsError)
        {
            buffer = null;
            mapSize = 0; mapKey = 0; descriptorSize = 0; descriptorVersion = 0;
            return st;
        }

        // The act of allocating may itself add a descriptor; over-allocate a few.
        sz += 8 * dsz;

        st = BootServices->AllocatePool(EfiMemoryType.LoaderData, sz, &buf);
        if (st.IsError)
        {
            buffer = null;
            mapSize = 0; mapKey = 0; descriptorSize = 0; descriptorVersion = 0;
            return st;
        }

        st = BootServices->GetMemoryMap(&sz, buf, &key, &dsz, &ver);
        if (st.IsError)
        {
            BootServices->FreePool(buf);
            buffer = null;
            mapSize = 0; mapKey = 0; descriptorSize = 0; descriptorVersion = 0;
            return st;
        }

        buffer = buf;
        mapSize = sz;
        mapKey = key;
        descriptorSize = dsz;
        descriptorVersion = ver;
        return EfiStatus.Success;
    }
}
