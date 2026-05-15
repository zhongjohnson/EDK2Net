// SPDX-License-Identifier: MIT
// EFI_LOADED_IMAGE_PROTOCOL — info about the currently running image.
// Reference: MdePkg/Include/Protocol/LoadedImage.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiLoadedImageProtocol
{
    public uint  Revision;          // EFI_LOADED_IMAGE_PROTOCOL_REVISION
    public EfiHandle ParentHandle;
    public EfiSystemTable* SystemTable;

    // Source location of the image
    public EfiHandle DeviceHandle;
    public void*     FilePath;      // EFI_DEVICE_PATH_PROTOCOL*
    public void*     Reserved;

    // Image's load options (command-line)
    public uint   LoadOptionsSize;
    public void*  LoadOptions;

    // Where the image was loaded into memory
    public void*           ImageBase;
    public ulong           ImageSize;
    public EfiMemoryType   ImageCodeType;
    public EfiMemoryType   ImageDataType;

    public delegate* unmanaged<EfiHandle, EfiStatus> Unload;

    public const uint Revision1 = 0x1000;
}
