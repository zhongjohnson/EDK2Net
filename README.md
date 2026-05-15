# EDK2Net

C# bindings & build infrastructure for writing **UEFI applications in C#**.

EDK2Net turns the .NET NativeAOT toolchain into a UEFI compiler: your C# program is published as a native PE32+ image with `Subsystem = EFI_APPLICATION`, calling the firmware-provided EFI services directly through function-pointer bindings — no managed runtime, no GC interactions in your hot path, no marshaling.

## Repository layout

The source tree mirrors the EDK2 package convention. Each top-level directory
maps onto an EDK2 package; everything still compiles into the single
`EDK2Net` assembly via `Compile Include` globs in `src/EDK2Net/EDK2Net.csproj`.

```
EDK2Net/
├── build/
│   ├── Uefi.props                              # shared NativeAOT + UEFI linker settings
│   └── run-qemu.ps1                            # boot a built .efi in QEMU + OVMF
├── MdePkg/                                     # MdePkg — core UEFI/PI definitions
│   ├── Include/
│   │   ├── Uefi/                               # EfiStatus, EfiGuid, EfiHandle, EfiTableHeader,
│   │   │                                       # Char16, EfiBaseTypes, EfiSystemTable,
│   │   │                                       # EfiBootServices, EfiRuntimeServices,
│   │   │                                       # EfiConfigurationTable
│   │   ├── Protocol/                           # SimpleTextInput/Output, LoadedImage,
│   │   │                                       # DevicePath, SimpleFileSystem (+ File),
│   │   │                                       # GraphicsOutput, BlockIo, DiskIo
│   │   ├── Guid/                               # EfiGuids — well-known MdePkg GUIDs
│   │   └── IndustryStandard/                   # Acpi (RSDP/XSDT/FADT/MADT), SmBios (2.x/3.x)
│   └── Library/
│       └── UefiLib/                            # UefiLib.cs — ergonomic helpers
├── ShellPkg/                                   # ShellPkg — UEFI Shell protocols & GUIDs
│   └── Include/
│       ├── Protocol/                           # EfiShellParametersProtocol
│       └── Guid/                               # ShellGuids (Shell, ShellParameters, ...)
├── src/
│   └── EDK2Net/                                # the .NET project that aggregates everything
│       ├── EDK2Net.csproj                      # Compile Include="..\..\MdePkg\..." etc.
│       ├── GlobalUsings.cs                     # global usings for MdePkg / ShellPkg namespaces
│       └── Runtime/
│           └── RuntimeStubs.cs                 # memcpy / memset / __chkstk / ...
└── samples/
    ├── HelloUefi/                              # minimal “Hello, UEFI” app
    └── ListFiles/                              # enumerates the volume this image was loaded from
```

### Namespace map

| Directory                              | Namespace                              |
|----------------------------------------|----------------------------------------|
| `MdePkg/Include/Uefi`                  | `EDK2Net.MdePkg.Uefi`                  |
| `MdePkg/Include/Protocol`              | `EDK2Net.MdePkg.Protocol`              |
| `MdePkg/Include/Guid`                  | `EDK2Net.MdePkg.Guid`                  |
| `MdePkg/Include/IndustryStandard`      | `EDK2Net.MdePkg.IndustryStandard`      |
| `MdePkg/Library/UefiLib`               | `EDK2Net.MdePkg.Library.UefiLib`       |
| `ShellPkg/Include/Protocol`            | `EDK2Net.ShellPkg.Protocol`            |
| `ShellPkg/Include/Guid`                | `EDK2Net.ShellPkg.Guid`                |
| `src/EDK2Net/Runtime`                  | `EDK2Net.Runtime`                      |

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
2. Translate the struct to C# under `MdePkg/Include/Protocol/` (or `ShellPkg/Include/Protocol/`
   for shell protocols), using `delegate* unmanaged<...>` for every function-pointer
   member. Preserve field order and offsets exactly. The file's namespace must
   match its directory (e.g. `namespace EDK2Net.MdePkg.Protocol;`).
3. Add the protocol's GUID as a `public static readonly EfiGuid` constant in
   `MdePkg/Include/Guid/EfiGuids.cs` (or `ShellPkg/Include/Guid/ShellGuids.cs`).
4. Locate the protocol at runtime with `BootServices->LocateProtocol`.

## Samples

| Sample      | Demonstrates                                                  |
|-------------|---------------------------------------------------------------|
| `HelloUefi` | `EfiMain`, `ConOut`, key wait                                 |
| `ListFiles` | `LoadedImage` → `SimpleFileSystem` → `OpenVolume` → `Read`   |

Build any sample with the same command, just changing the project path:

```powershell
dotnet publish .\samples\ListFiles\ListFiles.csproj -c Release -r win-x64
.\build\run-qemu.ps1 -Image .\samples\ListFiles\bin\Release\net9.0\win-x64\publish\ListFiles.efi
```

## Status & roadmap

- [x] Core types (`EfiStatus`, `EfiGuid`, `EfiHandle`, `EfiTableHeader`, `Char16`)
- [x] `SystemTable` / `BootServices` / `RuntimeServices` / `ConfigurationTable`
- [x] Console: `SimpleTextOutput`, `SimpleTextInput`
- [x] Image: `LoadedImage`, `DevicePath` (header + walk)
- [x] Storage: `SimpleFileSystem`, `FileProtocol`, `FileInfo`, `BlockIo`, `DiskIo`
- [x] Graphics: `GraphicsOutput` (GOP)
- [x] Industry standards: ACPI (RSDP/XSDT/RSDT/FADT/MADT), SMBIOS 2.x + 3.x
- [x] Shell: `EfiShellParametersProtocol` + GUIDs (read `argv` from UEFI Shell)
- [x] Helper layer: `UefiLib` (Print, LocateProtocol, GetConfigurationTable, ...)
- [ ] `EfiShellProtocol` deep binding
- [ ] Network stack (`SimpleNetwork`, `Tcp4/6`, `Http`)
- [ ] HII (Human Interface Infrastructure)
- [ ] AArch64 (`/MACHINE:ARM64`) configuration

## License

MIT. See [LICENSE](LICENSE).
