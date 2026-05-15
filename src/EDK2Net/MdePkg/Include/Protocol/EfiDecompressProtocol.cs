// SPDX-License-Identifier: MIT
//
// EFI_DECOMPRESS_PROTOCOL — UEFI EFI/Tiano decompression (used for compressed
// FFS sections, capsule payloads, etc.).
//
// Typical use:
//   GetInfo(src, srcSize, &dstSize, &scratchSize);
//   AllocatePool(dstSize, &dst);
//   AllocatePool(scratchSize, &scratch);
//   Decompress(src, srcSize, dst, dstSize, scratch, scratchSize);
//
// Reference: MdePkg/Include/Protocol/Decompress.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiDecompressProtocol
{
    /// <summary>Inspect the compressed stream to obtain destination and scratch
    /// buffer sizes required by <see cref="Decompress"/>.</summary>
    public delegate* unmanaged<EfiDecompressProtocol*, void*, uint, uint*, uint*, EfiStatus> GetInfo;

    /// <summary>Decompress <paramref name="source"/> into <paramref name="destination"/>.</summary>
    public delegate* unmanaged<EfiDecompressProtocol*, void*, uint, void*, uint, void*, uint, EfiStatus> Decompress;
}
