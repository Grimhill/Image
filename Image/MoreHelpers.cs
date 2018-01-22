using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    public static class MoreHelpers
    {
        public static List<string> AvailableFormats = new List<string>() { ".jpg", ".jpeg", ".bmp", ".png", ".tif", ".gif" };
        public static bool CheckForInputFormat(string ImgExtension)
        {
            if (!AvailableFormats.Contains(ImgExtension))
            {
                Console.WriteLine("Unsupport image format (extension. Support: jpg, jpeg, bmp, png, tif, gif (only first picture for animation)");
                return false;
            }
            else if (ImgExtension == ".gif")
            {
                Console.WriteLine("Worning! For gif animation take only first picture");
                return true;
            }
            return true;
        }

        public static string OutputFileNames(string fullFilePath)
        {
            if (System.IO.File.Exists(fullFilePath))
            {
                string folder = Path.GetDirectoryName(fullFilePath);
                string filename = Path.GetFileNameWithoutExtension(fullFilePath);
                string extension = Path.GetExtension(fullFilePath);
                int number = 0;

                Match regex = Regex.Match(fullFilePath, @"(.+) \((\d+)\)\.\w+");

                if (regex.Success)
                {
                    filename = regex.Groups[1].Value;
                    number = int.Parse(regex.Groups[2].Value);
                }

                do
                {
                    number++;
                    fullFilePath = Path.Combine(folder, string.Format("{0} ({1}){2}", filename, number, extension));
                }
                while (System.IO.File.Exists(fullFilePath));
            }
            return fullFilePath;

        }

        public static void DirectoryExistance(string path)
        {
            bool exists = Directory.Exists(path);

            if (!exists)
                Directory.CreateDirectory(path);
        }

        public static void Bbp24Gray2Gray8bpp(Bitmap img, string fileName)
        {
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            int r, ic, oc, bmpStride, outputStride;
            //ColorPalette palette;
            BitmapData bmpData, outputData;

            //Build a grayscale color Palette
            ColorPalette palette = image.Palette;
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

                //Note that ic is the input column and oc is the output column
                for (r = 0; r < img.Height; r++)
                    for (ic = oc = 0; oc < img.Width; ic += 3, ++oc)
                        outputPtr[r * outputStride + oc] = (byte)(int)
                        (bmpPtr[r * bmpStride + ic]);

            }

            //Unlock the images
            img.UnlockBits(bmpData);
            image.UnlockBits(outputData);

            string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_24bppGray28bppIndexed" + ImgExtension);
            image.Save(outName);
        }

        public static Bitmap Bbp24Gray2Gray8bpp(Bitmap img)
        {
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

                //Note that ic is the input column and oc is the output column
                for (r = 0; r < img.Height; r++)
                    for (ic = oc = 0; oc < img.Width; ic += 3, ++oc)
                        outputPtr[r * outputStride + oc] = (byte)(int)
                        (bmpPtr[r * bmpStride + ic]);

            }

            //Unlock the images
            img.UnlockBits(bmpData);
            image.UnlockBits(outputData);

            return image;
        }

        public static Bitmap Bbp24Gray2Gray8bppPalette(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            int r, ic, oc, bmpStride, outputStride;            
            BitmapData bmpData, outputData;

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

                //Note that ic is the input column and oc is the output column
                for (r = 0; r < img.Height; r++)
                    for (ic = oc = 0; oc < img.Width; ic += 3, ++oc)
                        outputPtr[r * outputStride + oc] = (byte)(int)
                        (bmpPtr[r * bmpStride + ic] +
                        bmpPtr[r * bmpStride + ic + 1] +
                        bmpPtr[r * bmpStride + ic + 2]);

            }

            //Unlock the images
            img.UnlockBits(bmpData);
            image.UnlockBits(outputData);

            return image;
        }

        public static Bitmap SetRGBPixels(Bitmap image, int[,] colorPlane, ColorPlaneRGB plane)
        {
            try
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int pixel = colorPlane[y, x];

                        if (plane == ColorPlaneRGB.R)
                        {
                            image.SetPixel(x, y, Color.FromArgb(pixel, 0, 0));
                        }
                        else if (plane == ColorPlaneRGB.G)
                        {
                            image.SetPixel(x, y, Color.FromArgb(0, pixel, 0));
                        }
                        else if (plane == ColorPlaneRGB.B)
                        {
                            image.SetPixel(x, y, Color.FromArgb(0, 0, pixel));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> SetRGBPixels <-");
            }
            return image;
        }
    }

    public enum ColorPlaneRGB
    {
        R,
        G,
        B,
        RGB,
        Rnarkoman,
        Gnarkoman,
        Bnarkoman
    }
}
