// SPDX-License-Identifier: MIT
// EFI_DISK_IO_PROTOCOL — byte-granular reads/writes layered on Block I/O.
// Reference: MdePkg/Include/Protocol/DiskIo.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiDiskIoProtocol
{
    public ulong Revision;
    public delegate* unmanaged<EfiDiskIoProtocol*, uint, ulong, nuint, void*, EfiStatus> ReadDisk;
    public delegate* unmanaged<EfiDiskIoProtocol*, uint, ulong, nuint, void*, EfiStatus> WriteDisk;

    public const ulong Revision1 = 0x00010000;
}
