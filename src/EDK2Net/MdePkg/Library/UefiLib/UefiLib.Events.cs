// SPDX-License-Identifier: MIT
//
// UefiLib — Event & Timer convenience.
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>Create a plain (non-notifying) event.</summary>
    public static EfiStatus CreateEvent(uint type, nuint notifyTpl, out EfiEvent evt)
    {
        EfiEvent e;
        var s = BootServices->CreateEvent(type, notifyTpl, null, null, &e);
        evt = e;
        return s;
    }

    /// <summary>Create a timer event (TPL_CALLBACK).</summary>
    public static EfiStatus CreateTimerEvent(out EfiEvent evt)
        => CreateEvent(EfiEventType.Timer, EfiTplLevel.Callback, out evt);

    /// <summary>
    /// Arm a timer event. <paramref name="hundredNanoseconds"/> uses UEFI's
    /// 100-ns ticks (e.g. 10_000_000 = 1 second).
    /// </summary>
    public static EfiStatus SetTimer(EfiEvent evt, uint type, ulong hundredNanoseconds)
        => BootServices->SetTimer(evt, type, hundredNanoseconds);

    public static EfiStatus CloseEvent(EfiEvent evt) => BootServices->CloseEvent(evt);
    public static EfiStatus SignalEvent(EfiEvent evt) => BootServices->SignalEvent(evt);
    public static EfiStatus CheckEvent(EfiEvent evt) => BootServices->CheckEvent(evt);

    /// <summary>WaitForEvent on a single event — returns when it is signaled.</summary>
    public static EfiStatus WaitForEvent(EfiEvent evt)
    {
        var e = evt;
        nuint index;
        return BootServices->WaitForEvent(1, &e, &index);
    }

    /// <summary>
    /// Sleep for <paramref name="milliseconds"/>. Backed by Stall (busy wait).
    /// Use a timer event + WaitForEvent if you need to sleep at TPL_CALLBACK
    /// or longer than ~1 second.
    /// </summary>
    public static EfiStatus SleepMs(uint milliseconds)
        => BootServices->Stall((nuint)milliseconds * 1000);

    /// <summary>Convert milliseconds to UEFI 100 ns ticks (for SetTimer).</summary>
    public static ulong MillisecondsToTicks(ulong milliseconds)
        => milliseconds * 10_000UL;

    /// <summary>Convert microseconds to UEFI 100 ns ticks.</summary>
    public static ulong MicrosecondsToTicks(ulong microseconds)
        => microseconds * 10UL;
}
