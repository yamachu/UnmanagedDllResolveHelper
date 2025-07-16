using System.Runtime.InteropServices;

namespace UnmanagedDllResolveHelper.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {

        var dllName = Environment.GetEnvironmentVariable("NATIVE_PLUGIN_NAME") ?? throw new InvalidOperationException("NATIVE_PLUGIN_NAME not set");
        using var lib = new NativePluginProviderDynamic($"vendor/{dllName}");

        Assert.NotNull(lib);
        Assert.Equal(43, lib.Run(1));

    }
}

public interface INativePluginProvider
{
    int Run(int a);
}

public class NativePluginProviderDynamic : INativePluginProvider, IDisposable
{
    private IntPtr _libHandle;
    private delegate int RunDelegate(int a);
    private RunDelegate? _run;

    public NativePluginProviderDynamic(string dllPath)
    {
        _libHandle = NativeLibrary.Load(dllPath);
        var proc = NativeLibrary.GetExport(_libHandle, "Run");
        _run = Marshal.GetDelegateForFunctionPointer<RunDelegate>(proc);
    }

    public int Run(int a)
    {
        if (_run == null)
            throw new InvalidOperationException("Native function not loaded");
        return _run(a);
    }

    public void Dispose()
    {
        if (_libHandle != IntPtr.Zero)
        {
            NativeLibrary.Free(_libHandle);
            _libHandle = IntPtr.Zero;
        }
    }
}
