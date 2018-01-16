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
            //write image back from rgb array into file            
            int height = Rc.GetLength(0);
            int width = Rc.GetLength(1);

            var bitmap = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            bitmap = Helpers.SetPixels(bitmap, Rc, Gc, Bc);

            bitmap.Save("rgbArrayToImage.jpg");
        }

        public static void RGBToGray(Bitmap image)
        {
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
            image.Save("rgbToGray.jpg");
        }

        public static void ImageTo1Bpp(Bitmap img)
        {
            int w = img.Width;
            int h = img.Height;
            string outName = String.Empty;
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

            outName = Directory.GetCurrentDirectory() + "\\Rand\\1BppImage.jpg";
            bmp.Save(outName);
        }

        public static void ImageTo1Bpp(Bitmap img, double level)
        {
            int w = img.Width;
            int h = img.Height;
            string outName = String.Empty;
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

            outName = Directory.GetCurrentDirectory() + "\\Rand\\1BppImage.jpg";
            bmp.Save(outName);
        }

        //only for RGB images, b&w 24bbp and 24bpp negatives. (what about8bpp?)
        public static void MakeNegativeAndBack(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;
            string outName = String.Empty;

            var Rcn = ArrOp.ConstSubArrayElements(255, Rc);
            var Gcn = ArrOp.ConstSubArrayElements(255, Gc);
            var Bcn = ArrOp.ConstSubArrayElements(255, Bc);

            image = Helpers.SetPixels(image, Rcn, Gcn, Bcn);

            outName = Directory.GetCurrentDirectory() + "\\NegativeAndBack\\NegativeOrRestored.jpg";
            //dont forget, that directory NegativeAndBack must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //only for RGB images, b&w 24bbp. (what about8bpp?)
        public static void GammaCorrectionFun(Bitmap img, double c, double gamma)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;
            string outName = String.Empty;

            //higher c and gamma - lighter image after correction
            var Rcg = ArrOp.ArrayToUint8(ArrOp.ArrayMultByConst(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Rc), gamma), c));
            var Gcg = ArrOp.ArrayToUint8(ArrOp.ArrayMultByConst(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gc), gamma), c));
            var Bcg = ArrOp.ArrayToUint8(ArrOp.ArrayMultByConst(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Bc), gamma), c)); ;

            image = Helpers.SetPixels(image, Rcg, Gcg, Bcg);

            outName = Directory.GetCurrentDirectory() + "\\Rand\\GammaCorrection.jpg";
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static void Mirror(Bitmap img, Mir direction)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            int[,] resultR = new int[height, width];
            int[,] resultG = new int[height, width];
            int[,] resultB = new int[height, width];
            string outName = String.Empty;

            padMyArray<int> padArr;
            padArr = new padMyArray<int>();

            if (direction.ToString() == "left" || direction.ToString() == "right")
            {
                resultR = padArr.padArray(Rc, 0, width, PadType.symmetric, Direction.pre);
                resultG = padArr.padArray(Gc, 0, width, PadType.symmetric, Direction.pre);
                resultB = padArr.padArray(Bc, 0, width, PadType.symmetric, Direction.pre);

                if (direction.ToString() == "left")
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\mirrorLeft.jpg";
                }
                else
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\mirrorRight.jpg";
                }
            }
            else
            {
                resultR = padArr.padArray(Rc, height, 0, PadType.symmetric, Direction.pre);
                resultG = padArr.padArray(Gc, height, 0, PadType.symmetric, Direction.pre);
                resultB = padArr.padArray(Bc, height, 0, PadType.symmetric, Direction.pre);

                if (direction.ToString() == "top")
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\mirrorTop.jpg";
                }
                else
                {
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\mirrorBot.jpg";
                }
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static void SaveImageInOtherFormat(Bitmap image, string name, SupportFormats newFormat)
        {
            string outName = name + "." + newFormat.ToString();
            //string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + name + "." + newFormat.ToString();
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
