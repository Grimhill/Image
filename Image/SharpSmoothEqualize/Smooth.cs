using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;
using Image.ColorSpaces;

namespace Image
{
    public static class Smoothing
    {
        private static List<string> SmoothVariants = new List<string>()
        { "_SmoothRGB", "_SmoothHSV", "_SmoothLab", "_SmoothfakeCIE1976L" };

        //image smoothing by entered size for average filter and save to file
        public static void Smooth(Bitmap img, int m, int n, SmoothInColorSpace cSpace)
        {        
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Sharp\\Smooth");
           
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            image = SmoothHelper(img, m, n, cSpace);

            string outName = defPath + imgName + SmoothVariants.ElementAt((int)cSpace) + imgExtension;
            
            Helpers.SaveOptions(image, outName, imgExtension);           
        }

        //image smoothing by entered size for average filter and return bitmap
        public static Bitmap SmoothBitmap(Bitmap img, int m, int n, SmoothInColorSpace cSpace)
        {
            return SmoothHelper(img, m, n, cSpace);
        }

        //image smoothing by entered size for average filter process
        private static Bitmap SmoothHelper(Bitmap img, int m, int n, SmoothInColorSpace cSpace)
        {        
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            List<ArraysListInt> Result = new List<ArraysListInt>();
            double[,] filter;

            if (!Checks.BinaryInput(img))
            {
                List<ArraysListInt> ColorList = Helpers.GetPixels(img);

                if (m >= 1 && n >= 1)
                {
                    //create average filter by entered size
                    filter = ImageFilter.FspecialSize(m, n, "average");

                    //smooth in choosen color space
                    switch (cSpace)
                    {
                        case SmoothInColorSpace.RGB:
                            if (Depth == 8)
                            {
                                var bw = ImageFilter.Filter_double(ColorList[0].Color, filter).ArrayToUint8();
                                Result.Add(new ArraysListInt() { Color = bw }); Result.Add(new ArraysListInt() { Color = bw });
                                Result.Add(new ArraysListInt() { Color = bw });
                            }
                            else
                            {
                                Result.Add(new ArraysListInt() { Color = ImageFilter.Filter_double(ColorList[0].Color, filter).ArrayToUint8() });
                                Result.Add(new ArraysListInt() { Color = ImageFilter.Filter_double(ColorList[1].Color, filter).ArrayToUint8() });
                                Result.Add(new ArraysListInt() { Color = ImageFilter.Filter_double(ColorList[2].Color, filter).ArrayToUint8() });
                            }
                            break;

                        case SmoothInColorSpace.HSV:
                            var hsv = RGBandHSV.RGB2HSV(img);
                            var hsv_temp = ImageFilter.Filter_double(hsv[2].Color, filter, PadType.replicate);

                            //Filter by V - Value (Brightness/яркость)
                            //artificially if V > 1; make him 1
                            Result = RGBandHSV.HSV2RGB(hsv[0].Color, hsv[1].Color, hsv_temp.ToBorderGreaterZero(1));
                            break;

                        case SmoothInColorSpace.Lab:
                            var lab = RGBandLab.RGB2Lab(img);
                            var lab_temp = ImageFilter.Filter_double(lab[0].Color, filter, PadType.replicate);

                            //Filter by L - lightness                    
                            Result = RGBandLab.Lab2RGB(lab_temp.ToBorderGreaterZero(255), lab[1].Color, lab[2].Color);
                            break;

                        case SmoothInColorSpace.fakeCIE1976L:
                            var fakeCIE1976L = RGBandLab.RGB2Lab1976(img);
                            var fakeCIE1976L_temp = ImageFilter.Filter_double(fakeCIE1976L[0].Color, filter, PadType.replicate);

                            //Filter by L - lightness                    
                            Result = RGBandLab.Lab1976toRGB(fakeCIE1976L_temp, fakeCIE1976L[1].Color, fakeCIE1976L[2].Color);
                            break;
                    }

                    image = Helpers.SetPixels(image, Result[0].Color, Result[1].Color, Result[2].Color);

                    if (Depth == 8)
                    { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
                }
                else
                {
                    Console.WriteLine("m and n parameters must be positive and greater or equal 1. Recommended 2 & 2 and higher. Method >Smooth<. Return black square.");
                }
            }
            else { Console.WriteLine("What did you expected to smooth binaty image? Return black square."); }

            return image;
        }
    }

    public enum SmoothInColorSpace
    {
        RGB,
        HSV,
        Lab,
        fakeCIE1976L
    }
}
