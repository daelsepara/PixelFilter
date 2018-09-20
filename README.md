# PixelFilter

![PixelFilter](/Screenshots/PixelFilter.png)

PixelFilter is a tool for applying various pixel-scaling/filtering algorithms to images. The graphical user interface is written in C# and can be compiled on Linux and OSX platforms. It utilizes the various pixel filter plugins from the other project (https://github.com/daelsepara/PixelFilterPlugins). In the image above, a Kuwahara filter ( utilizing a 7x7 window was applied to the 512x512 color image of Lena. 

# Pixel Filtering/Scaling Algorithms

![Pixel Filtering/Scaling Algorithms](/Screenshots/PixelScaling.png)

For PixelFilter to work properly, you need to build the various pixel filter shared libraries. These shared libraries and must be placed inside the folder where the PixelFilter executable resides. Currently, 36 pixel filter are available. Some of these filters accept a single parameter, e.g. scale, window size, magnification, etc. When available, more options appear on the dropdown list with the description and the name of the parameter:

* EPX Eric's Pixel eXpander (2-3x)
* Kuwahara (5x5, 7x7, 9x9, 11x11 window)
* XBRZ (2-6x)
* High quality nX (2-4x)
* Low quality nX (2-4x)
* XBR original algorithm (2-4x)
* SaI
* Eagle (2-3x)
* SuperSaI
* Super Eagle
* RGB/Dot-matrix effect (2-4x)
* TV/Vertical scanlines with gamma reduction (2-4x)
* AdvMame Interpolation (2-3x)
* Scale 3x (EPX generalized to 3X)
* Maginify / Pixel multiplier/duplicator (1-nx)
* AdvMame Scaler (2-3x)
* Horizontal scanlines without gamma reduction (2-4x)
* Vertical scanlines without gamma reduction (2-4x)
* Reverse Anti-Aliasing (2x)
* Ultra 2X
* Super 2X
* Scale 2X (EPX variant)
* DES 2X (DES algorithm in FNES extended to 2X)
* Original DES algorithm in FNES
* Bilinear
* Bilinear plus
* Bilinear++ 
* Asymmetric Hiqh Quality 2xN (2x2, 2x3, 2x4)
* Asymmetric Low Quality 2xN (2x2, 2x3, 2x4)
* EPX version B
* EPX version C
* Eagle 3X version B
* Flip (horizontal, vertical)
* Rotate (90, 180, 270)
* Nearest neighbor
* No-scale TV effect

# Filter queue

![Filter queue](/Screenshots/FilterQueue.png)

Various filters can be combined into a queue. Filters on the queue can be re-arranged and applied according to your preferred order.

# Image formats

You can load PNG, XPM, GIF, JPG, BMP, ICO, TIFF, ICNS, and PGM files while only you can only save the image in PNG, JPG, BMP, ICO, and TIFF formats.
