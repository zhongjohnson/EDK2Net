// SPDX-License-Identifier: MIT
//
// Global usings for the EDK2Net assembly.
//
// The source layout mirrors EDK2 packages (MdePkg/, ShellPkg/, ...), and the
// individual files are organized into matching namespaces (EDK2Net.MdePkg.Uefi,
// EDK2Net.MdePkg.Protocol, etc.). Because most types reference each other
// across those namespaces, we expose them globally inside the assembly so
// individual files don't need a wall of `using` directives at the top.
global using EDK2Net.MdePkg.Uefi;
global using EDK2Net.MdePkg.Guid;
global using EDK2Net.MdePkg.Protocol;
global using EDK2Net.MdePkg.IndustryStandard;
global using EDK2Net.ShellPkg.Guid;
global using EDK2Net.ShellPkg.Protocol;
