using Gdk;
using System;
using System.Runtime.InteropServices;

public static class FilterDLL
{
	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe delegate int FSizeX();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe delegate int FSizeY();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe delegate char* FName();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe delegate char* FDescription();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe delegate byte* FImage();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe delegate void FThreshold(bool threshold);

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	delegate void FRelease();

	[UnmanagedFunctionPointer(CallingConvention.Cdecl)]
	unsafe delegate void FApply(int argc, void** argv);

	unsafe public static Pixbuf ApplyFilter(string dll, Pixbuf input, int scale, bool threshold)
	{
		IntPtr pLibrary = Common.OSTest.IsWindows() ? DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.dll", dll)) : (Common.OSTest.IsRunningOnMac() ? DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.dylib", dll)) : DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.so", dll)));

		IntPtr pSizeX = DLLLoader.GetProcAddress(pLibrary, "SizeX");
		IntPtr pSizeY = DLLLoader.GetProcAddress(pLibrary, "SizeY");
		IntPtr pThreshold = DLLLoader.GetProcAddress(pLibrary, "Threshold");
		IntPtr pApply = DLLLoader.GetProcAddress(pLibrary, "Apply");
		IntPtr pImage = DLLLoader.GetProcAddress(pLibrary, "Image");
		IntPtr pRelease = DLLLoader.GetProcAddress(pLibrary, "Release");

		var SizeX = DLLLoader.LoadFunction<FSizeX>(pSizeX);
		var SizeY = DLLLoader.LoadFunction<FSizeY>(pSizeY);
		var Threshold = DLLLoader.LoadFunction<FThreshold>(pThreshold);
		var Apply = DLLLoader.LoadFunction<FApply>(pApply);
		var Image = DLLLoader.LoadFunction<FImage>(pImage);
		var Release = DLLLoader.LoadFunction<FRelease>(pRelease);

		var target = Common.PreparePixbuf(input);
		var targetx = input.Width;
		var targety = input.Height;

		int sizex, sizey;
		byte* filtered;
		Pixbuf output;

		void** Parameters = stackalloc void*[4];

		Parameters[0] = target;
		Parameters[1] = &targetx;
		Parameters[2] = &targety;
		Parameters[3] = &scale;

		Threshold(threshold);

		Apply(4, Parameters);

		filtered = Image();

		sizex = SizeX();
		sizey = SizeY();

		output = Common.InitializePixbuf(sizex, sizey);

		Common.Copy(output, filtered);

		Release();

		DLLLoader.FreeLibrary(pLibrary);

		return output;
	}

	unsafe public static string GetName(string dll)
	{
		IntPtr pLibrary = Common.OSTest.IsWindows() ? DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.dll", dll)) : (Common.OSTest.IsRunningOnMac() ? DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.dylib", dll)) : DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.so", dll)));

		IntPtr pName = DLLLoader.GetProcAddress(pLibrary, "Name");

		var Name = DLLLoader.LoadFunction<FName>(pName);

		var name = Marshal.PtrToStringAuto((IntPtr)Name());

		DLLLoader.FreeLibrary(pLibrary);

		return name;
	}

	unsafe public static string GetDescription(string dll)
	{
		IntPtr pLibrary = Common.OSTest.IsWindows() ? DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.dll", dll)) : (Common.OSTest.IsRunningOnMac() ? DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.dylib", dll)) : DLLLoader.LoadLibrary(String.Format("./libpixel++{0}.so", dll)));

		IntPtr pDescription = DLLLoader.GetProcAddress(pLibrary, "Description");

		var Description = DLLLoader.LoadFunction<FDescription>(pDescription);

		var description = Marshal.PtrToStringAuto((IntPtr)Description());

		DLLLoader.FreeLibrary(pLibrary);

		return description;
	}
}
