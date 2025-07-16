using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper.Platform
{

    internal class Linux
    {
        public static string? GetCurrentLibraryDirectory()
        {
            try
            {
                // For Linux, we'll use the app base directory for single-file apps
                var baseDirectory = System.AppContext.BaseDirectory;
                if (!string.IsNullOrEmpty(baseDirectory))
                {
                    return baseDirectory;
                }
                
                // Fallback to executing assembly location
                var assemblyLocation = System.Reflection.Assembly.GetExecutingAssembly().Location;
                if (!string.IsNullOrEmpty(assemblyLocation))
                {
                    return Directory.GetParent(assemblyLocation)?.FullName;
                }
            }
            catch
            {
                // Fallback to current directory
                return Directory.GetCurrentDirectory();
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