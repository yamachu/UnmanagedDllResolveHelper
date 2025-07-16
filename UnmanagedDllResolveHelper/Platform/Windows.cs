using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper.Platform
{

    public class Windows
    {
        private const int MAX_PATH = 260;
        private const int EXTENDED_MAX_PATH = 32768;

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern IntPtr GetModuleHandle(string? lpModuleName);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetModuleFileName(IntPtr hModule, char[] lpFilename, int nSize);

        public static string? GetCurrentLibraryDirectory()
        {
            try
            {
                var moduleHandle = GetModuleHandle(null);
                if (moduleHandle != IntPtr.Zero)
                {
                    var buffer = new char[EXTENDED_MAX_PATH];
                    var result = GetModuleFileName(moduleHandle, buffer, buffer.Length);
                    if (result > 0)
                    {
                        var modulePath = new string(buffer, 0, result);
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
                Path.Combine(basePath, $"{libraryName}.dll")
            };
            if (!libraryName.EndsWith(".dll"))
            {
                paths.Add(Path.Combine(basePath, $"lib{libraryName}.dll"));
            }

            return paths.ToArray();
        }

    }
}