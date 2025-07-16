namespace SampleNativeLibraryWithLoader;

using System;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

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
