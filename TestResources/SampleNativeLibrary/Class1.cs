namespace SampleNativeLibrary;

using System;
using System.Runtime.InteropServices;

public static class Class1
{
    [DllImport("SampleNativeLibraryDependency", EntryPoint = "Sum42", CallingConvention = CallingConvention.StdCall)]
    public static extern int Sum42(int a);

    [UnmanagedCallersOnly(EntryPoint = "Run", CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static int Run(int a)
    {
        return Sum42(a);
    }
}
