using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Image
{
    public static class HighContrastFilter
    {
        public static void HighContrastBlackWhite(Bitmap img, ContrastFilter filter)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Contrast\\HighContrastFilter");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = HighContrastProcess(img, filter, HighContastRGB.RGB);

            string outName = defPath + imgName + "_" + filter.ToString() + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static void HighContrastColored(Bitmap img, ContrastFilter filter, HighContastRGB cPlane)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Contrast\\HighContrastFilter");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = HighContrastProcess(img, filter, cPlane);

            string outName = defPath + imgName + "_" + filter.ToString() + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap HighContrastBlackWhiteBitmap(Bitmap img, ContrastFilter filter)
        {
            return HighContrastProcess(img, filter, HighContastRGB.RGB);
        }

        public static Bitmap HighContrastColoredBitmap(Bitmap img, ContrastFilter filter, HighContastRGB cPlane)
        {
            return HighContrastProcess(img, filter, cPlane);
        }


        private static Bitmap HighContrastProcess(Bitmap img, ContrastFilter filter, HighContastRGB cPlane, [CallerMemberName]string callName = "")
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            int[,] filterWindow = new int[3, 3];            

            if (filter == ContrastFilter.filterOne)
                filterWindow = ImageFilter.Ix3FWindow("HighContrast1");
            else
                filterWindow = ImageFilter.Ix3FWindow("HighContrast2");

            if (callName == "HighContrastBlackWhite")
            {
                if (Depth == 8 || Checks.BlackandWhite24bppCheck(img))
                {
                    var GrayC = MoreHelpers.BlackandWhiteProcessHelper(img);

                    resultR = ImageFilter.Filter_int(GrayC, filterWindow, PadType.replicate);
                    resultG = resultR; resultB = resultR;
                }
                else { Console.WriteLine("There non 8bit or 24bit black and white image at input. Method:" + callName); }
            }
            else
            {
                if (Checks.RGBinput(img))
                {
                    List<ArraysListInt> ColorList = Helpers.GetPixels(img);

                    switch(cPlane)
                    {
                        case HighContastRGB.R:
                            resultR = ImageFilter.Filter_int(ColorList[0].Color, filterWindow, PadType.replicate);
                            resultG = ColorList[1].Color; resultB = ColorList[2].Color;
                            break;

                        case HighContastRGB.G:
                            resultG = ImageFilter.Filter_int(ColorList[1].Color, filterWindow, PadType.replicate);
                            resultR = ColorList[0].Color; resultB = ColorList[2].Color;
                            break;

                        case HighContastRGB.B:
                            resultB = ImageFilter.Filter_int(ColorList[2].Color, filterWindow, PadType.replicate);
                            resultR = ColorList[0].Color; resultG = ColorList[1].Color;
                            break;

                        case HighContastRGB.RGB:
                            resultR = ImageFilter.Filter_int(ColorList[0].Color, filterWindow, PadType.replicate);
                            resultG = ImageFilter.Filter_int(ColorList[1].Color, filterWindow, PadType.replicate);
                            resultB = ImageFilter.Filter_int(ColorList[2].Color, filterWindow, PadType.replicate);
                            break;
                    }                    
                }
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);
            if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }  

            return image;
        }
    }

    public enum ContrastFilter
    {
        filterOne,
        filterTwo
    }

    public enum HighContastRGB
    {
        R,
        G,
        B,
        RGB
    }
}
