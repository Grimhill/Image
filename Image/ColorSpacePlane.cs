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
            string defPass = Directory.GetCurrentDirectory() + "\\ColorSpace\\ColorSpacePlane\\";
            Checks.DirectoryExistance(defPass);

            Bitmap image  = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            Bitmap outRes = new Bitmap(img.Width, img.Height, PixelFormat.Format8bppIndexed);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();
            string outName = String.Empty;

            if (Checks.NonRGBinput(img))
            {
                //obtain color components
                var ColorList  = Helpers.GetPixels(img);
                var Redcolor   = ColorList[0].Color;
                var Greencolor = ColorList[1].Color;
                var Bluecolor  = ColorList[2].Color;

                switch (colorPlane)
                {
                    case ColorPlaneRGB.R:
                        image = MoreHelpers.SetColorPlanePixels(image, Redcolor, ColorPlaneRGB.R);

                        outName = defPass + fileName + "_Rcplane" + ImgExtension;
                        break;

                    case ColorPlaneRGB.G:
                        image = MoreHelpers.SetColorPlanePixels(image, Greencolor, ColorPlaneRGB.G);

                        outName = defPass + fileName + "_Gcplane" + ImgExtension;
                        break;

                    case ColorPlaneRGB.B:
                        image = MoreHelpers.SetColorPlanePixels(image, Bluecolor, ColorPlaneRGB.B);

                        outName = defPass + fileName + "_Bcplane" + ImgExtension;
                        break;

                    case ColorPlaneRGB.RGB:
                        image = MoreHelpers.SetColorPlanePixels(image, Redcolor, ColorPlaneRGB.R);
                        outName = Checks.OutputFileNames(defPass + fileName + "_Rcplane" + ImgExtension);
                        image.Save(outName);

                        image = MoreHelpers.SetColorPlanePixels(image, Greencolor, ColorPlaneRGB.G);
                        outName = Checks.OutputFileNames(defPass + fileName + "_Gcplane" + ImgExtension);
                        image.Save(outName);

                        image = MoreHelpers.SetColorPlanePixels(image, Bluecolor, ColorPlaneRGB.B);
                        outName = Checks.OutputFileNames(defPass + fileName + "_Bcplane" + ImgExtension);
                        image.Save(outName);
                        break;

                    case ColorPlaneRGB.Rnarkoman:
                        image = MoreHelpers.SetColorPlanePixels(image, Redcolor, ColorPlaneRGB.R);
                        outRes = MoreHelpers.Narko8bppPalette(image);

                        outName = Checks.OutputFileNames(defPass + fileName + "_RcplaneNarkoman" + ImgExtension);
                        outRes.Save(outName);
                        break;

                    case ColorPlaneRGB.Gnarkoman:
                        image = MoreHelpers.SetColorPlanePixels(image, Greencolor, ColorPlaneRGB.G);
                        outRes = MoreHelpers.Narko8bppPalette(image);

                        outName = Checks.OutputFileNames(defPass + fileName + "_GcplaneNarkoman" + ImgExtension);
                        outRes.Save(outName);
                        break;

                    case ColorPlaneRGB.Bnarkoman:
                        image = MoreHelpers.SetColorPlanePixels(image, Bluecolor, ColorPlaneRGB.B);
                        outRes = MoreHelpers.Narko8bppPalette(image);

                        outName = Checks.OutputFileNames(defPass + fileName + "_BcplaneNarkoman" + ImgExtension);
                        outRes.Save(outName);
                        break;

                    default:

                        break;
                }

                if (colorPlane != ColorPlaneRGB.RGB && colorPlane != ColorPlaneRGB.Rnarkoman &&
                    colorPlane != ColorPlaneRGB.Gnarkoman && colorPlane != ColorPlaneRGB.Bnarkoman)
                {
                    outName = Checks.OutputFileNames(outName);
                    image.Save(outName);
                }
                //Helpers.SaveOptions(image, outName, ImgExtension);
            }
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
