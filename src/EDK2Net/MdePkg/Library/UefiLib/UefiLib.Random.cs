// SPDX-License-Identifier: MIT
//
// UefiLib.Random — wrappers for EFI_RNG_PROTOCOL (UEFI 2.4+).
//
// All helpers transparently locate the protocol on demand; if the firmware
// does not expose an RNG, calls return EFI_UNSUPPORTED (or 0 / false for the
// non-status overloads).
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Uefi;
using EDK2Net.MdePkg.Guid;
using EDK2Net.MdePkg.Protocol;

public static unsafe partial class UefiLib
{
    /// <summary>Locate the platform RNG protocol if available.</summary>
    public static EfiStatus GetRng(out EfiRngProtocol* rng)
    {
        EfiRngProtocol* p;
        var guid = EfiGuids.RngProtocol;
        var status = BootServices->LocateProtocol(&guid, null, (void**)&p);
        rng = status.IsError ? null : p;
        return status;
    }

    /// <summary>Fill <paramref name="buffer"/> with <paramref name="length"/>
    /// random bytes using the firmware's default algorithm.</summary>
    public static EfiStatus GetRandomBytes(byte* buffer, nuint length)
    {
        var status = GetRng(out var rng);
        if (status.IsError || rng == null) return status;
        return rng->GetRNG(rng, null, length, buffer);
    }

    /// <summary>Get a 64-bit random integer; returns 0 if RNG is unavailable.</summary>
    public static ulong GetRandomUInt64()
    {
        ulong value = 0;
        GetRandomBytes((byte*)&value, sizeof(ulong));
        return value;
    }

    /// <summary>Get a 32-bit random integer; returns 0 if RNG is unavailable.</summary>
    public static uint GetRandomUInt32()
    {
        uint value = 0;
        GetRandomBytes((byte*)&value, sizeof(uint));
        return value;
    }
}
