using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

//for salt and pepper noize
namespace Image
{
    public static class SaltPepperFilter
    {
        //SPFILT Performs linear and nonlinear spatial filtering
        public static void SaltPepperfilter(Bitmap img, int m, int n, double Q, SaltPepperfilterType spfiltType, bool unsharp, string fileName)
        {
            //m & n - filter window dimentions (m - row, n - col)
            //Q - filter order Q for Contraharmonic mean
            ArrayOperations ArrOp = new ArrayOperations();
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\SaltPepper");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            double[,] filter;
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            string outName = String.Empty;

            ArrGen<double> arrGen;
            arrGen = new ArrGen<double>();

            if (m < 4 & n < 4)
            {
                switch (spfiltType)
                {
                    //Arithmetic mean filtering.
                    //help with salt noize
                    case SaltPepperfilterType.amean:
                        filter = Filter.Fspecial(m, n, "average");
                        //resultR = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Rc), filter, PadType.replicate));
                        resultR = ArrOp.ArrayToUint8(Filter.Filter_double(Rc, filter));
                        //resultG = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Gc), filter, PadType.replicate));
                        resultG = ArrOp.ArrayToUint8(Filter.Filter_double(Gc, filter));
                        //resultB = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Bc), filter, PadType.replicate));
                        resultB = ArrOp.ArrayToUint8(Filter.Filter_double(Bc, filter));

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_ameanspFilt" + ImgExtension;
                        break;

                    //Geometric mean filtering.
                    //help with salt noize
                    case SaltPepperfilterType.gmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Rc)), filter, PadType.replicate));
                        var g_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Gc)), filter, PadType.replicate));
                        var b_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Bc)), filter, PadType.replicate));

                        resultR = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));
                        resultG = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(b_gmean, ((double)1 / (double)m / (double)n)));
                        resultB = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_gmeanspFilt" + ImgExtension;
                        break;

                    //harmonic mean filter
                    //help with salt noize
                    case SaltPepperfilterType.hmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1,
                            ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Rc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                        var g_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Gc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                        var b_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Bc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);

                        resultR = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, r_harmean));
                        resultG = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, g_harmean));
                        resultB = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, b_harmean));

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_hmeanspFilt" + ImgExtension;
                        break;

                    //contraharmonic mean filter Q>0 for pepper & <0 for salt
                    case SaltPepperfilterType.chmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_charmean = Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Rc), (Q + 1)), filter, PadType.replicate);
                        var g_charmean = Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Gc), (Q + 1)), filter, PadType.replicate);
                        var b_charmean = Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Bc), (Q + 1)), filter, PadType.replicate);

                        r_charmean = ArrOp.ArraydivElements(r_charmean, (Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Rc), Q), filter, PadType.replicate)));
                        g_charmean = ArrOp.ArraydivElements(g_charmean, (Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Gc), Q), filter, PadType.replicate)));
                        b_charmean = ArrOp.ArraydivElements(b_charmean, (Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Bc), Q), filter, PadType.replicate)));

                        resultR = ArrOp.ImageArrayToUint8(r_charmean);
                        resultG = ArrOp.ImageArrayToUint8(g_charmean);
                        resultB = ArrOp.ImageArrayToUint8(b_charmean);

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_chmeanspFilt" + ImgExtension;
                        break;

                    default:
                        resultR = Rc; resultG = Gc; resultB = Bc;

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_wrongspFilt" + ImgExtension;
                        break;
                }

                if (spfiltType == SaltPepperfilterType.chmean & unsharp)
                {
                    var im = Helpers.SetPixels(image, resultR, resultG, resultB);
                    image = Helpers.FastSharpImage(im);

                }
                else
                {
                    image = Helpers.SetPixels(image, resultR, resultG, resultB);
                }

                outName = MoreHelpers.OutputFileNames(outName);

                //dont forget, that directory Contour must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
            else
            {
                Console.WriteLine("Can`t if filter larger, than 3x3, sorry");
            }
        }

        public static void SaltPepperfilter(Bitmap img, int m, int n, SaltPepperfilterType spfiltType, string fileName)
        {
            //m & n - filter window dimentions (m - row, n - col)
            //Q - filter order Q for Contraharmonic mean
            var Q = 1.5; //default

            ArrayOperations ArrOp = new ArrayOperations();
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\SaltPepper");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            double[,] filter;
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            string outName = String.Empty;

            ArrGen<double> arrGen;
            arrGen = new ArrGen<double>();

            switch (spfiltType)
            {
                //Arithmetic mean filtering.
                //help with salt noize
                case SaltPepperfilterType.amean:
                    filter = Filter.Fspecial(m, n, "average");
                    resultR = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Rc), filter, PadType.replicate));
                    resultG = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Gc), filter, PadType.replicate));
                    resultB = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Bc), filter, PadType.replicate));

                    outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_ameanspFilt" + ImgExtension;
                    break;

                //Geometric mean filtering.
                //help with salt noize
                case SaltPepperfilterType.gmean:
                    filter = arrGen.ArrOfSingle(m, n, 1);

                    var r_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Rc)), filter, PadType.replicate));
                    var g_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Gc)), filter, PadType.replicate));
                    var b_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Bc)), filter, PadType.replicate));

                    resultR = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));
                    resultG = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(b_gmean, ((double)1 / (double)m / (double)n)));
                    resultB = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));

                    outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_gmeanspFilt" + ImgExtension;
                    break;

                //harmonic mean filter
                //help with salt noize
                case SaltPepperfilterType.hmean:
                    filter = arrGen.ArrOfSingle(m, n, 1);

                    var r_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Rc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                    var g_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Gc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                    var b_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Bc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);

                    resultR = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, r_harmean));
                    resultG = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, g_harmean));
                    resultB = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, b_harmean));

                    outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_hmeanspFilt" + ImgExtension;
                    break;

                //contraharmonic mean filter Q>0 for pepper & <0 for salt
                case SaltPepperfilterType.chmean:
                    filter = arrGen.ArrOfSingle(m, n, 1);

                    var r_charmean = Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Rc), (Q + 1)), filter, PadType.replicate);
                    var g_charmean = Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Gc), (Q + 1)), filter, PadType.replicate);
                    var b_charmean = Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Bc), (Q + 1)), filter, PadType.replicate);

                    r_charmean = ArrOp.ArraydivElements(r_charmean, (Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Rc), Q), filter, PadType.replicate)));
                    g_charmean = ArrOp.ArraydivElements(g_charmean, (Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Gc), Q), filter, PadType.replicate)));
                    b_charmean = ArrOp.ArraydivElements(b_charmean, (Filter.Filter_double(ArrOp.PowArrayElements(ArrOp.ImageUint8ToDouble(Bc), Q), filter, PadType.replicate)));

                    resultR = ArrOp.ImageArrayToUint8(r_charmean);
                    resultG = ArrOp.ImageArrayToUint8(g_charmean);
                    resultB = ArrOp.ImageArrayToUint8(b_charmean);

                    outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_charmeanspFilt" + ImgExtension;
                    break;

                default:
                    resultR = Rc; resultG = Gc; resultB = Bc;

                    outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_wrongspFilt" + ImgExtension;
                    break;
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            outName = MoreHelpers.OutputFileNames(outName);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
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
