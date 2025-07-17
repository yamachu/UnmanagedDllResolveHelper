using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper.Platform
{

    internal class Linux
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct DlInfo
        {
            public IntPtr dli_fname;
            public IntPtr dli_fbase;
            public IntPtr dli_sname;
            public IntPtr dli_saddr;
        }

        [DllImport("libdl")]
        private static extern int dladdr(IntPtr addr, out DlInfo info);

        private static int _Internal_ModuleHandle()
        {
            return DateTime.Now.Millisecond;
        }

        public static string? GetCurrentLibraryDirectory()
        {
            try
            {
                var handle = typeof(Linux)
                .GetMethod(
                    nameof(_Internal_ModuleHandle),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
                )?.MethodHandle.GetFunctionPointer();

                if (handle == null)
                    return null;

                if (handle.Value == IntPtr.Zero)
                    return null;

                var result = dladdr(handle.Value, out DlInfo info);
                if (result != 0 && info.dli_fname != IntPtr.Zero)
                {
                    var modulePath = Marshal.PtrToStringAnsi(info.dli_fname);
                    if (!string.IsNullOrEmpty(modulePath))
                    {
                        return Directory.GetParent(modulePath)?.FullName;
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
            };
            if (!libraryName.StartsWith("lib"))
            {
                paths.Add(Path.Combine(basePath, $"lib{libraryName}.so"));
            }

            return paths.ToArray();
        }

    }
}