using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using Image.ArrayOperations;

namespace Image
{
    public static class MoreHelpers
    {
        //Correction for input 8bit or one of planes of color image
        public static int[,] GammaCorrection(int[,] cPlane, double c, double gamma)
        {
            int[,] result = new int[cPlane.GetLength(0), cPlane.GetLength(1)];

            //higher c and gamma - lighter image after correction            
            result = cPlane.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(c).ArrayToUint8();

            return result;
        }

        public static int[,] Obtain8bppdata(Bitmap img)
        {
            int[,] pixelData = new int[img.Height, img.Width];

            var data = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);

            try
            {
                unsafe
                {
                    byte* bmpPtr = (byte*)data.Scan0;
                    for (int y = 0; y < img.Height; y++)
                    {
                        for (int x = 0; x < img.Width; x++)
                        {
                            pixelData[y, x] = (bmpPtr[x + y * data.Stride]);
                        }
                    }
                    img.UnlockBits(data);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem at method - Obtain8bppdata: " + e.Message);
            }

            return pixelData;
        }

        public static void Bbp24Gray2Gray8bpp(Bitmap img, string fileName)
        {
            Checks.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);
            image = Bbp24Gray2Gray8bppHelper(img);
            
            string outName = Checks.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_24bppGray28bppIndexed" + ImgExtension);
            image.Save(outName);
        }

        //can use independently
        public static Bitmap Bbp24Gray2Gray8bppHelper(Bitmap img)
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

            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem at method - Bbp24Gray2Gray8bpp: " + e.Message);
            }
            finally
            {
                //Unlock the imagess
                img.UnlockBits(bmpData);
                image.UnlockBits(outputData);
            }

            return image;
        }

        public static Bitmap Narko8bppPalette(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            int r, ic, oc, bmpStride, outputStride;
            BitmapData bmpData, outputData;

            //Lock the images
            bmpData = img.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadOnly, img.PixelFormat);
            outputData = image.LockBits(new Rectangle(0, 0, img.Width, img.Height), ImageLockMode.WriteOnly, PixelFormat.Format8bppIndexed);
            bmpStride = bmpData.Stride;
            outputStride = outputData.Stride;

            try
            {
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem at method - Bbp24Gray2Gray8bppPalette: " + e.Message);
            }
            finally
            {
                //Unlock the imagess
                img.UnlockBits(bmpData);
                image.UnlockBits(outputData);
            }

            return image;
        }

        public static Bitmap SetColorPlanePixels(Bitmap image, int[,] colorPlane, ColorPlaneRGB plane)
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
                Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> SetColorPlanePixels <-");
            }
            return image;
        }

        public static int[,] BlackandWhiteProcessHelper(Bitmap img)
        {
            int[,] empty = new int[1, 1];
            int[,] im = new int[img.Height, img.Width];
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (Depth == 8)
            {
                im = MoreHelpers.Obtain8bppdata(img);
            }
            else if (Depth == 24 || Depth == 32)
            {
                if (Checks.BlackandWhite24bppCheck(img))
                {
                    var ColorList = Helpers.GetPixels(img);
                    im = ColorList[0].Color;
                }
                else
                {
                    im = Helpers.RGBToGrayArray(img);
                }
            }
            else if (img.Height <= 3 || img.Width < 3)
            {
                Console.WriteLine("Bad input. Image less then filter 3x3");
                return empty;
            }
            else
            {
                Console.WriteLine("Bad input. Image didn`t 8bit BW or 24bit RGB/BW");
                return empty;
            }

            return im;
        }
    }
}
