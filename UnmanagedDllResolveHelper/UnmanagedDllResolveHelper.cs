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
            return true switch
            {
                bool _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => Platform.Linux.GetCurrentLibraryDirectory(),
                bool _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => Platform.OSX.GetCurrentLibraryDirectory(),
                bool _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => Platform.Windows.GetCurrentLibraryDirectory(),
                _ => throw new PlatformNotSupportedException("This platform is not supported."),
            };
        }

        private static string[] GetPossibleLibraryPaths(string libraryName, string basePath)
        {
            return true switch
            {
                bool _ when RuntimeInformation.IsOSPlatform(OSPlatform.Linux) => Platform.Linux.GetPossibleLibraryPaths(libraryName, basePath),
                bool _ when RuntimeInformation.IsOSPlatform(OSPlatform.OSX) => Platform.OSX.GetPossibleLibraryPaths(libraryName, basePath),
                bool _ when RuntimeInformation.IsOSPlatform(OSPlatform.Windows) => Platform.Windows.GetPossibleLibraryPaths(libraryName, basePath),
                _ => throw new PlatformNotSupportedException("This platform is not supported."),
            };
        }
    }
}