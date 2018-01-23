using System;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Image.ArrayOperations;

//for salt and pepper noize
namespace Image
{
    public static class SaltPepperFilter
    {
        //SPFILT Performs linear and nonlinear spatial filtering
        //unsharp for chmean
        public static void SaltPepperfilter(Bitmap img, int m, int n, double Q, SaltPepperfilterType spfiltType, bool unsharp, string fileName)
        {
            SaltPepperFilterHelper(img, m, n, Q, spfiltType, unsharp, fileName);        
        }

        public static void SaltPepperfilter(Bitmap img, int m, int n, SaltPepperfilterType spfiltType, string fileName)
        {
            SaltPepperFilterHelper(img, m, n, 1.5, spfiltType, false, fileName);    
        }

        public static void SaltPepperFilterHelper(Bitmap img, int m, int n, double Q, SaltPepperfilterType spfiltType, bool unsharp, string fileName)
        {
            //m & n - filter window dimentions (m - row, n - col)
            //Q - filter order Q for Contraharmonic mean           
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

            ArrGen<double> arrGen = new ArrGen<double>();

            if ((m < 4 && n < 4) && (m > 0 && n > 0))
            {
                switch (spfiltType)
                {
                    //Arithmetic mean filtering.
                    //help with salt noize
                    case SaltPepperfilterType.amean:
                        filter = Filter.Fspecial(m, n, "average");                        
                        resultR = (Filter.Filter_double(Rc, filter)).ArrayToUint8();                        
                        resultG = (Filter.Filter_double(Gc, filter)).ArrayToUint8();                        
                        resultB = (Filter.Filter_double(Bc, filter)).ArrayToUint8();

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_ameanspFilt" + ImgExtension;
                        break;

                    //Geometric mean filtering.
                    //help with salt noize
                    case SaltPepperfilterType.gmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);
                        
                        var r_gmean = (Filter.Filter_double((Rc.ImageUint8ToDouble().LogArrayElements()), filter, PadType.replicate)).ExpArrayElements();                        
                        var g_gmean = (Filter.Filter_double((Gc.ImageUint8ToDouble().LogArrayElements()), filter, PadType.replicate)).ExpArrayElements();                        
                        var b_gmean = (Filter.Filter_double((Bc.ImageUint8ToDouble().LogArrayElements()), filter, PadType.replicate)).ExpArrayElements();
                                                
                        resultR = r_gmean.PowArrayElements(((double)1 / (double)m / (double)n)).ImageArrayToUint8();                        
                        resultG = g_gmean.PowArrayElements(((double)1 / (double)m / (double)n)).ImageArrayToUint8();                        
                        resultB = b_gmean.PowArrayElements(((double)1 / (double)m / (double)n)).ImageArrayToUint8();

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_gmeanspFilt" + ImgExtension;
                        break;

                    //harmonic mean filter
                    //help with salt noize
                    case SaltPepperfilterType.hmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);
                        
                        var r_harmean = Filter.Filter_double(Rc.ImageUint8ToDouble().ArraySumWithConst((2.2204 * Math.Pow(10, -16))).ConstDivByArrayElements(1), filter, PadType.replicate);                        
                        var g_harmean = Filter.Filter_double(Gc.ImageUint8ToDouble().ArraySumWithConst((2.2204 * Math.Pow(10, -16))).ConstDivByArrayElements(1), filter, PadType.replicate);                       
                        var b_harmean = Filter.Filter_double(Bc.ImageUint8ToDouble().ArraySumWithConst((2.2204 * Math.Pow(10, -16))).ConstDivByArrayElements(1), filter, PadType.replicate);
                       
                        resultR = r_harmean.ConstDivByArrayElements(((double)m * (double)n)).ImageArrayToUint8();                        
                        resultG = g_harmean.ConstDivByArrayElements(((double)m * (double)n)).ImageArrayToUint8();                       
                        resultB = b_harmean.ConstDivByArrayElements(((double)m * (double)n)).ImageArrayToUint8();

                        outName = Directory.GetCurrentDirectory() + "\\SaltPepper\\" + fileName + "_hmeanspFilt" + ImgExtension;
                        break;

                    //contraharmonic mean filter Q>0 for pepper & <0 for salt
                    case SaltPepperfilterType.chmean:
                        filter = arrGen.ArrOfSingle(m, n, 1);
                        
                        var r_charmean = Filter.Filter_double(Rc.ImageUint8ToDouble().PowArrayElements((Q + 1)), filter, PadType.replicate);                        
                        var g_charmean = Filter.Filter_double(Gc.ImageUint8ToDouble().PowArrayElements((Q + 1)), filter, PadType.replicate);                        
                        var b_charmean = Filter.Filter_double(Bc.ImageUint8ToDouble().PowArrayElements((Q + 1)), filter, PadType.replicate);
                                                
                        r_charmean = (Filter.Filter_double(Rc.ImageUint8ToDouble().PowArrayElements(Q), filter, PadType.replicate)).ArraydivElements(r_charmean);                        
                        g_charmean = (Filter.Filter_double(Gc.ImageUint8ToDouble().PowArrayElements(Q), filter, PadType.replicate)).ArraydivElements(g_charmean);                        
                        b_charmean = (Filter.Filter_double(Bc.ImageUint8ToDouble().PowArrayElements(Q), filter, PadType.replicate)).ArraydivElements(b_charmean);

                        resultR = r_charmean.ImageArrayToUint8(); 
                        resultG = g_charmean.ImageArrayToUint8(); 
                        resultB = b_charmean.ImageArrayToUint8(); 

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

                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
            else
            {
                Console.WriteLine("Can`t implement filter larger, than 3x3, sorry");
            }
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
