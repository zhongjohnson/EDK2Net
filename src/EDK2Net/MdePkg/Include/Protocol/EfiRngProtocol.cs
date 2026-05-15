// SPDX-License-Identifier: MIT
//
// EFI_RNG_PROTOCOL — UEFI 2.4+ Random Number Generator.
// Reference: MdePkg/Include/Protocol/Rng.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiRngProtocol
{
    /// <summary>GetInfo(RNGAlgorithmListSize, RNGAlgorithmList) — enumerate algorithms.</summary>
    public delegate* unmanaged<EfiRngProtocol*, nuint*, EfiGuid*, EfiStatus> GetInfo;

    /// <summary>GetRNG(RNGAlgorithm, RNGValueLength, RNGValue) — fill bytes.</summary>
    public delegate* unmanaged<EfiRngProtocol*, EfiGuid*, nuint, byte*, EfiStatus> GetRNG;
}

/// <summary>Standard EFI_RNG_ALGORITHM_* GUIDs from the UEFI spec.</summary>
public static class EfiRngAlgorithm
{
    public static readonly EfiGuid Sp80090Hash256 =
        new(0xa7af67cb, 0x603b, 0x4d42, 0xba, 0x21, 0x70, 0xbf, 0xb6, 0x29, 0x3f, 0x96);

    public static readonly EfiGuid Sp80090Hmac256 =
        new(0xc5149b43, 0xae85, 0x4f53, 0x99, 0x82, 0xb9, 0x43, 0x35, 0xd3, 0xa9, 0xe7);

    public static readonly EfiGuid Sp80090Ctr256 =
        new(0x44f0de6e, 0x4d8c, 0x4045, 0xa8, 0xc7, 0x4d, 0xd1, 0x68, 0x85, 0x6b, 0x9e);

    public static readonly EfiGuid X9_31_3DES =
        new(0x63c4785a, 0xca34, 0x4012, 0xa3, 0xc8, 0x0b, 0x6a, 0x32, 0x4f, 0x55, 0x46);

    public static readonly EfiGuid X9_31_AES =
        new(0xacd03321, 0x777e, 0x4d3d, 0xb1, 0xc8, 0x20, 0xcf, 0xd8, 0x88, 0x20, 0xc9);

    public static readonly EfiGuid Raw =
        new(0xe43176d7, 0xb6e8, 0x4827, 0xb7, 0x84, 0x7f, 0xfd, 0xc4, 0xb6, 0x85, 0x61);
}
