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
    public static class spFilt
    {
        //SPFILT Performs linear and nonlinear spatial filtering
        public static void spfilt(Bitmap img, int m, int n, double Q, SpfiltType spfiltType, bool unsharp)
        {
            //m & n - filter window dimentions (m - row, n - col)
            //Q - filter order Q for Contraharmonic mean

            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            double[,] filter;
            int[,] resultR = new int[height, width];
            int[,] resultG = new int[height, width];
            int[,] resultB = new int[height, width];
            string outName = String.Empty;

            ArrGen<double> arrGen;
            arrGen = new ArrGen<double>();

            if (m < 4 & n < 4)
            {
                switch (spfiltType.ToString())
                {
                    //Arithmetic mean filtering.
                    //help with salt noize
                    case "amean":
                        filter = Filter.Fspecial(m, n, "average");
                        resultR = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Rc), filter, PadType.replicate));
                        resultG = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Gc), filter, PadType.replicate));
                        resultB = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Bc), filter, PadType.replicate));

                        outName = Directory.GetCurrentDirectory() + "\\Rand\\ameanspFilt.jpg";
                        break;

                    //Geometric mean filtering.
                    //help with salt noize
                    case "gmean":
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Rc)), filter, PadType.replicate));
                        var g_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Gc)), filter, PadType.replicate));
                        var b_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Bc)), filter, PadType.replicate));

                        resultR = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));
                        resultG = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(b_gmean, ((double)1 / (double)m / (double)n)));
                        resultB = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));

                        outName = Directory.GetCurrentDirectory() + "\\Rand\\gmeanspFilt.jpg";
                        break;

                    //harmonic mean filter
                    //help with salt noize
                    case "hmean":
                        filter = arrGen.ArrOfSingle(m, n, 1);

                        var r_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Rc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                        var g_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Gc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                        var b_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Bc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);

                        resultR = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, r_harmean));
                        resultG = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, g_harmean));
                        resultB = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, b_harmean));

                        outName = Directory.GetCurrentDirectory() + "\\Rand\\hmeanspFilt.jpg";
                        break;

                    //contraharmonic mean filter Q>0 for pepper & <0 for salt
                    case "chmean":
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

                        outName = Directory.GetCurrentDirectory() + "\\Rand\\chmeanspFilt.jpg";
                        break;

                    default:
                        resultR = Rc; resultG = Gc; resultB = Bc;

                        outName = Directory.GetCurrentDirectory() + "\\Rand\\wrongspFilt.jpg";
                        break;
                }

                if (spfiltType.ToString() == "chmean" & unsharp)
                {
                    var im = Helpers.SetPixels(image, resultR, resultG, resultB);
                    image = Helpers.FastSharpImage(im);

                }
                else
                {
                    image = Helpers.SetPixels(image, resultR, resultG, resultB);
                }

                //dont forget, that directory Rand must exist. Later add if not exist - creat
                image.Save(outName);
            }
            else
            {
                Console.WriteLine("Can`t if filter larger, than 3x3, sorry");
            }
        }

        public static void spfilt(Bitmap img, int m, int n, SpfiltType spfiltType)
        {
            //m & n - filter window dimentions (m - row, n - col)
            //Q - filter order Q for Contraharmonic mean

            var Q = 1.5; //default

            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            double[,] filter;
            int[,] resultR = new int[height, width];
            int[,] resultG = new int[height, width];
            int[,] resultB = new int[height, width];
            string outName = String.Empty;

            ArrGen<double> arrGen;
            arrGen = new ArrGen<double>();

            switch (spfiltType.ToString())
            {
                //Arithmetic mean filtering.
                //help with salt noize
                case "amean":
                    filter = Filter.Fspecial(m, n, "average");
                    resultR = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Rc), filter, PadType.replicate));
                    resultG = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Gc), filter, PadType.replicate));
                    resultB = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Bc), filter, PadType.replicate));

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\ameanspFilt.jpg";
                    break;

                //Geometric mean filtering.
                //help with salt noize
                case "gmean":
                    filter = arrGen.ArrOfSingle(m, n, 1);

                    var r_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Rc)), filter, PadType.replicate));
                    var g_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Gc)), filter, PadType.replicate));
                    var b_gmean = ArrOp.ExpArrayElements(Filter.Filter_double(ArrOp.LogArrayElements(ArrOp.ImageUint8ToDouble(Bc)), filter, PadType.replicate));

                    resultR = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));
                    resultG = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(b_gmean, ((double)1 / (double)m / (double)n)));
                    resultB = ArrOp.ImageArrayToUint8(ArrOp.PowArrayElements(r_gmean, ((double)1 / (double)m / (double)n)));

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\gmeanspFilt.jpg";
                    break;

                //harmonic mean filter
                //help with salt noize
                case "hmean":
                    filter = arrGen.ArrOfSingle(m, n, 1);

                    var r_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Rc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                    var g_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Gc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);
                    var b_harmean = Filter.Filter_double(ArrOp.ConstDivByArrayElements(1, ArrOp.ArraySumWithConst(ArrOp.ImageUint8ToDouble(Bc), 2.2204 * Math.Pow(10, -16))), filter, PadType.replicate);

                    resultR = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, r_harmean));
                    resultG = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, g_harmean));
                    resultB = ArrOp.ImageArrayToUint8(ArrOp.ConstDivByArrayElements((double)m * (double)n, b_harmean));

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\hmeanspFilt.jpg";
                    break;

                //contraharmonic mean filter Q>0 for pepper & <0 for salt
                case "chmean":
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
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\charmeanspFilt.jpg";
                    break;

                default:
                    resultR = Rc; resultG = Gc; resultB = Bc;

                    outName = Directory.GetCurrentDirectory() + "\\Rand\\wrongspFilt.jpg";
                    break;
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }
    }

    public enum SpfiltType
    {
        amean,
        gmean,
        hmean,
        chmean
    }
}
