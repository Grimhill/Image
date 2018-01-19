using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

//Some little operations
namespace Image
{
    public static class SomeLittle
    {
        public static void RGBArrayToImage(int[,] Rc, int[,] Gc, int[,] Bc)
        {
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");
            //write image back from rgb array into file            
            int height = Rc.GetLength(0);
            int width = Rc.GetLength(1);

            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            bitmap = Helpers.SetPixels(bitmap, Rc, Gc, Bc);

            bitmap.Save(Directory.GetCurrentDirectory() + "\\Rand\\rgbArrayToImage.jpg");
        }

        //only for 24bpp input
        public static void RGBToGray24bpp(Bitmap image, string ImgExtension)
        {
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            // Loop through the images pixels to reset color.
            for (int x = 0; x < image.Height; x++)
            {
                for (int y = 0; y < image.Width; y++)
                {
                    Color pixelColor = image.GetPixel(y, x);

                    byte r = pixelColor.R;
                    byte g = pixelColor.G;
                    byte b = pixelColor.B;
                    //0.2989 * R + 0.5870 * G + 0.1140 * B 
                    byte gray = (byte)(0.2989 * r + 0.587 * g + 0.114 * b);

                    Color newColor = Color.FromArgb(gray, gray, gray);
                    image.SetPixel(y, x, newColor); // Now greyscale
                }
            }
            //image.Save("rgbToGray24bpp.jpg");
            string outName = Directory.GetCurrentDirectory() + "\\Rand\\rgbToGray24bpp" + ImgExtension;
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void RGB2Gray8bpp(Bitmap img, string fileName)
        {
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            int r, ic, oc, bmpStride, outputStride;
            ColorPalette palette;
            BitmapData bmpData, outputData;

            //Build a grayscale color Palette
            palette = image.Palette;
            for (int i = 0; i < 256; i++)
            {
                Color tmp = Color.FromArgb(255, i, i, i);
                palette.Entries[i] = Color.FromArgb(255, i, i, i);
            }
            image.Palette = palette;

            //Lock the images
            bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            outputData = image.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            bmpStride = bmpData.Stride;
            outputStride = outputData.Stride;

            //Traverse each pixel of the image
            unsafe
            {
                byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer(),
                outputPtr = (byte*)outputData.Scan0.ToPointer();

                //Convert the pixel to it's luminance using the formula:
                // L = .299*R + .587*G + .114*B
                //Note that ic is the input column and oc is the output column
                for (r = 0; r < img.Height; r++)
                    for (ic = oc = 0; oc < img.Width; ic += 3, ++oc)
                        outputPtr[r * outputStride + oc] = (byte)(int)
                        (0.299f * bmpPtr[r * bmpStride + ic] +
                        0.587f * bmpPtr[r * bmpStride + ic + 1] +
                        0.114f * bmpPtr[r * bmpStride + ic + 2]);

            }

            //Unlock the images
            img.UnlockBits(bmpData);
            image.UnlockBits(outputData);

            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_rgbToGray8bppIndexed" + ImgExtension);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            image.Save(outName);
            //Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void ImageTo1Bpp(Bitmap img, string fileName)
        {
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);

            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

            byte[] scan = new byte[(w + 7) / 8];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x % 8 == 0) scan[x / 8] = 0;
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= 0.5) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((long)data.Scan0 + data.Stride * y), scan.Length);
            }
            bmp.UnlockBits(data);

            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_1BppImage" + ImgExtension);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(bmp, outName, ImgExtension);
        }

        public static void ImageTo1Bpp(Bitmap img, double level, string fileName)
        {
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);

            int w = img.Width;
            int h = img.Height;
            Bitmap bmp = new Bitmap(w, h, PixelFormat.Format1bppIndexed);
            BitmapData data = bmp.LockBits(new Rectangle(0, 0, w, h), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

            if (level > 1 || level < 0)
            {
                Console.WriteLine("Level value must be in range 0..1. Set to default 0.5");
                level = 0.5;
            }

            byte[] scan = new byte[(w + 7) / 8];
            for (int y = 0; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    if (x % 8 == 0) scan[x / 8] = 0;
                    Color c = img.GetPixel(x, y);
                    if (c.GetBrightness() >= level) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                }
                Marshal.Copy(scan, 0, (IntPtr)((long)data.Scan0 + data.Stride * y), scan.Length);
            }
            bmp.UnlockBits(data);

            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_1BppImageLvl" + level.ToString() + ImgExtension);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(bmp, outName, ImgExtension);
        }

        //only for RGB images, b&w 24bbp and 24bpp negatives. (what about8bpp?)
        public static void MakeNegativeAndBack(Bitmap img, string fileName)
        {           
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            var Rcn = Rc.ConstSubArrayElements(255); 
            var Gcn = Gc.ConstSubArrayElements(255); 
            var Bcn = Bc.ConstSubArrayElements(255); 

            image = Helpers.SetPixels(image, Rcn, Gcn, Bcn);

            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_NegativeOrRestored" + ImgExtension);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        //only for RGB images, b&w 24bbp. (what about8bpp?)
        public static void GammaCorrectionFun(Bitmap img, double c, double gamma, string fileName)
        {          
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            //higher c and gamma - lighter image after correction            
            var Rcg = Rc.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(c).ArrayToUint8();            
            var Gcg = Gc.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(c).ArrayToUint8();            
            var Bcg = Bc.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(c).ArrayToUint8();

            image = Helpers.SetPixels(image, Rcg, Gcg, Bcg);

            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_GammaCorrection" + ImgExtension);
            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void Mirror(Bitmap img, Mir direction, string fileName)
        {            
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            string outName = String.Empty;

            PadMyArray<int> padArr;
            padArr = new PadMyArray<int>();

            if (direction.ToString() == "left" || direction.ToString() == "right")
            {
                resultR = padArr.PadArray(Rc, 0, img.Width, PadType.symmetric, Direction.pre);
                resultG = padArr.PadArray(Gc, 0, img.Width, PadType.symmetric, Direction.pre);
                resultB = padArr.PadArray(Bc, 0, img.Width, PadType.symmetric, Direction.pre);

                if (direction.ToString() == "left")
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_mirrorLeft" + ImgExtension;
                }
                else
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_mirrorRight" + ImgExtension;
                }
            }
            else
            {
                resultR = padArr.PadArray(Rc, img.Height, 0, PadType.symmetric, Direction.pre);
                resultG = padArr.PadArray(Gc, img.Height, 0, PadType.symmetric, Direction.pre);
                resultB = padArr.PadArray(Bc, img.Height, 0, PadType.symmetric, Direction.pre);

                if (direction.ToString() == "top")
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_mirrorTop" + ImgExtension;
                }
                else
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_mirrorBot" + ImgExtension;
                }
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            outName = MoreHelpers.OutputFileNames(outName);
            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void CropImage(Bitmap bitmap, string filename, int cutLeft, int cutRight, int cutTop, int cutBottom, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Cropped");

            //count new width and height
            int newWidth = bitmap.Width - cutLeft - cutRight;
            int newHeight = bitmap.Height - cutTop - cutBottom;

            //new bitmam object for cropped image    
            Bitmap img = new Bitmap(newWidth, newHeight, bitmap.PixelFormat);

            if (newWidth <= 0 || newHeight <= 0)
            {
                Console.WriteLine("Crop width or height more than image`s one.");
            }
            else
            {
                //left x-coordinate of the upper-left corner of the rectangle.
                //right y-coordinate of the upper-left corner of the rectangle.
                Rectangle rect = new Rectangle(cutLeft, cutTop, newWidth, newHeight);

                //clone selected area into new bitmap object
                Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);

                string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Cropped\\" + fileName + "_cropped" + ImgExtension);

                //img.Save(outName, ImageFormat.Jpeg);
                Helpers.SaveOptions(img, outName, ImgExtension);
            }
        }

        public static void Bpp8ConvertTo24bpp(Bitmap img, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            using (Graphics g = Graphics.FromImage(image))
            {
                // Prevent DPI conversion
                g.PageUnit = GraphicsUnit.Pixel;
                // Draw the image
                g.DrawImageUnscaled(img, 0, 0);
            }

            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_8bppto24bpp" + ImgExtension);
            outName = MoreHelpers.OutputFileNames(outName);
            //Helpers.SaveOptions(image, outName, ImgExtension);
            image.Save(outName);
        }

        public static void SaveImageInOtherFormat(Bitmap image, string fileName, SupportFormats newFormat)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            //string outName = fileName + "." + newFormat.ToString();
            //string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + name + "." + newFormat.ToString();
            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + newFormat.ToString());
            Helpers.SaveOptions(image, outName, newFormat.ToString().ToLower());
        }
    }

    public enum Mir
    {
        top,
        right,
        bot,
        left
    }
}
