# UnmanagedDllResolveHelper

UnmanagedDllResolveHelper is a .NET helper library for resolving unmanaged (native) library dependencies at runtime, based on the location of the calling native library.  
This is especially useful for self-contained native plugins or AOT scenarios where dependent libraries are placed alongside the main native library.

## Features

- Resolves dependent native libraries from the same directory as the calling library
- Supports Windows, Linux, and macOS
- No configuration or search path setup required
- Simple integration with AssemblyLoadContext

## How to Use

### 1. Add Reference

Add a reference to the `UnmanagedDllResolveHelper` project or NuGet package in your native library project.

### 2. Register the Resolver

In your native library's static constructor, register the resolver to `AssemblyLoadContext.Default.ResolvingUnmanagedDll`:

```csharp
using System.Runtime.Loader;

static Class1()
{
    AssemblyLoadContext.Default.ResolvingUnmanagedDll += UnmanagedDllResolveHelper.UnmanagedDllCurrentLibraryLocationResolver.ResolveUnmanagedDll;
}
```

### 3. Use P/Invoke as Usual

Declare your P/Invoke as usual. For example:

```csharp
[DllImport("SampleNativeLibraryDependency", EntryPoint = "Sum42", CallingConvention = CallingConvention.StdCall)]
public static extern int Sum42(int a);
```

### 4. Build and Place Native Libraries

Build your native libraries and place any dependencies in the same directory as the main native library.  
For example, if your main library is `SampleNativeLibraryWithLoader.dll` and it depends on `SampleNativeLibraryDependency.dll`, place both in the same output folder.

### 5. Example

```csharp
public static class Class1
{
    static Class1()
    {
        AssemblyLoadContext.Default.ResolvingUnmanagedDll += UnmanagedDllResolveHelper.UnmanagedDllCurrentLibraryLocationResolver.ResolveUnmanagedDll;
    }

    [DllImport("SampleNativeLibraryDependency", EntryPoint = "Sum42", CallingConvention = CallingConvention.StdCall)]
    public static extern int Sum42(int a);

    [UnmanagedCallersOnly(EntryPoint = "Run", CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static int Run(int a)
    {
        return Sum42(a);
    }
}
```

## Testing

See `UnmanagedDllResolveHelper.Tests/UnitTest1.cs` for a dynamic loading example using `NativeLibrary.Load` and function pointer binding.

## Supported Platforms

- Windows (.dll)
- Linux (.so)
- macOS (.dylib)

## License

MIT License
