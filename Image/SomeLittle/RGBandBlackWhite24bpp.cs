using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    //Some operations with RGB and 24 BW
    public static class RGBandBlackWhite24bpp
    {
        //save image from R G B arrays and save to file
        public static void RGBArraysToFile(int[,] r, int[,] g, int[,] b)
        {
            string defPath = GetImageInfo.MyPath("Rand");

            int height = r.GetLength(0);
            int width  = r.GetLength(1);

            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            if (r.GetLength(0) != g.GetLength(0) || r.GetLength(0) != b.GetLength(0)
                || r.GetLength(1) != g.GetLength(1) || r.GetLength(1) != b.GetLength(1))
            {
                Console.WriteLine("Array dimentions dismatch in operation. Method: RGBArrayToImage");
            }
            else if (r.Cast<int>().Min() < 0 || g.Cast<int>().Min() < 0 || b.Cast<int>().Min() < 0)
            {
                Console.WriteLine("One of RGB array contain negativ vaules. Method: RGBArrayToImage");
            }
            else
            {
                image = Helpers.SetPixels(image, r, g, b);
                Helpers.SaveOptions(image, defPath + "rgbArrayToImage.jpg", ".jpg");                
            }
        }

        //form bitmap from R G B arrays
        public static Bitmap RGBArraysToBitmap(int[,] r, int[,] g, int[,] b)
        {
            int height = r.GetLength(0);
            int width  = r.GetLength(1);

            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            if (r.GetLength(0) != g.GetLength(0) || r.GetLength(0) != b.GetLength(0)
                || r.GetLength(1) != g.GetLength(1) || r.GetLength(1) != b.GetLength(1))           
                Console.WriteLine("Array dimentions dismatch in operation. Method: RGBArrayToImage");
            
            else if (r.Cast<int>().Min() < 0 || g.Cast<int>().Min() < 0 || b.Cast<int>().Min() < 0)            
                Console.WriteLine("One of RGB array contain negativ vaules. Method: RGBArrayToImage");
            
            else            
                image = Helpers.SetPixels(image, r, g, b);                
           
            return image;
        }

        //convert RGB image to 24bpp BW and save to file
        public static void RGBtoBlackWhite24bpp(Bitmap img)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            if (Checks.RGBinput(img))
            {
                var gray = Helpers.RGBToGrayArray(img);
                image = Helpers.SetPixels(image, gray, gray, gray);

                string outName = defPath + "_rgbToGray24bpp" + imgExtension;
                Helpers.SaveOptions(img, outName, imgExtension);
            }
        }

        //convert RGB image to 24bpp BW and return bitmap object
        public static Bitmap RGBtoBlackWhite24bppBitmap(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            if (Checks.RGBinput(img))
            {
                var gray = Helpers.RGBToGrayArray(img);
                image = Helpers.SetPixels(image, gray, gray, gray);
            }

            return image;
        }
    }
}
