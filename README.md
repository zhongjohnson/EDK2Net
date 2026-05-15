# EDK2Net

C# bindings & build infrastructure for writing **UEFI applications in C#**, modeled on the [edk2-cs](https://github.com/) approach.

EDK2Net turns the .NET NativeAOT toolchain into a UEFI compiler: your C# program is published as a native PE32+ image with `Subsystem = EFI_APPLICATION`, calling the firmware-provided EFI services directly through function-pointer bindings — no managed runtime, no GC interactions in your hot path, no marshaling.

## Repository layout

```
EDK2Net/
├── build/
│   ├── Uefi.props          # shared NativeAOT + UEFI linker settings
│   └── run-qemu.ps1        # boot a built .efi in QEMU + OVMF
├── src/
│   └── EDK2Net/            # bindings library
│       ├── Efi/            # core types & service tables
│       │   ├── EfiStatus.cs
│       │   ├── EfiGuid.cs
│       │   ├── EfiHandle.cs
│       │   ├── EfiTableHeader.cs
│       │   ├── Char16.cs
│       │   ├── EfiSystemTable.cs
│       │   ├── EfiBootServices.cs
│       │   ├── EfiRuntimeServices.cs
│       │   └── Protocols/
│       │       ├── EfiSimpleTextOutputProtocol.cs
│       │       └── EfiSimpleTextInputProtocol.cs
│       └── Runtime/
│           └── RuntimeStubs.cs   # memcpy/memset/__chkstk/...
└── samples/
    └── HelloUefi/          # minimal sample app
        ├── HelloUefi.csproj
        └── Program.cs
```

## Prerequisites

| Tool                          | Why                                              |
|-------------------------------|--------------------------------------------------|
| **.NET 9 SDK**                | NativeAOT publish                                |
| **Visual Studio Build Tools** | `link.exe` (MSVC linker) for the AOT publish     |
| **QEMU**                      | `qemu-system-x86_64` to run the .efi             |
| **OVMF firmware**             | `OVMF.fd` — provides UEFI inside QEMU            |

On Windows, install the “Desktop development with C++” workload to get `link.exe`. NativeAOT will invoke it from the MSBuild publish step.

## Build the sample

```powershell
dotnet publish .\samples\HelloUefi\HelloUefi.csproj -c Release -r win-x64
```

The result is renamed to `HelloUefi.efi` automatically by `build/Uefi.props`:

```
samples\HelloUefi\bin\Release\net9.0\win-x64\publish\HelloUefi.efi
```

## Run under QEMU

```powershell
$env:OVMF_FD = "C:\path\to\OVMF.fd"
.\build\run-qemu.ps1 -Image .\samples\HelloUefi\bin\Release\net9.0\win-x64\publish\HelloUefi.efi
```

You should see `Hello, UEFI from C#!` on the UEFI console, then the app waits for a keystroke.

## How it works

1. `build/Uefi.props` configures NativeAOT to strip everything UEFI doesn't have (globalization, reflection, exception messages, stack traces) and tells the Microsoft linker:
   * `/SUBSYSTEM:EFI_APPLICATION` — PE subsystem 10 (UEFI)
   * `/ENTRY:EfiMain`             — bypass the CRT, jump straight to your C# entry
   * `/NODEFAULTLIB`              — no CRT linkage
2. Your entry point is decorated `[UnmanagedCallersOnly(EntryPoint = "EfiMain")]` and matches `EFI_IMAGE_ENTRY_POINT`:
   ```csharp
   public static nuint EfiMain(EfiHandle imageHandle, EfiSystemTable* systemTable)
   ```
3. EFI services are exposed as plain C structs of function pointers (`delegate* unmanaged<...>`), so calling `systemTable->ConOut->OutputString(...)` from C# produces the exact same x64 calling-convention sequence as C/EDK2.
4. `RuntimeStubs.cs` provides `memcpy` / `memset` / `__chkstk` etc., which the compiler may emit even with `/NODEFAULTLIB`.

## Extending the bindings

To add a new UEFI protocol:

1. Find its definition in EDK2 (`MdePkg/Include/Protocol/...h`).
2. Translate the struct to C# under `src/EDK2Net/Efi/Protocols/`, using `delegate* unmanaged<...>` for every function-pointer member. Preserve field order and offsets exactly.
3. Add the protocol's GUID as a `public static readonly EfiGuid` constant.
4. Locate the protocol at runtime with `BootServices->LocateProtocol`.

## Status & roadmap

- [x] Core types (`EfiStatus`, `EfiGuid`, `EfiHandle`, `EfiTableHeader`, `Char16`)
- [x] `SystemTable` / `BootServices` / `RuntimeServices`
- [x] `SimpleTextOutput` / `SimpleTextInput`
- [ ] `LoadedImage`, `DevicePath`, `SimpleFileSystem`, `File` protocols
- [ ] `GraphicsOutput` (GOP)
- [ ] AArch64 (`/MACHINE:ARM64`) configuration

## License

MIT. See [LICENSE](LICENSE).
