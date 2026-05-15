// SPDX-License-Identifier: MIT
//
// UefiLib.Timestamp — wrappers for EFI_TIMESTAMP_PROTOCOL (UEFI 2.5+),
// the firmware-exposed monotonic counter most useful for measuring short
// intervals at high resolution.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Uefi;
using EDK2Net.MdePkg.Guid;
using EDK2Net.MdePkg.Protocol;

public static unsafe partial class UefiLib
{
    /// <summary>Locate the platform timestamp protocol if present.</summary>
    public static EfiStatus GetTimestampProtocol(out EfiTimestampProtocol* protocol)
    {
        EfiTimestampProtocol* p;
        var guid = EfiGuids.TimestampProtocol;
        var status = BootServices->LocateProtocol(&guid, null, (void**)&p);
        protocol = status.IsError ? null : p;
        return status;
    }

    /// <summary>Read the current free-running counter value, or 0 on failure.</summary>
    public static ulong GetTimestamp()
    {
        if (GetTimestampProtocol(out var p).IsError || p == null)
            return 0;
        return p->GetTimestamp();
    }

    /// <summary>Read the timestamp counter properties (frequency / wrap value).</summary>
    public static EfiStatus GetTimestampProperties(out EfiTimestampProperties properties)
    {
        properties = default;
        var status = GetTimestampProtocol(out var p);
        if (status.IsError || p == null) return status;
        EfiTimestampProperties props;
        status = p->GetProperties(&props);
        if (!status.IsError) properties = props;
        return status;
    }

    /// <summary>Convert two timestamp samples into elapsed nanoseconds, accounting
    /// for the 64-bit wrap reported by <see cref="EfiTimestampProperties.EndValue"/>.
    /// Returns 0 if the protocol is unavailable.</summary>
    public static ulong TimestampDeltaNanoseconds(ulong start, ulong end)
    {
        if (GetTimestampProperties(out var props).IsError || props.Frequency == 0)
            return 0;

        ulong ticks = end >= start
            ? end - start
            : (props.EndValue - start) + end + 1;

        // ns = ticks * 1e9 / freq, computed to avoid overflow for typical freqs.
        const ulong NsPerSec = 1_000_000_000UL;
        ulong whole = ticks / props.Frequency * NsPerSec;
        ulong rem   = ticks % props.Frequency * NsPerSec / props.Frequency;
        return whole + rem;
    }
}
