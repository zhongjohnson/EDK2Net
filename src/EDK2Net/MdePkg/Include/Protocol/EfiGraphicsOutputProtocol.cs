// SPDX-License-Identifier: MIT
//
// EFI_GRAPHICS_OUTPUT_PROTOCOL — framebuffer + Blt.
// Reference: MdePkg/Include/Protocol/GraphicsOutput.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

public enum EfiGraphicsPixelFormat : uint
{
    PixelRedGreenBlueReserved8BitPerColor = 0,
    PixelBlueGreenRedReserved8BitPerColor = 1,
    PixelBitMask                           = 2,
    PixelBltOnly                           = 3,
    PixelFormatMax                         = 4,
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiPixelBitmask
{
    public uint RedMask;
    public uint GreenMask;
    public uint BlueMask;
    public uint ReservedMask;
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiGraphicsOutputModeInformation
{
    public uint                   Version;
    public uint                   HorizontalResolution;
    public uint                   VerticalResolution;
    public EfiGraphicsPixelFormat PixelFormat;
    public EfiPixelBitmask        PixelInformation;
    public uint                   PixelsPerScanLine;
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiGraphicsOutputProtocolMode
{
    public uint  MaxMode;
    public uint  Mode;
    public EfiGraphicsOutputModeInformation* Info;
    public nuint SizeOfInfo;
    public EfiPhysicalAddress FrameBufferBase;
    public nuint FrameBufferSize;
}

[StructLayout(LayoutKind.Sequential, Size = 4)]
public struct EfiGraphicsOutputBltPixel
{
    public byte Blue;
    public byte Green;
    public byte Red;
    public byte Reserved;
}

public enum EfiGraphicsOutputBltOperation : uint
{
    VideoFill         = 0,
    VideoToBltBuffer  = 1,
    BufferToVideo     = 2,
    VideoToVideo      = 3,
    OperationMax      = 4,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiGraphicsOutputProtocol
{
    public delegate* unmanaged<EfiGraphicsOutputProtocol*, uint, nuint*, EfiGraphicsOutputModeInformation**, EfiStatus> QueryMode;
    public delegate* unmanaged<EfiGraphicsOutputProtocol*, uint, EfiStatus>                                              SetMode;
    public delegate* unmanaged<EfiGraphicsOutputProtocol*, EfiGraphicsOutputBltPixel*, EfiGraphicsOutputBltOperation,
                               nuint, nuint, nuint, nuint, nuint, nuint, nuint, EfiStatus>                               Blt;
    public EfiGraphicsOutputProtocolMode* Mode;
}
