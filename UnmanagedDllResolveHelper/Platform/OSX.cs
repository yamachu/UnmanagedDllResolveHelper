using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper.Platform
{

    public class OSX
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DlInfo
        {
            public IntPtr dli_fname;
            public IntPtr dli_fbase;
            public IntPtr dli_sname;
            public IntPtr dli_saddr;
        }

        [DllImport("libdl.dylib")]
        private static extern int dladdr(IntPtr addr, out DlInfo info);

        [UnmanagedCallersOnly(EntryPoint = "__DONT_CALL_UnmanagedDllResolveHelper_Platform_OSX__")]
        public static void __OSX_NATIVE_EXPORT_FUNCTION__() { }

        private static unsafe IntPtr GetCurrentModuleHandle()
        {
            return (IntPtr)(delegate* unmanaged<void>)&__OSX_NATIVE_EXPORT_FUNCTION__;
        }

        public static string? GetCurrentLibraryDirectory()
        {
            try
            {
                var moduleHandle = GetCurrentModuleHandle();
                if (moduleHandle != IntPtr.Zero)
                {
                    var result = dladdr(moduleHandle, out DlInfo info);
                    if (result != 0 && info.dli_fname != IntPtr.Zero)
                    {
                        var modulePath = Marshal.PtrToStringAnsi(info.dli_fname);
                        if (!string.IsNullOrEmpty(modulePath))
                        {
                            return Directory.GetParent(modulePath)?.FullName;
                        }
                    }
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public static string[] GetPossibleLibraryPaths(string libraryName, string basePath)
        {
            var paths = new List<string>
            {
                Path.Combine(basePath, $"{libraryName}.dylib"),
                Path.Combine(basePath, $"lib{libraryName}.dylib")
            };
            if (!libraryName.StartsWith("lib"))
            {
                paths.Add(Path.Combine(basePath, $"lib{libraryName}.dylib"));
            }

            return paths.ToArray();
        }

    }
}