namespace SampleNativeLibraryDependency;

using System;
using System.Runtime.InteropServices;

public static class Class1
{
    [UnmanagedCallersOnly(EntryPoint = "Sum42", CallConvs = new[] { typeof(System.Runtime.CompilerServices.CallConvStdcall) })]
    public static int Sum42(int a)
    {
        return 42 + a;
    }
}
