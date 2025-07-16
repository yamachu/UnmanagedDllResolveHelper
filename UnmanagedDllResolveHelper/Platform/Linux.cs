using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper.Platform
{

    public class Linux
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DlInfo
        {
            public IntPtr dli_fname;
            public IntPtr dli_fbase;
            public IntPtr dli_sname;
            public IntPtr dli_saddr;
        }

        [DllImport("libdl.so.2")]
        private static extern int dladdr_linux(IntPtr addr, out DlInfo info);

        private static IntPtr GetCurrentModuleHandle()
        {
            var methodHandle = typeof(Linux).GetMethod(nameof(GetCurrentModuleHandle),
                BindingFlags.NonPublic | BindingFlags.Static)?.MethodHandle;

            if (methodHandle.HasValue)
            {
                return methodHandle.Value.GetFunctionPointer();
            }

            return IntPtr.Zero;
        }

        public static string? GetCurrentLibraryDirectory()
        {
            try
            {
                var moduleHandle = GetCurrentModuleHandle();
                if (moduleHandle != IntPtr.Zero)
                {
                    var result = dladdr_linux(moduleHandle, out DlInfo info);
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
                Path.Combine(basePath, $"{libraryName}.so"),
                Path.Combine(basePath, $"lib{libraryName}.so")
            };
            if (!libraryName.StartsWith("lib"))
            {
                paths.Add(Path.Combine(basePath, $"lib{libraryName}.so"));
            }

            return paths.ToArray();
        }

    }
}