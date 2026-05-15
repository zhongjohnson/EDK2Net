// SPDX-License-Identifier: MIT
// Core EFI scalar types. Mirror EDK2 MdePkg/Include/Uefi/UefiBaseType.h.
namespace EDK2Net.MdePkg.Uefi;

using System;

/// <summary>
/// EFI_STATUS — 64-bit on x64 UEFI. The high bit indicates an error.
/// </summary>
public readonly struct EfiStatus : IEquatable<EfiStatus>
{
    public readonly nuint Value;

    public EfiStatus(nuint value) => Value = value;

    public bool IsError => (Value & HighBit) != 0;
    public bool IsSuccess => Value == 0;
    public bool IsWarning => Value != 0 && (Value & HighBit) == 0;

    private static nuint HighBit => unchecked((nuint)1 << (System.IntPtr.Size * 8 - 1));

    public static EfiStatus Encode(nuint code, bool error)
        => new(error ? (HighBit | code) : code);

    // --- Success ---------------------------------------------------------
    public static EfiStatus Success => new(0);

    // --- Errors (high bit set). Values from UefiBaseType.h --------------
    public static EfiStatus LoadError           => Encode(1, true);
    public static EfiStatus InvalidParameter    => Encode(2, true);
    public static EfiStatus Unsupported         => Encode(3, true);
    public static EfiStatus BadBufferSize       => Encode(4, true);
    public static EfiStatus BufferTooSmall      => Encode(5, true);
    public static EfiStatus NotReady            => Encode(6, true);
    public static EfiStatus DeviceError         => Encode(7, true);
    public static EfiStatus WriteProtected      => Encode(8, true);
    public static EfiStatus OutOfResources      => Encode(9, true);
    public static EfiStatus VolumeCorrupted     => Encode(10, true);
    public static EfiStatus VolumeFull          => Encode(11, true);
    public static EfiStatus NoMedia             => Encode(12, true);
    public static EfiStatus MediaChanged        => Encode(13, true);
    public static EfiStatus NotFound            => Encode(14, true);
    public static EfiStatus AccessDenied        => Encode(15, true);
    public static EfiStatus NoResponse          => Encode(16, true);
    public static EfiStatus NoMapping           => Encode(17, true);
    public static EfiStatus Timeout             => Encode(18, true);
    public static EfiStatus NotStarted          => Encode(19, true);
    public static EfiStatus AlreadyStarted      => Encode(20, true);
    public static EfiStatus Aborted             => Encode(21, true);
    public static EfiStatus IcmpError           => Encode(22, true);
    public static EfiStatus TftpError           => Encode(23, true);
    public static EfiStatus ProtocolError       => Encode(24, true);
    public static EfiStatus IncompatibleVersion => Encode(25, true);
    public static EfiStatus SecurityViolation   => Encode(26, true);
    public static EfiStatus CrcError            => Encode(27, true);
    public static EfiStatus EndOfMedia          => Encode(28, true);
    public static EfiStatus EndOfFile           => Encode(31, true);
    public static EfiStatus InvalidLanguage     => Encode(32, true);
    public static EfiStatus CompromisedData     => Encode(33, true);

    public bool Equals(EfiStatus other) => Value == other.Value;
    public override bool Equals(object? obj) => obj is EfiStatus s && Equals(s);
    public override int GetHashCode() => (int)Value;
    public static bool operator ==(EfiStatus a, EfiStatus b) => a.Value == b.Value;
    public static bool operator !=(EfiStatus a, EfiStatus b) => a.Value != b.Value;

    public static implicit operator nuint(EfiStatus s) => s.Value;
    public static implicit operator EfiStatus(nuint v) => new(v);
}
