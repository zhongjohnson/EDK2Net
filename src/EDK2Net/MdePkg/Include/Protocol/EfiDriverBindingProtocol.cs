// SPDX-License-Identifier: MIT
//
// EFI_DRIVER_BINDING_PROTOCOL — the entry-point an EFI driver implements to
// declare which controllers it supports and how to start/stop them. Always
// installed by EFI driver modules in their EntryPoint.
//
// Reference: MdePkg/Include/Protocol/DriverBinding.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiDriverBindingProtocol
{
    public delegate* unmanaged<EfiDriverBindingProtocol*, EfiHandle, EfiDevicePathProtocol*, EfiStatus> Supported;
    public delegate* unmanaged<EfiDriverBindingProtocol*, EfiHandle, EfiDevicePathProtocol*, EfiStatus> Start;
    public delegate* unmanaged<EfiDriverBindingProtocol*, EfiHandle, nuint, EfiHandle*, EfiStatus>      Stop;

    public uint   Version;
    public EfiHandle ImageHandle;
    public EfiHandle DriverBindingHandle;
}
