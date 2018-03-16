using System;
using System.IO;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;

namespace Image
{
    public static class MoreHelpers
    {
        //obtain pixels values of 8bit image correctly
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

        //no comments
        public static Bitmap Narko8bppPalette(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed); 
           
            int r, ic, oc, bmpStride, outputStride;            
            BitmapData bmpData, outputData;
            
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
                Console.WriteLine("Problem at method - Narko8bppPalette: " + e.Message);
            }
            finally
            {
                //Unlock the imagess
                img.UnlockBits(bmpData);
                image.UnlockBits(outputData);
            }

            return image;          
        }

        //set separate color plane
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

        //obtain array of BW(gray) data for some functions
        public static int[,] BlackandWhiteProcessHelper(Bitmap img)
        {
            int[,] empty = new int[1, 1];
            int[,] im = new int[img.Height, img.Width];
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (img.Height < 3 || img.Width < 3)
            {
                Console.WriteLine("Bad input. Image less then filter 3x3");
                return empty;
            }
            else if (Depth == 8)
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
            else if(Depth == 1)
            {
                var ColorList = Helpers.GetPixels(img);
                im = ColorList[0].Color;
            }
            else
            {
                Console.WriteLine("Bad input. Image didn`t 8bit BW or 24bit RGB/BW");
                return empty;
            }

            return im;
        }
         
        //invert binary helepr
        public static int[,] InvertBinaryArray(int[,] arr, [CallerMemberName]string callName = "")
        {            
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];

            if (arr.Cast<int>().Max() == 1)
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                        if(arr[i,j] == 1)
                            result[i, j] = 0;
                        else
                            result[i, j] = 1;
                    }
                }
            }
                
            else if (arr.Cast<int>().Max() == 255)
            {
                for (int i = 0; i < arr.GetLength(0); i++)
                {
                    for (int j = 0; j < arr.GetLength(1); j++)
                    {
                        if (arr[i, j] == 255)
                            result[i, j] = 0;
                        else
                            result[i, j] = 255;
                    }
                }
            }
            else
            {
                Console.WriteLine("May be non-binary input. Method: " + callName);
            }

            return result;
        }               

        #region write image to file in use methods
        //write to file for some methods
        public static void WriteImageToFile(int[,] r, int[,] g, int[,] b, string outName, OutType type)
        {
            string ImgExtension = Path.GetExtension(outName).ToLower();            

            if (r.Length != g.Length || r.Length != b.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> WriteImageToFile(int[,] R, int[,] G, int[,] B, string fileName, OutType type) <-");
            }
            else
            {
                Bitmap image = new Bitmap(r.GetLength(1), g.GetLength(0), PixelFormat.Format24bppRgb);
                image = Helpers.SetPixels(image, r, g, b);

                if (type == OutType.OneBpp)               
                    image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5);
                
                else if (type == OutType.EightBpp)                
                    image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image);                            

                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }

        public static void WriteImageToFile(int[,] r, int[,] g, int[,] b, string fileName, string directoryName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName            = Path.GetFileNameWithoutExtension(fileName);
            Checks.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            if (r.Length != g.Length || r.Length != b.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in hsv2rgb operation -> WriteImageToFile(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                Bitmap image = new Bitmap(r.GetLength(1), g.GetLength(0), PixelFormat.Format24bppRgb);
                image = Helpers.SetPixels(image, r, g, b);

                string outName = Directory.GetCurrentDirectory() + "\\" + directoryName + "\\" + fileName + ImgExtension;
               
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }
        #endregion        
    }
}
