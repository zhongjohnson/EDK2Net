// SPDX-License-Identifier: MIT
//
// UefiLib — File-system convenience.
//
// Provides one-shot helpers for the common UEFI file-IO recipe:
//     LoadedImage -> SimpleFileSystem -> OpenVolume -> Open -> Read -> Close
namespace EDK2Net.MdePkg.Library.UefiLib;

using EDK2Net.MdePkg.Guid;
using EDK2Net.MdePkg.Protocol;
using EDK2Net.MdePkg.Uefi;

public static unsafe partial class UefiLib
{
    /// <summary>
    /// Open the root directory of the volume that this image was loaded from.
    /// Caller is responsible for <c>root->Close(root)</c>.
    /// </summary>
    public static EfiStatus OpenImageRoot(out EfiFileProtocol* root)
    {
        root = null;

        EfiLoadedImageProtocol* li;
        var s = HandleProtocol(s_imageHandle, EfiGuids.LoadedImageProtocol, (void**)&li);
        if (s.IsError) return s;

        EfiSimpleFileSystemProtocol* fs;
        s = HandleProtocol(li->DeviceHandle, EfiGuids.SimpleFileSystemProtocol, (void**)&fs);
        if (s.IsError) return s;

        EfiFileProtocol* r;
        s = fs->OpenVolume(fs, &r);
        if (s.IsError) return s;

        root = r;
        return EfiStatus.Success;
    }

    /// <summary>Open a child entry under <paramref name="parent"/>.</summary>
    public static EfiStatus OpenFile(
        EfiFileProtocol* parent,
        string name,
        ulong mode,
        ulong attributes,
        out EfiFileProtocol* file)
    {
        EfiFileProtocol* f;
        fixed (char* nm = name)
        {
            var s = parent->Open(parent, &f, (Char16*)nm, mode, attributes);
            file = s.IsSuccess ? f : null;
            return s;
        }
    }

    /// <summary>Open an existing file for reading.</summary>
    public static EfiStatus OpenFileRead(EfiFileProtocol* parent, string name, out EfiFileProtocol* file)
        => OpenFile(parent, name, EfiFileMode.Read, 0, out file);

    /// <summary>Create or truncate a file for read/write.</summary>
    public static EfiStatus CreateFile(EfiFileProtocol* parent, string name, out EfiFileProtocol* file)
        => OpenFile(parent, name,
                    EfiFileMode.Read | EfiFileMode.Write | EfiFileMode.Create,
                    0, out file);

    public static EfiStatus CloseFile(EfiFileProtocol* file)
        => file == null ? EfiStatus.Success : file->Close(file);

    /// <summary>
    /// Read up to <paramref name="bufferSize"/> bytes; updates
    /// <paramref name="bytesRead"/> with what was actually read.
    /// </summary>
    public static EfiStatus ReadFile(EfiFileProtocol* file, void* buffer, ref nuint bytesRead, nuint bufferSize)
    {
        nuint n = bufferSize;
        var s = file->Read(file, &n, buffer);
        bytesRead = n;
        return s;
    }

    public static EfiStatus WriteFile(EfiFileProtocol* file, void* buffer, ref nuint bytesToWrite)
    {
        nuint n = bytesToWrite;
        var s = file->Write(file, &n, buffer);
        bytesToWrite = n;
        return s;
    }

    /// <summary>
    /// Read the whole file into a freshly pool-allocated buffer. Caller frees
    /// with <see cref="FreePool"/>.
    /// </summary>
    public static EfiStatus ReadAllBytes(EfiFileProtocol* file, out void* buffer, out nuint size)
    {
        buffer = null; size = 0;

        // Query EFI_FILE_INFO to get the size.
        var infoGuid = EfiGuids.FileInfo;
        nuint infoSize = (nuint)sizeof(EfiFileInfo) + 256 * sizeof(char);
        void* info = null;

        var s = AllocatePool(infoSize, &info);
        if (s.IsError) return s;

        s = file->GetInfo(file, &infoGuid, &infoSize, info);
        if (s == EfiStatus.BufferTooSmall)
        {
            FreePool(info);
            s = AllocatePool(infoSize, &info);
            if (s.IsError) return s;
            s = file->GetInfo(file, &infoGuid, &infoSize, info);
        }
        if (s.IsError) { FreePool(info); return s; }

        ulong fileSize = ((EfiFileInfo*)info)->FileSize;
        FreePool(info);

        if (fileSize == 0)
        {
            buffer = null;
            size = 0;
            return EfiStatus.Success;
        }

        void* buf = null;
        s = AllocatePool((nuint)fileSize, &buf);
        if (s.IsError) return s;

        nuint read = (nuint)fileSize;
        s = file->Read(file, &read, buf);
        if (s.IsError) { FreePool(buf); return s; }

        buffer = buf;
        size = read;
        return EfiStatus.Success;
    }

    /// <summary>
    /// Open a file by path relative to the image's root volume, read it
    /// entirely into a pool buffer, and close the handle. Caller frees.
    /// </summary>
    public static EfiStatus ReadAllBytesFromImageVolume(string path, out void* buffer, out nuint size)
    {
        buffer = null; size = 0;

        EfiFileProtocol* root;
        var s = OpenImageRoot(out root);
        if (s.IsError) return s;

        EfiFileProtocol* file;
        s = OpenFileRead(root, path, out file);
        if (s.IsError) { CloseFile(root); return s; }

        s = ReadAllBytes(file, out buffer, out size);
        CloseFile(file);
        CloseFile(root);
        return s;
    }
}
