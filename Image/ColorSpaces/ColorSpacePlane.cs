using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Image
{
    public static class ColorSpacePlane
    {
        //obtain selected Color plane in file or their combo.
        public static void RGBcomponents(Bitmap img, ColorPlaneRGB colorPlane)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("ColorSpace\\ColorSpacePlane");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            Bitmap outRes = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            string outName = String.Empty;

            if (Checks.RGBinput(img))
            {
                //obtain color components
                var ColorList = Helpers.GetPixels(img);
                var Red   = ColorList[0].Color;
                var Green = ColorList[1].Color;
                var Blue  = ColorList[2].Color;

                switch (colorPlane)
                {
                    case ColorPlaneRGB.R:
                        image = MoreHelpers.SetColorPlanePixels(image, Red, ColorPlaneRGB.R);

                        outName = defPath + imgName + "_Rcplane" + imgExtension;
                        break;

                    case ColorPlaneRGB.G:
                        image = MoreHelpers.SetColorPlanePixels(image, Green, ColorPlaneRGB.G);

                        outName = defPath + imgName + "_Gcplane" + imgExtension;
                        break;

                    case ColorPlaneRGB.B:
                        image = MoreHelpers.SetColorPlanePixels(image, Blue, ColorPlaneRGB.B);

                        outName = defPath + imgName + "_Bcplane" + imgExtension;
                        break;

                    case ColorPlaneRGB.RGB:
                        image = MoreHelpers.SetColorPlanePixels(image, Red, ColorPlaneRGB.R);
                        outName = defPath + imgName + "_Rcplane" + imgExtension;
                        Helpers.SaveOptions(image, outName, imgExtension);

                        image = MoreHelpers.SetColorPlanePixels(image, Green, ColorPlaneRGB.G);
                        outName = defPath + imgName + "_Gcplane" + imgExtension;
                        Helpers.SaveOptions(image, outName, imgExtension);

                        image = MoreHelpers.SetColorPlanePixels(image, Blue, ColorPlaneRGB.B);
                        outName = defPath + imgName + "_Bcplane" + imgExtension;
                        Helpers.SaveOptions(image, outName, imgExtension);
                        break;

                    case ColorPlaneRGB.RGcombo:
                        image = Helpers.SetPixels(image, Red, Green, new int[Blue.GetLength(0), Blue.GetLength(1)]);

                        outName = defPath + imgName + "_RGplanes" + imgExtension;
                        break;

                    case ColorPlaneRGB.RBcombo:
                        image = Helpers.SetPixels(image, Red, new int[Blue.GetLength(0), Blue.GetLength(1)], Blue);

                        outName = defPath + imgName + "_RBplanes" + imgExtension;
                        break;

                    case ColorPlaneRGB.GBcombo:
                        image = Helpers.SetPixels(image, new int[Blue.GetLength(0), Blue.GetLength(1)], Green, Blue);

                        outName = defPath + imgName + "_GBplanes" + imgExtension;
                        break;

                    case ColorPlaneRGB.Rnarkoman:
                        image = MoreHelpers.SetColorPlanePixels(image, Red, ColorPlaneRGB.R);
                        outRes = MoreHelpers.Narko8bppPalette(image);

                        outName = defPath + imgName + "_RcplaneNarkoman" + imgExtension;
                        Helpers.SaveOptions(image, outName, imgExtension);
                        break;

                    case ColorPlaneRGB.Gnarkoman:
                        image = MoreHelpers.SetColorPlanePixels(image, Green, ColorPlaneRGB.G);
                        outRes = MoreHelpers.Narko8bppPalette(image);

                        outName = defPath + imgName + "_GcplaneNarkoman" + imgExtension;
                        Helpers.SaveOptions(image, outName, imgExtension);
                        break;

                    case ColorPlaneRGB.Bnarkoman:
                        image = MoreHelpers.SetColorPlanePixels(image, Blue, ColorPlaneRGB.B);
                        outRes = MoreHelpers.Narko8bppPalette(image);

                        outName = defPath + imgName + "_BcplaneNarkoman" + imgExtension;
                        Helpers.SaveOptions(image, outName, imgExtension);
                        break;

                    default:

                        break;
                }

                if (colorPlane != ColorPlaneRGB.RGB && colorPlane != ColorPlaneRGB.Rnarkoman &&
                    colorPlane != ColorPlaneRGB.Gnarkoman && colorPlane != ColorPlaneRGB.Bnarkoman)
                {
                    Helpers.SaveOptions(image, outName, imgExtension);
                }
            }
        }

        //obtain some cplane and return as array
        public static int[,] RGBcomponentArray(Bitmap img, RGBplane colorPlane)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            int[,] cPlane = new int[img.Height, img.Width];
            switch (colorPlane)
            {
                case RGBplane.R:
                    cPlane = ColorList[0].Color;
                    break;

                case RGBplane.G:
                    cPlane = ColorList[1].Color;
                    break;

                case RGBplane.B:
                    cPlane = ColorList[2].Color;
                    break;
            }

            return cPlane;
        } 
    }

    public enum ColorPlaneRGB
    {
        R,
        G,
        B,
        RGB,
        RGcombo,
        RBcombo,
        GBcombo,
        Rnarkoman,
        Gnarkoman,
        Bnarkoman
    }

    public enum RGBplane
    {
        R, G, B
    }
}
