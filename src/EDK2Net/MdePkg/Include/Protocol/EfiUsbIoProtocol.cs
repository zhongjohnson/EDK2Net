// SPDX-License-Identifier: MIT
//
// EFI_USB_IO_PROTOCOL — UEFI access to a USB function/interface. Bound by
// the USB bus driver onto every discovered endpoint.
//
// Reference: MdePkg/Include/Protocol/UsbIo.h.
namespace EDK2Net.MdePkg.Protocol;

using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential)]
public struct EfiUsbDeviceDescriptor
{
    public byte   Length;
    public byte   DescriptorType;
    public ushort BcdUSB;
    public byte   DeviceClass;
    public byte   DeviceSubClass;
    public byte   DeviceProtocol;
    public byte   MaxPacketSize0;
    public ushort IdVendor;
    public ushort IdProduct;
    public ushort BcdDevice;
    public byte   StrManufacturer;
    public byte   StrProduct;
    public byte   StrSerialNumber;
    public byte   NumConfigurations;
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiUsbConfigDescriptor
{
    public byte   Length;
    public byte   DescriptorType;
    public ushort TotalLength;
    public byte   NumInterfaces;
    public byte   ConfigurationValue;
    public byte   Configuration;
    public byte   Attributes;
    public byte   MaxPower;
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiUsbInterfaceDescriptor
{
    public byte Length;
    public byte DescriptorType;
    public byte InterfaceNumber;
    public byte AlternateSetting;
    public byte NumEndpoints;
    public byte InterfaceClass;
    public byte InterfaceSubClass;
    public byte InterfaceProtocol;
    public byte Interface;
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiUsbEndpointDescriptor
{
    public byte   Length;
    public byte   DescriptorType;
    public byte   EndpointAddress;
    public byte   Attributes;
    public ushort MaxPacketSize;
    public byte   Interval;
}

[StructLayout(LayoutKind.Sequential)]
public struct EfiUsbDeviceRequest
{
    public byte   RequestType;
    public byte   Request;
    public ushort Value;
    public ushort Index;
    public ushort Length;
}

public enum EfiUsbDataDirection : uint
{
    DataIn       = 0,
    DataOut      = 1,
    NoData       = 2,
}

[StructLayout(LayoutKind.Sequential)]
public unsafe struct EfiUsbIoProtocol
{
    public delegate* unmanaged<EfiUsbIoProtocol*, EfiUsbDeviceRequest*, EfiUsbDataDirection, uint, void*, nuint, uint*, EfiStatus> ControlTransfer;
    public delegate* unmanaged<EfiUsbIoProtocol*, byte, void*, nuint*, nuint, uint*, EfiStatus> BulkTransfer;
    public delegate* unmanaged<EfiUsbIoProtocol*, byte, byte, nuint, void*, nuint, void*, void*, uint*, EfiStatus> AsyncInterruptTransfer;
    public delegate* unmanaged<EfiUsbIoProtocol*, byte, void*, nuint*, nuint, uint*, EfiStatus> SyncInterruptTransfer;
    public delegate* unmanaged<EfiUsbIoProtocol*, byte, void*, nuint, uint*, EfiStatus> IsochronousTransfer;
    public delegate* unmanaged<EfiUsbIoProtocol*, byte, void*, nuint, void*, void*, uint*, EfiStatus> AsyncIsochronousTransfer;

    public delegate* unmanaged<EfiUsbIoProtocol*, EfiUsbDeviceDescriptor*, EfiStatus>      GetDeviceDescriptor;
    public delegate* unmanaged<EfiUsbIoProtocol*, EfiUsbConfigDescriptor*, EfiStatus>      GetActiveConfigDescriptor;
    public delegate* unmanaged<EfiUsbIoProtocol*, EfiUsbInterfaceDescriptor*, EfiStatus>   GetInterfaceDescriptor;
    public delegate* unmanaged<EfiUsbIoProtocol*, byte, EfiUsbEndpointDescriptor*, EfiStatus> GetEndpointDescriptor;

    public delegate* unmanaged<EfiUsbIoProtocol*, ushort, byte, Char16**, EfiStatus>       GetStringDescriptor;
    public delegate* unmanaged<EfiUsbIoProtocol*, ushort**, EfiStatus>                     GetSupportedLanguages;

    public delegate* unmanaged<EfiUsbIoProtocol*, EfiStatus> PortReset;
}

/// <summary>Standard USB request-type bits (bmRequestType).</summary>
public static class UsbRequestType
{
    public const byte HostToDevice = 0x00;
    public const byte DeviceToHost = 0x80;

    public const byte TypeStandard = 0x00;
    public const byte TypeClass    = 0x20;
    public const byte TypeVendor   = 0x40;

    public const byte RecipientDevice    = 0x00;
    public const byte RecipientInterface = 0x01;
    public const byte RecipientEndpoint  = 0x02;
    public const byte RecipientOther     = 0x03;
}

/// <summary>Standard USB device-class codes (bDeviceClass / bInterfaceClass).</summary>
public static class UsbClassCode
{
    public const byte PerInterface  = 0x00;
    public const byte Audio         = 0x01;
    public const byte Communication = 0x02;
    public const byte Hid           = 0x03;
    public const byte MassStorage   = 0x08;
    public const byte Hub           = 0x09;
    public const byte Cdc           = 0x0A;
    public const byte Video         = 0x0E;
    public const byte Wireless      = 0xE0;
    public const byte Vendor        = 0xFF;
}
