using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    public static class SaltPepperFilter
    {
        private static List<string> FilterVariants = new List<string>()
        { "_ameanspFilt", "_gmeanspFilt", "_hmeanspFilt", "_chmeanspFilt" };

        //SPFILT Performs linear and nonlinear spatial filtering for salt and pepper noize
        //unsharp for chmean
        public static void SaltPepperfilter(Bitmap img, int m, int n, double fOrder, SaltPepperfilterType spfiltType, bool unsharp)
        {
            SaltPepperFilterHelper(img, m, n, fOrder, spfiltType, unsharp);
        }

        public static Bitmap SaltPepperfilterBitmap(Bitmap img, int m, int n, double fOrder, SaltPepperfilterType spfiltType, bool unsharp)
        {
            return SaltPepperFilterProcess(img, m, n, fOrder, spfiltType, unsharp);          
        }

        public static void SaltPepperfilter(Bitmap img, int m, int n, SaltPepperfilterType spfiltType)
        {
            //stupid reinsurance
            if (spfiltType == SaltPepperfilterType.chmean)
            {
                SaltPepperFilterHelper(img, m, n, 1.5, spfiltType, false);
                SaltPepperFilterHelper(img, m, n, -1.5, spfiltType, false);
            }
            else
                SaltPepperFilterHelper(img, m, n, 0, spfiltType, false);
        }      

        private static void SaltPepperFilterHelper(Bitmap img, int m, int n, double fOrder, SaltPepperfilterType spfiltType, bool unsharp)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("SaltPepper");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
        
            string outName = defPath + imgName + FilterVariants.ElementAt((int)spfiltType) + imgExtension;

            Helpers.SaveOptions(image, outName, imgExtension);           
        }

        private static Bitmap SaltPepperFilterProcess(Bitmap img, int m, int n, double fOrder, SaltPepperfilterType spfiltType, bool unsharp)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            double[,] filter;
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            ArrGen<double> arrGen = new ArrGen<double>();
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (m >= 1 && n >= 1)
            {
                switch (spfiltType)
                {
                    //Arithmetic mean filtering.
                    //help with salt noize
                    case SaltPepperfilterType.amean:
                        filter = ImageFilter.FspecialSize(m, n, "average");

                        resultR = (ImageFilter.Filter_double(Rc, filter)).ArrayToUint8();
                        resultG = (ImageFilter.Filter_double(Gc, filter)).ArrayToUint8();
                        resultB = (ImageFilter.Filter_double(Bc, filter)).ArrayToUint8();
                        break;

                    //Geometric mean filtering.
                    //help with salt noize
                    case SaltPepperfilterType.gmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_gmean = (ImageFilter.Filter_double((Rc.ImageUint8ToDouble().LogArrayElements()), filter, PadType.replicate)).ExpArrayElements();
                        var g_gmean = (ImageFilter.Filter_double((Gc.ImageUint8ToDouble().LogArrayElements()), filter, PadType.replicate)).ExpArrayElements();
                        var b_gmean = (ImageFilter.Filter_double((Bc.ImageUint8ToDouble().LogArrayElements()), filter, PadType.replicate)).ExpArrayElements();

                        resultR = r_gmean.PowArrayElements(((double)1 / (double)m / (double)n)).ImageArrayToUint8();
                        resultG = g_gmean.PowArrayElements(((double)1 / (double)m / (double)n)).ImageArrayToUint8();
                        resultB = b_gmean.PowArrayElements(((double)1 / (double)m / (double)n)).ImageArrayToUint8();
                        break;

                    //harmonic mean filter
                    //help with salt noize
                    case SaltPepperfilterType.hmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_harmean = ImageFilter.Filter_double(Rc.ImageUint8ToDouble().ArraySumWithConst((2.2204 
                            * Math.Pow(10, -16))).ConstDivByArrayElements(1), filter, PadType.replicate);
                        var g_harmean = ImageFilter.Filter_double(Gc.ImageUint8ToDouble().ArraySumWithConst((2.2204 
                            * Math.Pow(10, -16))).ConstDivByArrayElements(1), filter, PadType.replicate);
                        var b_harmean = ImageFilter.Filter_double(Bc.ImageUint8ToDouble().ArraySumWithConst((2.2204 
                            * Math.Pow(10, -16))).ConstDivByArrayElements(1), filter, PadType.replicate);

                        resultR = r_harmean.ConstDivByArrayElements(((double)m * (double)n)).ImageArrayToUint8();
                        resultG = g_harmean.ConstDivByArrayElements(((double)m * (double)n)).ImageArrayToUint8();
                        resultB = b_harmean.ConstDivByArrayElements(((double)m * (double)n)).ImageArrayToUint8();
                        break;

                    //contraharmonic mean filter Q>0 for pepper & <0 for salt
                    case SaltPepperfilterType.chmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_charmean = ImageFilter.Filter_double(Rc.ImageUint8ToDouble().PowArrayElements((fOrder + 1)), filter, PadType.replicate);
                        var g_charmean = ImageFilter.Filter_double(Gc.ImageUint8ToDouble().PowArrayElements((fOrder + 1)), filter, PadType.replicate);
                        var b_charmean = ImageFilter.Filter_double(Bc.ImageUint8ToDouble().PowArrayElements((fOrder + 1)), filter, PadType.replicate);

                        r_charmean = r_charmean.ArraydivElements(ImageFilter.Filter_double(Rc.ImageUint8ToDouble().PowArrayElements(fOrder), filter, PadType.replicate));
                        g_charmean = g_charmean.ArraydivElements(ImageFilter.Filter_double(Gc.ImageUint8ToDouble().PowArrayElements(fOrder), filter, PadType.replicate));
                        b_charmean = b_charmean.ArraydivElements(ImageFilter.Filter_double(Bc.ImageUint8ToDouble().PowArrayElements(fOrder), filter, PadType.replicate));

                        resultR = r_charmean.ImageArrayToUint8();
                        resultG = g_charmean.ImageArrayToUint8();
                        resultB = b_charmean.ImageArrayToUint8();
                        break;

                    default:
                        resultR = Rc; resultG = Gc; resultB = Bc;
                        break;
                }

                image = Helpers.SetPixels(image, resultR, resultG, resultB);

                if (unsharp)  //spfiltType == SaltPepperfilterType.chmean & unsharp
                {
                    image = Helpers.FastSharpImage(image);
                }             

                if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
                if (Depth == 1) { image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5); }
            }
            else
            {
                Console.WriteLine("m and n parameters must be positive geater or equal 1. Recommended 2 & 2 and higher. Method >SaltandPapperFilter<");
            }

            return image;
        }
    }

    public enum SaltPepperfilterType
    {
        amean,
        gmean,
        hmean,
        chmean
    }
}
