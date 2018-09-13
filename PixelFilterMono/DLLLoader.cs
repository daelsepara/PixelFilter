using System;
using System.Runtime.InteropServices;

// adapted from: https://github.com/GtkSharp/GtkSharp/blob/develop/Source/Libs/Shared/FuncLoader.cs
class DLLLoader
{
	static class Linux
	{
		[DllImport("libdl", EntryPoint = "dlopen")]
		public static extern IntPtr DLOpen(string path, int flags);

		[DllImport("libdl", EntryPoint = "dlsym")]
		public static extern IntPtr DLSym(IntPtr handle, string symbol);

		[DllImport("libdl", EntryPoint = "dlclose")]
		public static extern int DLClose(IntPtr handle);
	}

	static class OSX
	{
		[DllImport("libSystem", EntryPoint = "dlopen")]
		public static extern IntPtr DLOpen(string path, int flags);

		[DllImport("libSystem", EntryPoint = "dlsym")]
		public static extern IntPtr DLSym(IntPtr handle, string symbol);

		[DllImport("libSystem", EntryPoint = "dlclose")]
		public static extern int DLClose(IntPtr handle);
	}

	const int RTLD_LAZY = 0x0001;
	const int RTLD_GLOBAL = 0x0100;

	public static IntPtr LoadLibrary(string libname)
	{
		if (Common.OSTest.IsRunningOnMac())
		{
			return OSX.DLOpen(libname, RTLD_GLOBAL | RTLD_LAZY);
		}

		return Linux.DLOpen(libname, RTLD_GLOBAL | RTLD_LAZY);
	}

	public static IntPtr GetProcAddress(IntPtr library, string function)
	{
		var ret = IntPtr.Zero;

		if (Common.OSTest.IsRunningOnMac())
		{
			ret = OSX.DLSym(library, function);
		}
		else
		{
			ret = Linux.DLSym(library, function);
		}

		return ret;
	}

	public static T LoadFunction<T>(IntPtr procaddress)
	{
		if (procaddress == IntPtr.Zero)
			return default(T);

		return Marshal.GetDelegateForFunctionPointer<T>(procaddress);
	}

	public static int FreeLibrary(IntPtr handle)
	{
		if (Common.OSTest.IsRunningOnMac())
		{
			return OSX.DLClose(handle);
		}

		return Linux.DLClose(handle);
	}
}
