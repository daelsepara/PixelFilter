using Gdk;
using System;
using System.Runtime.InteropServices;

public static class Common
{
	// see: https://github.com/jpobst/Pinta/blob/1.6/Pinta.Core/Managers/SystemManager.cs#L125
	public static class OSTest
	{
		[DllImport("libc", EntryPoint = "uname")]
		static extern int Uname(IntPtr buf);

		public static bool IsRunningOnMac()
		{
			IntPtr buf = IntPtr.Zero;
			try
			{
				buf = Marshal.AllocHGlobal(8192);
				// This is a hacktastic way of getting sysname from uname ()
				if (Uname(buf) == 0)
				{
					string os = Marshal.PtrToStringAnsi(buf);

					if (os == "Darwin")
						return true;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Error: {0}", ex.Message);
			}
			finally
			{
				if (buf != IntPtr.Zero)
					Marshal.FreeHGlobal(buf);
			}

			return false;
		}
	}

	public static Pixbuf InitializePixbuf(int width, int height)
	{
		var pixbuf = new Pixbuf(Colorspace.Rgb, false, 8, width, height);

		pixbuf.Fill(0);

		return pixbuf;
	}

	unsafe public static byte* PreparePixbuf(Pixbuf input)
	{
		var temp = (byte*)Marshal.AllocHGlobal(input.Width * input.Height * input.NChannels);

		for (var y = 0; y < input.Height; y++)
		{
			for (var x = 0; x < input.Width; x++)
			{
				var ptr = input.Pixels + y * input.Rowstride + x * input.NChannels;

				for (var offset = 0; offset < input.NChannels; offset++)
				{
					temp[(y * input.Width + x) * input.NChannels + offset] = Marshal.ReadByte(ptr, offset);
				}
			}
		}

		return temp;
	}

	unsafe public static void Copy(Pixbuf dst, byte* src)
	{
		if (dst != null)
		{
			for (var y = 0; y < dst.Height; y++)
			{
				for (var x = 0; x < dst.Width; x++)
				{
					var ptr = dst.Pixels + y * dst.Rowstride + x * dst.NChannels;

					for (var offset = 0; offset < dst.NChannels; offset++)
					{
						Marshal.WriteByte(ptr, offset, src[(y * dst.Width + x) * dst.NChannels + offset]);
					}
				}
			}
		}
	}

	public static void Free(params IDisposable[] trash)
	{
		foreach (var item in trash)
		{
			if (item != null)
			{
				item.Dispose();
			}
		}
	}

	unsafe public static void Free(params byte*[] trash)
	{
		foreach (var item in trash)
		{
			if (item != null)
			{
				Marshal.FreeHGlobal((IntPtr)item);
			}
		}
	}
}
