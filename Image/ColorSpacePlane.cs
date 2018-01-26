using Image.ArrayOperations;
using Image.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Image
{
    public static class ColorSpacePlane
    {
        public static void RGBcomponents(Bitmap img, ColorPlaneRGB colorPlane, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace\\ColorSpacePlane");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            Bitmap outRes = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            //obtain color components
            var ColorList  = Helpers.GetPixels(img);
            var Redcolor   = ColorList[0].Color;
            var Greencolor = ColorList[1].Color;
            var Bluecolor  = ColorList[2].Color;

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            string outName = String.Empty;

            switch (colorPlane)
            {
                case ColorPlaneRGB.R:
                    image = MoreHelpers.SetColorPlanePixels(image, Redcolor, ColorPlaneRGB.R);

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_Rcplane" + ImgExtension;
                    break;

                case ColorPlaneRGB.G:
                    image = MoreHelpers.SetColorPlanePixels(image, Greencolor, ColorPlaneRGB.G);

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_Gcplane" + ImgExtension;
                    break;

                case ColorPlaneRGB.B:
                    image = MoreHelpers.SetColorPlanePixels(image, Bluecolor, ColorPlaneRGB.B);

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_Bcplane" + ImgExtension;
                    break;

                case ColorPlaneRGB.RGB:
                    image   = MoreHelpers.SetColorPlanePixels(image, Redcolor, ColorPlaneRGB.R);
                    outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() +
                        "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_Rcplane" + ImgExtension);
                    image.Save(outName);

                    image   = MoreHelpers.SetColorPlanePixels(image, Greencolor, ColorPlaneRGB.G);
                    outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() +
                        "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_Gcplane" + ImgExtension);
                    image.Save(outName);

                    image  = MoreHelpers.SetColorPlanePixels(image, Bluecolor, ColorPlaneRGB.B);
                    outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() +
                        "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_Bcplane" + ImgExtension);
                    image.Save(outName);
                    break;

                case ColorPlaneRGB.Rnarkoman:
                    image  = MoreHelpers.SetColorPlanePixels(image, Redcolor, ColorPlaneRGB.R);
                    outRes = MoreHelpers.Narko8bppPalette(image);

                    outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() +
                        "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_RcplaneNarkoman" + ImgExtension);
                    outRes.Save(outName);
                    break;

                case ColorPlaneRGB.Gnarkoman:
                    image  = MoreHelpers.SetColorPlanePixels(image, Greencolor, ColorPlaneRGB.G);
                    outRes = MoreHelpers.Narko8bppPalette(image);

                    outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() +
                        "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_GcplaneNarkoman" + ImgExtension);
                    outRes.Save(outName);
                    break;

                case ColorPlaneRGB.Bnarkoman:
                    image  = MoreHelpers.SetColorPlanePixels(image, Bluecolor, ColorPlaneRGB.B);
                    outRes = MoreHelpers.Narko8bppPalette(image);

                    outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() +
                        "\\ColorSpace\\ColorSpacePlane\\" + fileName + "_BcplaneNarkoman" + ImgExtension);
                    outRes.Save(outName);
                    break;

                default:

                    break;
            }

            if (colorPlane != ColorPlaneRGB.RGB && colorPlane != ColorPlaneRGB.Rnarkoman &&
                colorPlane != ColorPlaneRGB.Gnarkoman && colorPlane != ColorPlaneRGB.Bnarkoman)
            {
                outName = MoreHelpers.OutputFileNames(outName);
                image.Save(outName);
            }
            //Helpers.SaveOptions(image, outName, ImgExtension);
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

    public enum AnotherColorPlane
    {
        L,
        Lfake
    }
}
