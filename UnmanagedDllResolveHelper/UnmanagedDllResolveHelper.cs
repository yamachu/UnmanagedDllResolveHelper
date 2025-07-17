using System;
using System.IO;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper
{
    public static class UnmanagedDllCurrentLibraryLocationResolver
    {
        public static Func<object, string, IntPtr> ResolveUnmanagedDll(IntPtr baseFunctionPointer)
        {
            return (_, unmanagedDllName) =>
            {
                var currentLibraryDir = GetCurrentLibraryDirectory(baseFunctionPointer);
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
            };
        }

        public static IntPtr ResolveUnmanagedDll(object _, string unmanagedDllName)
        {
            unsafe
            {
                var functionPointer = (IntPtr)(delegate*<IntPtr, string?>)&GetCurrentLibraryDirectory;

                return ResolveUnmanagedDll(functionPointer)(_, unmanagedDllName);
            }
        }

        private static string? GetCurrentLibraryDirectory(IntPtr functionPointer)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Platform.OSX.GetCurrentLibraryDirectory(functionPointer);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Platform.Windows.GetCurrentLibraryDirectory(functionPointer);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Platform.Linux.GetCurrentLibraryDirectory(functionPointer);
            }
            else
            {
                return null;
            }
        }

        private static string[] GetPossibleLibraryPaths(string libraryName, string basePath)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Platform.OSX.GetPossibleLibraryPaths(libraryName, basePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Platform.Windows.GetPossibleLibraryPaths(libraryName, basePath);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return Platform.Linux.GetPossibleLibraryPaths(libraryName, basePath);
            }
            else
            {
                return Array.Empty<string>();
            }
        }
    }
}