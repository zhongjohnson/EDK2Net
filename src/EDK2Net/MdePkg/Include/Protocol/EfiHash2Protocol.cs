// SPDX-License-Identifier: MIT
//
// EFI_HASH2_PROTOCOL — UEFI 2.5+ message-digest interface (SHA-1, SHA-2 family,
// MD5). Variant of EFI_HASH_PROTOCOL that hides scatter-gather details and
// supports streaming via Init/Update/Final.
//
// Reference: MdePkg/Include/Protocol/Hash2.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

/// <summary>EFI_HASH_OUTPUT — fixed buffers sized for the largest supported
/// algorithm. Callers typically pin a digest-specific overlay.</summary>
[StructLayout(LayoutKind.Explicit)]
public unsafe struct EfiHashOutput
{
    [FieldOffset(0)] public fixed byte Md5Hash[16];
    [FieldOffset(0)] public fixed byte Sha1Hash[20];
    [FieldOffset(0)] public fixed byte Sha224Hash[28];
    [FieldOffset(0)] public fixed byte Sha256Hash[32];
    [FieldOffset(0)] public fixed byte Sha384Hash[48];
    [FieldOffset(0)] public fixed byte Sha512Hash[64];
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiHash2Protocol
{
    /// <summary>Get hash size (bytes) for the given algorithm GUID.</summary>
    public delegate* unmanaged<EfiHash2Protocol*, EfiGuid*, nuint*, EfiStatus> GetHashSize;

    /// <summary>One-shot hash of a single buffer.</summary>
    public delegate* unmanaged<EfiHash2Protocol*, EfiGuid*, byte*, nuint, EfiHashOutput*, EfiStatus> Hash;

    /// <summary>Begin a streaming hash with the given algorithm.</summary>
    public delegate* unmanaged<EfiHash2Protocol*, EfiGuid*, EfiStatus> HashInit;

    /// <summary>Append data to the in-progress hash.</summary>
    public delegate* unmanaged<EfiHash2Protocol*, byte*, nuint, EfiStatus> HashUpdate;

    /// <summary>Finalize the in-progress hash into the output buffer.</summary>
    public delegate* unmanaged<EfiHash2Protocol*, EfiHashOutput*, EfiStatus> HashFinal;
}

/// <summary>EFI_HASH2_SERVICE_BINDING_PROTOCOL — used to create per-instance
/// EFI_HASH2_PROTOCOL handles. Bind via the standard EFI Service Binding
/// pattern (CreateChild / DestroyChild).</summary>
[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiServiceBindingProtocol
{
    public delegate* unmanaged<EfiServiceBindingProtocol*, EfiHandle*, EfiStatus> CreateChild;
    public delegate* unmanaged<EfiServiceBindingProtocol*, EfiHandle, EfiStatus>  DestroyChild;
}

/// <summary>Standard EFI hash algorithm GUIDs (subset).</summary>
public static class EfiHashAlgorithm
{
    public static readonly EfiGuid Sha1   = new(0x2ae9d80f, 0x3fb2, 0x4095, 0xb7, 0xb1, 0xe9, 0x31, 0x57, 0xb9, 0x46, 0xb6);
    public static readonly EfiGuid Sha224 = new(0x8df01a06, 0x9bd5, 0x4bf7, 0xb0, 0x21, 0xdb, 0x4f, 0xd9, 0xcc, 0xf4, 0x5b);
    public static readonly EfiGuid Sha256 = new(0x51aa59de, 0xfdf2, 0x4ea3, 0xbc, 0x63, 0x87, 0x5f, 0xb7, 0x84, 0x2e, 0xe9);
    public static readonly EfiGuid Sha384 = new(0xefa96432, 0xde33, 0x4dd2, 0xae, 0xe6, 0x32, 0x8c, 0x33, 0xdf, 0x77, 0x7a);
    public static readonly EfiGuid Sha512 = new(0xcaa4381e, 0x750c, 0x4770, 0xb8, 0x70, 0x7a, 0x23, 0xb4, 0xe4, 0x21, 0x30);
    public static readonly EfiGuid Md5    = new(0xaf7c79c,  0x65b5, 0x4319, 0xb0, 0xae, 0x44, 0xec, 0x48, 0x4e, 0x4a, 0xd7);
}
