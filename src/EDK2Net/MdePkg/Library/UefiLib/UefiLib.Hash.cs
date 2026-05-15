// SPDX-License-Identifier: MIT
//
// UefiLib.Hash — convenience around EFI_HASH2_PROTOCOL.
//
// EFI_HASH2 is a "service binding" protocol: callers must first locate the
// EFI_HASH2_SERVICE_BINDING_PROTOCOL on a handle, call CreateChild to get a
// per-instance handle, then HandleProtocol(EFI_HASH2_PROTOCOL_GUID) on that
// child. This helper hides the boilerplate and gives a one-shot Hash() and
// matching Sha256/Sha1/Md5 helpers.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Uefi;
using EDK2Net.MdePkg.Guid;
using EDK2Net.MdePkg.Protocol;

public static unsafe partial class UefiLib
{
    /// <summary>Locate a Hash2 service-binding handle, create a child instance,
    /// and return both the protocol pointer and the child handle so the caller
    /// can release it via <see cref="DestroyHash2Child"/>.</summary>
    private static EfiStatus LocateFirstServiceBinding(EfiGuid sbGuid, out EfiServiceBindingProtocol* sb)
    {
        sb = null;
        EfiHandle* handles = null;
        nuint count = 0;
        var s = BootServices->LocateHandleBuffer(EfiLocateSearchType.ByProtocol, &sbGuid, null, &count, &handles);
        if (s.IsError) return s;
        try
        {
            if (count == 0) return EfiStatus.NotFound;
            EfiServiceBindingProtocol* p;
            s = BootServices->HandleProtocol(handles[0], &sbGuid, (void**)&p);
            if (!s.IsError) sb = p;
            return s;
        }
        finally
        {
            if (handles != null) BootServices->FreePool(handles);
        }
    }

    /// <summary>Locate Hash2 service-binding, create a child instance, and
    /// return both the protocol pointer and the child handle.</summary>
    public static EfiStatus CreateHash2Child(out EfiHash2Protocol* hash, out EfiHandle childHandle)
    {
        hash = null;
        childHandle = default;

        var s = LocateFirstServiceBinding(EfiGuids.Hash2ServiceBinding, out var sb);
        if (s.IsError) return s;

        EfiHandle child = default;
        s = sb->CreateChild(sb, &child);
        if (s.IsError) return s;

        EfiHash2Protocol* h;
        var hGuid = EfiGuids.Hash2Protocol;
        s = BootServices->HandleProtocol(child, &hGuid, (void**)&h);
        if (s.IsError)
        {
            sb->DestroyChild(sb, child);
            return s;
        }

        hash = h;
        childHandle = child;
        return EfiStatus.Success;
    }

    /// <summary>Release a child handle previously obtained from <see cref="CreateHash2Child"/>.</summary>
    public static EfiStatus DestroyHash2Child(EfiHandle childHandle)
    {
        var s = LocateFirstServiceBinding(EfiGuids.Hash2ServiceBinding, out var sb);
        if (s.IsError) return s;
        return sb->DestroyChild(sb, childHandle);
    }

    /// <summary>One-shot hash of <paramref name="data"/>. Allocates and releases
    /// a Hash2 child internally. <paramref name="output"/> must point to a
    /// buffer large enough for the algorithm's digest.</summary>
    public static EfiStatus Hash(EfiGuid algorithm, byte* data, nuint length, EfiHashOutput* output)
    {
        var s = CreateHash2Child(out var hash, out var child);
        if (s.IsError) return s;
        try
        {
            return hash->Hash(hash, &algorithm, data, length, output);
        }
        finally
        {
            DestroyHash2Child(child);
        }
    }

    /// <summary>SHA-256 convenience. <paramref name="digest"/> must be at least 32 bytes.</summary>
    public static EfiStatus Sha256(byte* data, nuint length, byte* digest)
    {
        EfiHashOutput out_;
        var s = Hash(EfiHashAlgorithm.Sha256, data, length, &out_);
        if (s.IsError) return s;
        for (int i = 0; i < 32; i++) digest[i] = out_.Sha256Hash[i];
        return s;
    }
}
