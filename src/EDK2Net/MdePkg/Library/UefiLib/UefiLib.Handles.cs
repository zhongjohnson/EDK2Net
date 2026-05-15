// SPDX-License-Identifier: MIT
//
// UefiLib.Handles — handle/protocol enumeration helpers.
//
// Wraps the most common BootServices discovery calls:
//   * LocateHandleBuffer (filtered or all)
//   * ProtocolsPerHandle (which protocols a handle exposes)
//   * OpenProtocol / CloseProtocol (driver-aware open with attributes)
//
// All buffers returned by firmware are allocated via BootServices pool and
// must be released by the caller with FreePool.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>Find every handle that exposes <paramref name="protocol"/>.
    /// On success, <paramref name="handles"/> points to a pool-allocated
    /// array of <paramref name="count"/> entries that the caller must
    /// release via <see cref="FreePool"/>.</summary>
    public static EfiStatus LocateHandleBuffer(
        EfiGuid protocol,
        out EfiHandle* handles,
        out nuint count)
    {
        handles = null;
        count = 0;
        EfiHandle* buf;
        nuint c;
        var s = BootServices->LocateHandleBuffer(
            EfiLocateSearchType.ByProtocol, &protocol, null, &c, &buf);
        if (s.IsError) return s;
        handles = buf;
        count = c;
        return s;
    }

    /// <summary>Return every handle in the system. Caller releases the buffer
    /// via <see cref="FreePool"/>.</summary>
    public static EfiStatus LocateAllHandles(out EfiHandle* handles, out nuint count)
    {
        handles = null;
        count = 0;
        EfiHandle* buf;
        nuint c;
        var s = BootServices->LocateHandleBuffer(
            EfiLocateSearchType.AllHandles, null, null, &c, &buf);
        if (s.IsError) return s;
        handles = buf;
        count = c;
        return s;
    }

    /// <summary>Enumerate the protocol GUIDs installed on a handle. The
    /// returned array of <see cref="EfiGuid"/>* pointers is pool-allocated
    /// and must be released by the caller with <see cref="FreePool"/>.</summary>
    public static EfiStatus ProtocolsPerHandle(
        EfiHandle handle,
        out EfiGuid** protocolBuffer,
        out nuint count)
    {
        protocolBuffer = null;
        count = 0;
        EfiGuid** buf;
        nuint c;
        var s = BootServices->ProtocolsPerHandle(handle, &buf, &c);
        if (s.IsError) return s;
        protocolBuffer = buf;
        count = c;
        return s;
    }

    /// <summary>Open a protocol on a handle for the current image
    /// (BY_HANDLE_PROTOCOL semantics by default).</summary>
    public static EfiStatus OpenProtocol(
        EfiHandle handle,
        EfiGuid protocol,
        out void* iface,
        uint attributes = 0x00000001 /* BY_HANDLE_PROTOCOL */)
    {
        void* p;
        var s = BootServices->OpenProtocol(
            handle, &protocol, &p, ImageHandle, default, attributes);
        iface = s.IsError ? null : p;
        return s;
    }

    /// <summary>Close a previously-opened protocol for the current image.</summary>
    public static EfiStatus CloseProtocol(EfiHandle handle, EfiGuid protocol)
        => BootServices->CloseProtocol(handle, &protocol, ImageHandle, default);
}
