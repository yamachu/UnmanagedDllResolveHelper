using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace UnmanagedDllResolveHelper.Platform
{

    internal class Windows
    {
        private const uint GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 0x00000004;
        private const uint GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x00000002;
        private const int MAX_PATH = 260;
        private const int EXTENDED_MAX_PATH = 32768;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern bool GetModuleHandleEx(uint dwFlags, IntPtr lpModuleName, out IntPtr phModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern uint GetModuleFileName(IntPtr hModule, StringBuilder lpFilename, int nSize);

        private static int _Internal_ModuleHandle()
        {
            return DateTime.Now.Millisecond;
        }

        public static string? GetCurrentLibraryDirectory()
        {
            try
            {
                var handle = typeof(Windows)
                .GetMethod(
                    nameof(_Internal_ModuleHandle),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
                )?.MethodHandle.GetFunctionPointer();

                if (handle == null)
                    return null;

                GetModuleHandleEx(
                    GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS | GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT,
                    handle.Value,
                    out var hModule);

                if (hModule == IntPtr.Zero)
                    return null;

                var sb = new StringBuilder(EXTENDED_MAX_PATH);
                GetModuleFileName(hModule, sb, sb.Capacity);
                if (sb.Length == 0)
                    return null;

                var fullPath = sb.ToString();

                return Path.GetDirectoryName(fullPath);
            }
            catch
            {
                return null;
            }
        }

        public static string[] GetPossibleLibraryPaths(string libraryName, string basePath)
        {
            var paths = new List<string>
            {
                Path.Combine(basePath, $"{libraryName}.dll")
            };
            if (!libraryName.StartsWith("lib"))
            {
                paths.Add(Path.Combine(basePath, $"lib{libraryName}.dll"));
            }

            return paths.ToArray();
        }

    }
}
