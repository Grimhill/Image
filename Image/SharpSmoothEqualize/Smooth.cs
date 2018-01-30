using Image.ArrayOperations;
using Image.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Image.SharpSmoothEqualize
{
    public static class Smoothing
    {
        //only for RGB images, b&w 24bbp.
        public static void Smooth(Bitmap img, int m, int n, SmoothInColorSpace cSpace, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string defPass = Directory.GetCurrentDirectory() + "\\Sharp\\Smooth\\";
            Checks.DirectoryExistance(defPass);

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            List<ArraysListInt> Result = new List<ArraysListInt>();

            double[,] filter;
            string outName = String.Empty;

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (Depth == 8) { ImgExtension = ".png"; }

            if (m > 0 && n > 0)
            {

                filter = Filter.Fspecial(m, n, "average");

                switch (cSpace)
                {
                    case SmoothInColorSpace.RGB:
                        Result.Add(new ArraysListInt() { Color = Filter.Filter_double(ColorList[0].Color, filter).ArrayToUint8() });
                        Result.Add(new ArraysListInt() { Color = Filter.Filter_double(ColorList[1].Color, filter).ArrayToUint8() });
                        Result.Add(new ArraysListInt() { Color = Filter.Filter_double(ColorList[2].Color, filter).ArrayToUint8() });

                        outName = defPass + fileName + "_SmoothRGB" + ImgExtension;
                        break;

                    case SmoothInColorSpace.HSV:
                        var hsv = RGBandHSV.RGB2HSV(img);
                        var hsv_temp = Filter.Filter_double(hsv[2].Color, filter, PadType.replicate);

                        //Filter by V - Value (Brightness/яркость)
                        //artificially if V > 1, make him 1
                        Result = RGBandHSV.HSV2RGB(hsv[0].Color, hsv[1].Color, hsv_temp.ToBorderGreaterZero(1));

                        outName = defPass + fileName + "_SmoothHSV" + ImgExtension;
                        break;

                    case SmoothInColorSpace.Lab:
                        var lab = RGBandLab.RGB2Lab(img);
                        var lab_temp = Filter.Filter_double(lab[0].Color, filter, PadType.replicate);

                        //Filter by L - lightness                    
                        Result = RGBandLab.Lab2RGB(lab_temp.ToBorderGreaterZero(255), lab[1].Color, lab[2].Color);

                        outName = defPass + fileName + "_SmoothLab" + ImgExtension;
                        break;

                    case SmoothInColorSpace.fakeCIE1976L:
                        var fakeCIE1976L = RGBandLab.RGB2Lab(img);
                        var fakeCIE1976L_temp = Filter.Filter_double((fakeCIE1976L[0].Color).ArrayMultByConst(2.57), filter, PadType.replicate);

                        //Filter by L - lightness                    
                        Result = RGBandLab.Lab2RGB(fakeCIE1976L_temp.ArrayDivByConst(2.57), fakeCIE1976L[1].Color, fakeCIE1976L[2].Color);

                        outName = defPass + fileName + "_SmoothfakeCIE1976L" + ImgExtension;
                        break;
                }

                image = Helpers.SetPixels(image, Result[0].Color, Result[1].Color, Result[2].Color);
                outName = Checks.OutputFileNames(outName);

                if (Depth == 8)
                { image = MoreHelpers.Bbp24Gray2Gray8bppHelper(image); }

                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
            else
            {
                Console.WriteLine("m and n parameters must be greater, then 0. Recommended 2 & 2 and higher. Method >Smooth<");
            }
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
