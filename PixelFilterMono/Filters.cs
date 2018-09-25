using System;
using System.Collections.Generic;
using System.IO;

public static class Filters
{
	public static List<FilterPlugin> FilterList = new List<FilterPlugin>();

	public static void Initialize()
	{
		AddFilter("2xscl", "Scale", new int[] { 2 });
		AddFilter("advmameinterp", "Scale", new int[] { 2, 3 });
		AddFilter("amscale", "Scale", new int[] { 2, 3 });
		AddFilter("bilinear", "Scale", new int[] { 2 });
		AddFilter("bilinearplus", "Scale", new int[] { 2 });
		AddFilter("bilinearpp", "Scale", new int[] { 2 });
		AddFilter("des", "Scale", new int[] { 1 });
		AddFilter("des2x", "Scale", new int[] { 2 });
		AddFilter("eagle", "Scale", new int[] { 2, 3 });
		AddFilter("eagle3xb", "Scale", new int[] { 3 });
		AddFilter("epx", "Scale", new int[] { 2, 3 });
		AddFilter("epxb", "Scale", new int[] { 2 });
		AddFilter("epxc", "Scale", new int[] { 2 });
		AddFilter("flip", "Flip (Horizonal/Vertical)", new int[] { 0, 1 });
		AddFilter("gs", "Iterations", new int[] { 10, 50, 100, 500, 1000 });
		AddFilter("horiz", "Scale", new int[] { 1, 2, 3, 4 });
		AddFilter("hq2xn", "Scale", new int[] { 2, 3, 4 });
		AddFilter("hqx", "Scale", new int[] { 2, 3, 4 });
		AddFilter("kuwahara", "Window", new int[] { 5, 7, 9, 11, 13, 15, 17, 19, 21, 23, 25 });
		AddFilter("lq2xn", "Scale", new int[] { 2, 3, 4 });
		AddFilter("lqx", "Scale", new int[] { 2, 3, 4 });
		AddFilter("magnify", "Magnification", new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });
		AddFilter("nearest", "Scale", new int[] { 2 });
		AddFilter("reverseaa", "Scale", new int[] { 2 });
		AddFilter("rgb", "Scale", new int[] { 1, 2, 3, 4 });
		AddFilter("rotate", "Rotation Angle (90, 180, 270)", new int[] { 0, 1, 2 });
		AddFilter("sai", "Scale", new int[] { 2 });
		AddFilter("scale3x", "Scale", new int[] { 3 });
		AddFilter("super2x", "Scale", new int[] { 2 });
		AddFilter("supereagle", "Scale", new int[] { 2 });
		AddFilter("supersai", "Scale", new int[] { 2 });
		AddFilter("tv", "Scale", new int[] { 1, 2, 3, 4 });
		AddFilter("tvzero", "Scanlines", new int[] { 2, 3, 4 });
		AddFilter("ultra2x", "Scale", new int[] { 2 });
		AddFilter("vertscan", "Scale", new int[] { 1, 2, 3, 4 });
		AddFilter("xbr", "Scale", new int[] { 2, 3, 4 });
		AddFilter("xbrz", "Scale", new int[] { 2, 3, 4, 5, 6 });

		foreach (var filter in FilterList)
		{
			Console.WriteLine("{0}: {1}", filter.Name, filter.Description);
		}
	}

	static void AddFilter(string library, string parameter, int[] parameters)
	{
		if (Common.OSTest.IsRunningOnMac() ? File.Exists(String.Format("./libpixel++{0}.dylib", library)) : File.Exists(String.Format("./libpixel++{0}.so", library)))
		{
			var description = FilterDLL.GetDescription(library);
			var name = FilterDLL.GetName(library);

			FilterList.Add(new FilterPlugin(name, description, library, parameter, parameters, FilterDLL.ApplyFilter));
		}
	}
}
