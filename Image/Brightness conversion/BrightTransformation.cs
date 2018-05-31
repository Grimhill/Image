using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    public static class BrightTransformation
    {
        private static List<string> StretchContrastVariant = new List<string>()
        { "_StretchContrastR", "_StretchContrastG", "_StretchContrastB", "_StretchContrast" };

        private static List<string> GammaVariant = new List<string>()
        { "_GammaCorrectionR", "_GammaCorrectionG", "_GammaCorrectionB", "_GammaCorrection" };

        private static List<string> LogTransVariant = new List<string>()
        { "_LogTransR", "_LogTransG", "_LogTransB", "_LogTrans" };

        // S = 1/(1 + (m/r)^E); r - image array, m - coef, E - controls function incline. m & E > 0
        //m recomended average value of the color plane, E 4-5
        public static void StretchContrast(Bitmap img, BrightConvPlane plane, double m, double e)
        {
            BrightTransformationShapka(img, "stretch", plane, m, e);
        }

        public static Bitmap StretchContrastBitmap(Bitmap img, BrightConvPlane plane, double m, double e)
        {
            return BrightTransformationHelper(img, "stretch", plane, m, e);
        }

        //s = c * log(1 + r), r - pixel values. c - constant, c > 0
        public static void LogTransformation(Bitmap img, BrightConvPlane plane, double multCoeff)
        {
            BrightTransformationShapka(img, "log", plane, multCoeff, 0);
        }

        public static Bitmap LogTransformationBitmap(Bitmap img, BrightConvPlane plane, double multCoeff)
        {
            return BrightTransformationHelper(img, "log", plane, multCoeff, 0);
        }

        //s = c * r^gamma, r - pixel values. c & gama - const, c & gamma > 0
        public static void GammaCorrection(Bitmap img, BrightConvPlane plane, double multCoeff, double gamma)
        {
            BrightTransformationShapka(img, "gamma", plane, multCoeff, gamma);
        }

        public static Bitmap GammaCorrectionBitmap(Bitmap img, BrightConvPlane plane, double multCoeff, double gamma)
        {
            return BrightTransformationHelper(img, "gamma", plane, multCoeff, gamma);
        }

        //constOne - c for gamma\log (multCoeff); m for stretch
        //constTwo - e for stretch; gamma for gammaCorr :o
        private static void BrightTransformationShapka(Bitmap img, string operation, BrightConvPlane plane, double constOne, double constTwo)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("BrightConv");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            string outName = string.Empty;

            image = BrightTransformationHelper(img, operation, plane, constOne, constTwo);

            int index = 0;
            foreach (var n in Enum.GetValues(typeof(BrightConvPlane)).Cast<BrightConvPlane>())
            {
                if (Depth == 8) { index = 3; break; }//ugly
                if (n == plane) { index = (int)n; break; };
            }

            if (operation == "stretch")
                outName = defPath + imgName + StretchContrastVariant.ElementAt(index) + "_m_" + constOne + "_E_" + constTwo + imgExtension;
            else if(operation == "log")
                outName = defPath + imgName + LogTransVariant.ElementAt(index) + "_c_" + constOne + imgExtension;
            else if (operation == "gamma")
                outName = defPath + imgName + GammaVariant.ElementAt(index) + "_c_" + constOne + "_gamma_" + constTwo + imgExtension;

            Helpers.SaveOptions(image, outName, imgExtension);
        }

        private static Bitmap BrightTransformationHelper(Bitmap img, string operation, BrightConvPlane plane, double constOne, double constTwo)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);

            if (constOne > 0 && constTwo > 0)
            {
                if (!Checks.BinaryInput(img))
                {
                    if (Depth == 8)
                    {
                        if (operation == "stretch")                        
                            resultR = ColorList[0].Color.StretchContrastFunc(constOne, constTwo);                        
                        else if (operation == "log")
                            resultR = ColorList[0].Color.LogTransformationFunc(constOne);
                        else if (operation == "gamma")
                            resultR = ColorList[0].Color.GammaCorrectionFunc(constOne, constTwo);

                        resultG = resultR; resultB = resultR;
                    }
                    else
                    {
                        switch (plane)
                        {
                            case BrightConvPlane.Rplane: //R plane
                                if (operation == "stretch")
                                    resultR = ColorList[0].Color.StretchContrastFunc(constOne, constTwo);   
                                else if (operation == "log")
                                    resultR = ColorList[0].Color.LogTransformationFunc(constOne);
                                else if (operation == "gamma")
                                    resultR = ColorList[0].Color.GammaCorrectionFunc(constOne, constTwo);

                                resultG = ColorList[1].Color; resultB = ColorList[2].Color;
                                break;

                            case BrightConvPlane.Gplane: //G plane 
                                if (operation == "stretch")
                                    resultG = ColorList[1].Color.StretchContrastFunc(constOne, constTwo);
                                else if (operation == "log")
                                    resultG = ColorList[1].Color.LogTransformationFunc(constOne);
                                else if (operation == "gamma")
                                    resultG = ColorList[1].Color.GammaCorrectionFunc(constOne, constTwo);

                                resultR = ColorList[0].Color; resultB = ColorList[2].Color;
                                break;

                            case BrightConvPlane.Bplane: //B plane
                                if (operation == "stretch")
                                    resultB = ColorList[2].Color.StretchContrastFunc(constOne, constTwo);
                                else if (operation == "log")
                                    resultB = ColorList[2].Color.LogTransformationFunc(constOne);
                                else if (operation == "gamma")
                                    resultB = ColorList[2].Color.GammaCorrectionFunc(constOne, constTwo);

                                resultR = ColorList[0].Color; resultG = ColorList[1].Color;
                                break;

                            case BrightConvPlane.RGB:
                                if (operation == "stretch")
                                {
                                    resultR = ColorList[0].Color.StretchContrastFunc(constOne, constTwo);
                                    resultG = ColorList[1].Color.StretchContrastFunc(constOne, constTwo);
                                    resultB = ColorList[2].Color.StretchContrastFunc(constOne, constTwo);
                                }
                                else if (operation == "log")
                                {
                                    resultR = ColorList[0].Color.LogTransformationFunc(constOne);
                                    resultG = ColorList[1].Color.LogTransformationFunc(constOne);
                                    resultB = ColorList[2].Color.LogTransformationFunc(constOne);

                                }
                                else if (operation == "gamma")
                                {
                                    resultR = ColorList[0].Color.GammaCorrectionFunc(constOne, constTwo);
                                    resultG = ColorList[1].Color.GammaCorrectionFunc(constOne, constTwo);
                                    resultB = ColorList[2].Color.GammaCorrectionFunc(constOne, constTwo);

                                }
                                break;
                        }
                    }

                    image = Helpers.SetPixels(image, resultR, resultG, resultB);

                    if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
                }
                else { Console.WriteLine("What did you expected to make log transformation with binary image? Return black square."); }
            }
            else { Console.WriteLine("Input constants must be positive value. Return black square."); }

            return image;
        }


        private static int[,] StretchContrastFunc(this int[,] x, double m, double e)
        {
            return x.ArrayToDouble().ArraySumWithConst((2.2204 * Math.Pow(10, -16))).ConstDivByArrayElements(m).
                PowArrayElements(e).ArraySumWithConst(1).ConstDivByArrayElements(1).ImageDoubleToUint8();
        }

        private static int[,] LogTransformationFunc(this int[,] x, double multCoeff)
        {
            return x.ArrayToDouble().ArraySumWithConst(1).LogArrayElements().ArrayMultByConst(multCoeff).ArrayToUint8();
        }

        private static int[,] GammaCorrectionFunc(this int[,] x, double multCoeff, double gamma)
        {
            return x.ArrayToDouble().PowArrayElements(gamma).ArrayMultByConst(multCoeff).ArrayToUint8();
        }        
    }

    public enum BrightConvPlane
    {
        Rplane,
        Gplane,
        Bplane,
        RGB
    }
}
