using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper
{
    public static class UnmanagedDllCurrentLibraryLocationResolver
    {
        public static IntPtr ResolveUnmanagedDll(Assembly _, string unmanagedDllName)
        {
            var currentLibraryDir = GetCurrentLibraryDirectory();
            if (string.IsNullOrEmpty(currentLibraryDir))
            {
                return IntPtr.Zero;
            }

            var possiblePaths = GetPossibleLibraryPaths(unmanagedDllName, currentLibraryDir);
            foreach (var path in possiblePaths)
            {
                if (!File.Exists(path))
                {
                    continue;
                }

                if (NativeLibrary.TryLoad(path, out IntPtr handle))
                {
                    return handle;
                }
            }

            return IntPtr.Zero;
        }

        private static string? GetCurrentLibraryDirectory()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Platform.Linux.GetCurrentLibraryDirectory();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Platform.OSX.GetCurrentLibraryDirectory();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Platform.Windows.GetCurrentLibraryDirectory();
            }
            else
            {
                throw new PlatformNotSupportedException("This platform is not supported.");
            }
        }

        private static string[] GetPossibleLibraryPaths(string libraryName, string basePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Platform.Linux.GetPossibleLibraryPaths(libraryName, basePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Platform.OSX.GetPossibleLibraryPaths(libraryName, basePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Platform.Windows.GetPossibleLibraryPaths(libraryName, basePath);
            }
            else
            {
                throw new PlatformNotSupportedException("This platform is not supported.");
            }
        }
    }
}