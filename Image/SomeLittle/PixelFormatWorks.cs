using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Image
{
    //contain methods where we change image depth 1 8 24 - bitmap parameter PixelFormat
    public static class PixelFormatWorks
    {
        #region RGB2Gray8bpp
        //convert 24bpp RGB image into 8bpp gray
        public static void RGB2Gray8bpp(Bitmap img)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);
            image = RGB2Gray8bppConveter(img);

            string outName = defPath + imgName + "_rgbToGray8bppIndexed.png";
            Helpers.SaveOptions(image, outName, ".png");
        }

        public static Bitmap RGB2Gray8bppBitmap(Bitmap img)
        {
            return RGB2Gray8bppConveter(img);
        }

        private static Bitmap RGB2Gray8bppConveter(Bitmap img)
        {
            int r, ic, oc, bmpStride, outputStride;
            ColorPalette palette;
            BitmapData inputData, outputData;
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8)
            {
                Console.WriteLine("Image already 8bit. Method RGB2Gray8bpp. Return themself");
                return img;
            }

            if (Checks.RGBinput(img))
            {
                //Build a grayscale color Palette
                palette = image.Palette;
                for (int i = 0; i < 256; i++)
                {
                    Color tmp = Color.FromArgb(255, i, i, i);
                    palette.Entries[i] = Color.FromArgb(255, i, i, i);
                }
                image.Palette = palette;

                //Lock the images
                inputData    = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                outputData   = image.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                bmpStride    = inputData.Stride;
                outputStride = outputData.Stride;

                try
                {
                    //Traverse each pixel of the image
                    unsafe
                    {
                        byte* bmpPtr = (byte*)inputData.Scan0.ToPointer();
                        var outputPtr = (byte*)outputData.Scan0.ToPointer();

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
                }
                catch (Exception e)
                {
                    Console.WriteLine("Some problems while using unsafe code. Method: RGB2Gray8bpp. \nMessage: " + e.Message);
                }
                finally
                {
                    //Unlock the images
                    img.UnlockBits(inputData);
                    image.UnlockBits(outputData);
                }
            }

            return image;
        }

        #endregion

        #region Bpp24Gray2Gray8bpp
        //convert 24bpp BW image into 8bpp gray
        public static void Bpp24Gray2Gray8bpp(Bitmap img)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);
            image = Bpp24Gray2Gray8bppConverter(img);

            string outName = defPath + imgName + "_24bppGrayto8bppIndexed.png";
            Helpers.SaveOptions(image, outName, ".png");
        }

        public static Bitmap Bpp24Gray2Gray8bppBitMap(Bitmap img)
        {
            return Bpp24Gray2Gray8bppConverter(img);
        }

        private static Bitmap Bpp24Gray2Gray8bppConverter(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            int r, ic, oc, bmpStride, outputStride;
            ColorPalette palette;
            BitmapData bmpData, outputData;

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8)
            {
                Console.WriteLine("Image already 8bit. Method Bpp24Gray2Gray8bpp. Return themself");
                return img;
            }

            if (Checks.BWinput(img))
            {
                //Build a grayscale color Palette
                palette = image.Palette;
                for (int i = 0; i < 256; i++)
                {
                    Color tmp = Color.FromArgb(255, i, i, i);
                    palette.Entries[i] = Color.FromArgb(255, i, i, i);
                }
                image.Palette = palette;

                //Lock the images
                bmpData      = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
                outputData   = image.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
                bmpStride    = bmpData.Stride;
                outputStride = outputData.Stride;

                try
                {
                    //Traverse each pixel of the image
                    unsafe
                    {
                        byte* bmpPtr = (byte*)bmpData.Scan0.ToPointer();
                        var outputPtr = (byte*)outputData.Scan0.ToPointer();

                        //Note that ic is the input column and oc is the output column
                        for (r = 0; r < img.Height; r++)
                            for (ic = oc = 0; oc < img.Width; ic += 3, ++oc)
                                outputPtr[r * outputStride + oc] = (byte)(int)
                                (bmpPtr[r * bmpStride + ic]);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Some problems while using unsafe code.Method: Bpp24Gray2Gray8bpp. \nMessage: " + e.Message);
                }
                finally
                {
                    //Unlock the imagess
                    img.UnlockBits(bmpData);
                    image.UnlockBits(outputData);
                }
            }

            return image;
        }

        #endregion

        #region ImageTo1bpp
        //convert 24bpp RGB image into binary image
        public static void ImageTo1Bpp(Bitmap img)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            image = ImageTo1BppConverter(img, 0.5);

            string outName = defPath + imgName + "_1BppImage.png";
            Helpers.SaveOptions(image, outName, ".png");
        }

        public static Bitmap ImageTo1BppBitmap(Bitmap img)
        {
            return ImageTo1BppConverter(img, 0.5);
        }

        public static void ImageTo1Bpp(Bitmap img, double level)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            image = ImageTo1BppConverter(img, level);

            string outName = defPath + imgName + "_1BppImageLvl_" + level.ToString() + ".png";
            Helpers.SaveOptions(image, outName, ".png");
        }

        public static Bitmap ImageTo1BppBitmap(Bitmap img, double level)
        {
            return ImageTo1BppConverter(img, level);
        }

        private static Bitmap ImageTo1BppConverter(Bitmap img, double level)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            BitmapData data = image.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);

            if (level > 1 || level < 0)
            {
                Console.WriteLine("Level value must be in range 0..1. Set to default 0.5");
                level = 0.5;
            }

            try
            {
                byte[] scan = new byte[(img.Width + 7) / 8];
                for (int y = 0; y < img.Height; y++)
                {
                    for (int x = 0; x < img.Width; x++)
                    {
                        if (x % 8 == 0) scan[x / 8] = 0;
                        Color c = img.GetPixel(x, y);
                        if (c.GetBrightness() >= level) scan[x / 8] |= (byte)(0x80 >> (x % 8));
                    }
                    Marshal.Copy(scan, 0, (IntPtr)((long)data.Scan0 + data.Stride * y), scan.Length);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Some problems at Imageto1bpp converting method. \nMessage: " + e.Message);
            }
            finally { image.UnlockBits(data); }

            return image;
        }

        #endregion

        #region Bpp8fastTo24bpp
        //convert 8bpp image into 24bpp BW 
        public static void Bpp8fastTo24bppGray(Bitmap img)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = Bpp8fastTo24bppGrayHelper(img);

            string outName = defPath + imgName + "_8bppto24bpp" + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap Bpp8fastTo24bppGrayBitmap(Bitmap img)
        {
            return Bpp8fastTo24bppGrayHelper(img);
        }

        private static Bitmap Bpp8fastTo24bppGrayHelper(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (Depth == 8)
            {
                using (Graphics g = Graphics.FromImage(image))
                {
                    g.PageUnit = GraphicsUnit.Pixel; // Prevent DPI conversion                    
                    g.DrawImageUnscaled(img, 0, 0);  // Draw the image
                }
            }
            else
            { Console.WriteLine("Non 8bit input at method: Bpp8fastTo24bppGray."); }

            return image;
        }

        #endregion
    }
}
