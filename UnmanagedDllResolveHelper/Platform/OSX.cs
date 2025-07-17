using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper.Platform
{

    internal class OSX
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

        private static int _Internal_ModuleHandle()
        {
            return DateTime.Now.Millisecond;
        }

        public static string? GetCurrentLibraryDirectory()
        {
            try
            {
                IntPtr handle = IntPtr.Zero;
                var maybeHandle = typeof(OSX)
                .GetMethod(
                    nameof(_Internal_ModuleHandle),
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static
                )?.MethodHandle;
                if (maybeHandle.HasValue)
                {
                    handle = maybeHandle.Value.GetFunctionPointer();
                    System.Console.WriteLine($"maybeHandle: {handle}");
                }
                else
                {
                    handle = GetCurrentModuleHandle();
                }

                if (handle != IntPtr.Zero)
                {
                    var result = dladdr(handle, out DlInfo info);
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
            };
            if (!libraryName.StartsWith("lib"))
            {
                paths.Add(Path.Combine(basePath, $"lib{libraryName}.dylib"));
            }

            return paths.ToArray();
        }

    }
}