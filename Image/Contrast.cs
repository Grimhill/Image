using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

//BW and RGB contrast operation
namespace Image
{
    public static class Contrast
    {
        //only for b&w image

        #region Contrast_BW
        //default BW contrast low and high intensities of image in 1% of image
        public static void ContrastBlackWhite(Bitmap img, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var GrayC = ColorList[0].Color;
            string outName = String.Empty;

            //In values between low_in and high_in
            //Find In limits to contrast image Based on it`s intensity
            //No sence for values 0.5 and more. 0.01 - here use only default value
            var In = Stretchlims(GrayC, 0.01); //number - intensity in % pixels saturated at low and high intensities of image

            //Only positive values in range 0.01 - 0.99
            //var In = stretchlims(Gc, [0.01, 0.99]);

            //make Out intensity to values between low_out and high_out
            //Only positive values in range [0:1; 0:1]
            // If high_out < low_out, the output image is reversed, as in a photographic negative.           
            double[] Out = { 0, 1 }; //default value
            double gamma = 1; //gamma defaults to 1 (linear mapping)       

            var Cont = Inlut(GrayC, CountLut(In, Out, gamma));  //Convert integer values using lookup table

            image = Helpers.SetPixels(image, Cont, Cont, Cont);

            outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastDefault" + ImgExtension;

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        //low & high Adjust the grayscale image, specifying the contrast limits
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var GrayC = ColorList[0].Color;
            string outName = String.Empty;

            if (low_in > 1 || high_in > 1 || low_in < 0 || high_in < 0)
            {
                Console.WriteLine("low_in and high_in limits must be in range [0:1]");
            }
            else if (low_in > high_in)
            {
                Console.WriteLine("low_in must be less then high_in");
            }
            else
            {
                //In values between low_in and high_in                             
                double[] In = new double[2] { low_in, high_in }; //low and high specifying the contrast limits 

                //make Out intensity to values between low_out and high_out
                //Only positive values in range [0:1; 0:1]
                //If high_out < low_out, the output image is reversed, as in a photographic negative.               
                double[] Out = { 0, 1 }; //default value
                double gamma = 1; //gamma defaults to 1 (linear mapping)

                var Cont = Inlut(GrayC, CountLut(In, Out, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, Cont, Cont, Cont);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastInLim" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }

        //low_in & high_in Adjust the grayscale image, specifying the contrast limits at input
        //low_out & high_out make Out intensity to values between low_out and high_out
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in, double low_out, double high_out, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var GrayC = ColorList[0].Color;
            string outName = String.Empty;

            if (low_in > 1 || high_in > 1 || low_in < 0 || high_in < 0 ||
                low_out > 1 || high_out > 1 || low_out < 0 || high_out < 0)
            {
                Console.WriteLine("low_in and high_in limits must be in range [0:1] \n" +
                    "low_out and high_out limits must be in range [0:1]");
            }
            else if (low_in > high_in)
            {
                Console.WriteLine("low_in must be less then high_im");
            }
            else
            {
                //In values between low_in and high_in                             
                double[] In = new double[2] { low_in, high_in }; //low and high specifying the contrast limits

                //If high_out < low_out, the output image is reversed, as in a photographic negative. 
                //Only positive values in range [0:1; 0:1]
                double[] Out = new double[2] { low_out, high_out };
                double gamma = 1; //gamma defaults to 1 (linear mapping)

                var Cont = Inlut(GrayC, CountLut(In, Out, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, Cont, Cont, Cont);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastInOutLim" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }

        //low & high Adjust the grayscale image, specifying the contrast limits.
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in, double gamma, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var GrayC = ColorList[0].Color;
            string outName = String.Empty;

            if (low_in > 1 || high_in > 1 || low_in < 0 || high_in < 0)
            {
                Console.WriteLine("low_in and high_in limits must be in range [0:1]");
            }
            else if (low_in > high_in)
            {
                Console.WriteLine("low_in must be less then high_in");
            }
            else
            {
                //In values between low_in and high_in                             
                double[] In = new double[2] { low_in, high_in }; //low and high specifying the contrast limits 

                //make Out intensity to values between low_out and high_out
                //Only positive values in range [0:1; 0:1]
                //If high_out < low_out, the output image is reversed, as in a photographic negative.                
                double[] Out = { 0, 1 }; //default value   

                var Cont = Inlut(GrayC, CountLut(In, Out, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, Cont, Cont, Cont);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastInLimGam" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }

        //low_in & high_in Adjust the grayscale image, specifying the contrast limits at input
        //low_out & high_out make Out intensity to values between low_out and high_out
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in, double low_out, double high_out, double gamma, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var GrayC = ColorList[0].Color;
            string outName = String.Empty;

            if (low_in > 1 || high_in > 1 || low_in < 0 || high_in < 0 ||
                low_out > 1 || high_out > 1 || low_out < 0 || high_out < 0)
            {
                Console.WriteLine("low_in and high_in limits must be in range [0:1] \n" +
                    "low_out and high_out limits must be in range [1:0]");
            }
            else if (low_in > high_in)
            {
                Console.WriteLine("low_in must be less then high_in");
            }
            else
            {
                //In values between low_in and high_in                             
                double[] In = new double[2] { low_in, high_in }; //low and high specifying the contrast limits

                //Only positive values in range [0:1; 0:1]
                //If high_out < low_out, the output image is reversed, as in a photographic negative.               
                double[] Out = new double[2] { low_out, high_out };

                var Cont = Inlut(GrayC, CountLut(In, Out, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, Cont, Cont, Cont);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastInOutLimGam" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }

        #endregion Contrast_BW


        #region Contrast_RGB
        //low_in & high_in specifying the contrast limits at input for each color component        
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;
            string outName = String.Empty;

            if (Rc_low_in > 1 || Rc_high_in > 1 || Gc_low_in > 1 || Gc_high_in > 1 || Bc_low_in > 1 || Bc_high_in > 1)
            {
                Console.WriteLine("low_in limit and high_in limit must be in range [0 1]");
            }
            else if (Rc_low_in > Rc_high_in || Gc_low_in > Gc_high_in || Bc_low_in > Bc_high_in)
            {
                Console.WriteLine("lo_inw limit must be less then high_in limit for each image plane");
            }
            else
            {
                //low and high specifying the contrast limits                              
                double[] InRc = new double[2] { Rc_low_in, Rc_high_in };
                double[] InGc = new double[2] { Gc_low_in, Gc_high_in };
                double[] InBc = new double[2] { Bc_low_in, Bc_high_in };

                //make Out intensity to values between low_out and high_out
                //Only positive values in range [0:1; 0:1]
                //If high_out < low_out, the output image is reversed, as in a photographic negative.                
                double[] Out = { 0, 1 }; //default value
                double gamma = 1; //gamma defaults to 1 (linear mapping)

                //Look up table
                var ContRc = Inlut(Rc, CountLut(InRc, Out, gamma));  //Convert integer values using lookup table
                var ContGc = Inlut(Gc, CountLut(InGc, Out, gamma));  //Convert integer values using lookup table
                var ContBc = Inlut(Bc, CountLut(InBc, Out, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, ContRc, ContGc, ContBc);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastRGBInLim" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        } //double [] L_in, double [] H_in

        //low_in & high_in specifying the contrast limits at input for each color component
        //low_out & high_out make Out intensity to values between low_out and high_out for each color component
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;
            string outName = String.Empty;

            if (Rc_low_in > 1 || Rc_high_in > 1 || Gc_low_in > 1 || Gc_high_in > 1 || Bc_low_in > 1 || Bc_high_in > 1
                || Rc_low_in < 0 || Rc_high_in < 0 || Gc_low_in < 0 || Gc_high_in < 0 || Bc_low_in < 0 || Bc_high_in < 0
                || Rc_low_out > 1 || Rc_high_out > 1 || Gc_low_out > 1 || Gc_high_out > 1 || Bc_low_out > 1 || Bc_high_out > 1
                || Rc_low_out < 0 || Rc_high_out < 0 || Gc_low_out < 0 || Gc_high_out < 0 || Bc_low_out < 0 || Bc_high_out < 0)
            {
                Console.WriteLine("low_in limit and high_in limits must be in range [0:1]\n" +
                    "low_out and high_out limits must be in range [0:1]");
            }
            else if (Rc_low_in > Rc_high_in || Gc_low_in > Gc_high_in || Bc_low_in > Bc_high_in)
            {
                Console.WriteLine("lo_inw limit must be less then high_in limit for each image plane");
            }
            else
            {
                //low and high specifying the In contrast limits                              
                double[] RcIn = new double[2] { Rc_low_in, Rc_high_in };
                double[] GcIn = new double[2] { Gc_low_in, Gc_high_in };
                double[] BcIn = new double[2] { Bc_low_in, Bc_high_in };

                //low and high specifying the Out contrast limits                              
                double[] RcOut = new double[2] { Rc_low_out, Rc_high_out };
                double[] GcOut = new double[2] { Gc_low_out, Gc_high_out };
                double[] BcOut = new double[2] { Bc_low_out, Bc_high_out };

                double gamma = 1; //gamma defaults to 1 (linear mapping)

                //Look up table
                var ContRc = Inlut(Rc, CountLut(RcIn, RcOut, gamma));  //Convert integer values using lookup table
                var ContGc = Inlut(Gc, CountLut(GcIn, GcOut, gamma));  //Convert integer values using lookup table
                var ContBc = Inlut(Bc, CountLut(BcIn, BcOut, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, ContRc, ContGc, ContBc);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastRGBInOutLim" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }  //double [,] Lh_in, double [,] Lh_out

        //low_in & high_in specifying the contrast limits at input for each color component    
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in, double gamma, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;
            string outName = String.Empty;

            if (Rc_low_in > 1 || Rc_high_in > 1 || Gc_low_in > 1 || Gc_high_in > 1 || Bc_low_in > 1 || Bc_high_in > 1)
            {
                Console.WriteLine("low limit and high limit must be in range [0 1]");
            }
            else if (Rc_low_in > Rc_high_in || Gc_low_in > Gc_high_in || Bc_low_in > Bc_high_in)
            {
                Console.WriteLine("low limit must be less then high limit for each image plane");
            }
            else
            {
                //low and high specifying the In contrast limits                              
                double[] RcIn = new double[2] { Rc_low_in, Rc_high_in };
                double[] GcIn = new double[2] { Gc_low_in, Gc_high_in };
                double[] BcIn = new double[2] { Bc_low_in, Bc_high_in };

                //make Out intensity to values between low_out and high_out
                //Only positive values in range [0; 0:0.99], also can change low, but it`s for fun
                //If high_out < low_out, the output image is reversed, as in a photographic negative.                
                double[] Out = { 0, 1 }; //default value              

                //Look up table
                var ContRc = Inlut(Rc, CountLut(RcIn, Out, gamma));  //Convert integer values using lookup table
                var ContGc = Inlut(Gc, CountLut(GcIn, Out, gamma));  //Convert integer values using lookup table
                var ContBc = Inlut(Bc, CountLut(BcIn, Out, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, ContRc, ContGc, ContBc);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastRGBInLimGam" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        } //double [] L_in, double [] H_in

        //low_in & high_in specifying the contrast limits at input for each color component
        //low_out & high_out make Out intensity to values between low_out and high_out for each color component
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out, double gamma, string ImgExtension)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contrast");

            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;
            string outName = String.Empty;

            if (Rc_low_in > 1 || Rc_high_in > 1 || Gc_low_in > 1 || Gc_high_in > 1 || Bc_low_in > 1 || Bc_high_in > 1
                || Rc_low_in < 0 || Rc_high_in < 0 || Gc_low_in < 0 || Gc_high_in < 0 || Bc_low_in < 0 || Bc_high_in < 0
                || Rc_low_out > 1 || Rc_high_out > 1 || Gc_low_out > 1 || Gc_high_out > 1 || Bc_low_out > 1 || Bc_high_out > 1
                || Rc_low_out < 0 || Rc_high_out < 0 || Gc_low_out < 0 || Gc_high_out < 0 || Bc_low_out < 0 || Bc_high_out < 0)
            {
                Console.WriteLine("low_in limit and high_in limits must be in range [0:1]\n" +
                    "low_out and high_out limits must be in range [0:1]");
            }
            else if (Rc_low_in > Rc_high_in || Gc_low_in > Gc_high_in || Bc_low_in > Bc_high_in)
            {
                Console.WriteLine("lo_inw limit must be less then high_in limit for each image plane");
            }
            else
            {
                //low and high specifying the In contrast limits                              
                double[] RcIn = new double[2] { Rc_low_in, Rc_high_in };
                double[] GcIn = new double[2] { Gc_low_in, Gc_high_in };
                double[] BcIn = new double[2] { Bc_low_in, Bc_high_in };

                //low and high specifying the Out contrast limits                              
                double[] RcOut = new double[2] { Rc_low_out, Rc_high_out };
                double[] GcOut = new double[2] { Gc_low_out, Gc_high_out };
                double[] BcOut = new double[2] { Bc_low_out, Bc_high_out };

                //Look up table
                var ContRc = Inlut(Rc, CountLut(RcIn, RcOut, gamma));  //Convert integer values using lookup table
                var ContGc = Inlut(Gc, CountLut(GcIn, GcOut, gamma));  //Convert integer values using lookup table
                var ContBc = Inlut(Bc, CountLut(BcIn, BcOut, gamma));  //Convert integer values using lookup table

                image = Helpers.SetPixels(image, ContRc, ContGc, ContBc);

                outName = Directory.GetCurrentDirectory() + "\\Contrast\\ContrastRGBInOutLimGam" + ImgExtension;
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        } //double [,] Lh_in, double [,] Lh_out
        #endregion Contrast_RGB


        //Find limits to contrast stretch an image\\ Using for default BW contrast
        //IntensityProcent - intensity in % pixels saturated at low and high intensities of image
        public static double[] Stretchlims(int[,] Gc, double IntensityProcent)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            double[] ImLH = new double[2]; //contatin a pair of gray values, which represent image low & high limits to contrast stretch an image
            double[] tol = new double[2]; //tol saturates equal fractions at low and high pixel values

            if (IntensityProcent == 0.01)
            {
                tol[0] = .01; //default
                tol[1] = .99; //default
            }
            else
            {
                tol[0] = IntensityProcent;
                tol[1] = 1 - IntensityProcent;
            }

            if (tol[0] < tol[1]) // tol[0] - low, tol[1] - high
            {
                var ImHist = Contrast.ImHist(Gc); //obtain img histohram

                int[] CumulativeSum = new int[256];
                //CumulativeSum[0] = ImHist[0];
                for (int i = 0; i < 256; i++)
                {
                    if (i == 0)
                    {
                        CumulativeSum[i] = ImHist[i];
                    }
                    else
                    {
                        CumulativeSum[i] = ImHist[i] + CumulativeSum[i - 1];
                    }
                }

                //cumulative distribution function
                var cdf = ArrOp.VectorDivByConst(ArrOp.VectorToDouble(CumulativeSum), ImHist.Sum());
                var ilow = Array.IndexOf(cdf, cdf.First(x => x > tol[0])); //index first low
                var ihigh = Array.IndexOf(cdf, cdf.First(x => x >= tol[1])); //index first high

                if (ilow == ihigh) //this could happen if img is flat
                {
                    //no implementation exception c(: ImLH[0, 0] = 1; ImLH[1, 0] = 256;
                }
                else
                {
                    ImLH[0] = ((double)ilow - 1) / 255; ImLH[1] = ((double)ihigh - 1) / 255; //convert to range [0 1]
                }
            }
            else
            {
                ImLH[0] = 0; ImLH[1] = 1;
            }

            return ImLH;
        }

        //if want for use for RGB image, input Rc, Gc, Bc args
        public static double[] Stretchlims(int[,] Gc, double low, double high)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            double[] ImLH = new double[2];

            double[] tol = { low, high };

            if (tol[0] < tol[1]) // tol[0] - low, tol[1] - high
            {
                var ImHist = Contrast.ImHist(Gc); //obtain img histohram

                int[] CumulativeSum = new int[256];
                //CumulativeSum[0] = ImHist[0];
                for (int i = 0; i < 256; i++)
                {
                    if (i == 0)
                    {
                        CumulativeSum[i] = ImHist[i];
                    }
                    else
                    {
                        CumulativeSum[i] = ImHist[i] + CumulativeSum[i - 1];
                    }
                }

                //cumulative distribution function
                var cdf = ArrOp.VectorDivByConst(ArrOp.VectorToDouble(CumulativeSum), ImHist.Sum());
                var ilow = Array.IndexOf(cdf, cdf.First(x => x > tol[0])); //index first low
                var ihigh = Array.IndexOf(cdf, cdf.First(x => x >= tol[1])); //index first high

                if (ilow == ihigh) //this could happen if img is flat
                {
                    //no implementation exception c(: ImLH[0, 0] = 1; ImLH[1, 0] = 256;
                }
                else
                {
                    ImLH[0] = ((double)ilow - 1) / 255; ImLH[1] = ((double)ihigh - 1) / 255;
                }
            }
            else
            {
                ImLH[0] = 0; ImLH[1] = 1;
            }

            return ImLH;
        }


        //image histogram (for BW or RC\GC\Bc)
        //only for uint8 images realization
        public static int[] ImHist(int[,] Im) //imHist(int[,] Im, n) where n - uint size. 8 - 256, 16 - 65536
        {
            int[] tempData = Im.Cast<int>().ToArray();
            int[] imHistResult = new int[256];
            int count = 0;

            int[] temp = new int[256];
            for (int k = 0; k < temp.Length; k++)
            {
                temp[k] = k;
            }

            for (int i = 0; i < 256; i++)
            {
                count = 0;
                for (int j = 0; j < tempData.Length; j++)
                {
                    if (temp[i] == tempData[j])
                    {
                        count++;
                    }
                }
                imHistResult[i] = count;
            }
            return imHistResult;
        }


        //count Look up table
        public static double[] CountLut(double[] In, double[] Out, double gamma)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            //prepare for look up table, receive vector of doubles for uint8 [0 255]
            //Cumulating sum of elemets 1/256
            double[] temp = new double[256];
            for (int k = 1; k < temp.Length; k++)
            {
                temp[k] = temp[k - 1] + (double)1 / (double)256;
            }

            double[] Lut = new double[256];

            if (Out[0] == 0 && Out[1] == 1) //if out [0 1]
            {
                //(arr - low_in) ./ (high_in - low_in)) .^ gamma
                Lut = ArrOp.PowVectorElements(ArrOp.VectorDivByConst(ArrOp.VectorSubConst(temp, In[0]), In[1] - In[0]), gamma);

                for (int i = 0; i < Lut.Length; i++)
                {
                    if (Lut[i] < 0) { Lut[i] = 0; }
                    else if (Lut[i] > 1) { Lut[i] = 1; }
                }
            }
            else if (Out[0] == 1 && Out[1] == 0) //if out [1 0]
            {
                //(arr - low_in) ./ (high_in - low_in)) .^ gamma
                Lut = ArrOp.PowVectorElements(ArrOp.VectorDivByConst(ArrOp.VectorSubConst(temp, In[0]), In[1] - In[0]), gamma);
                Lut = ArrOp.ConstSubVectorElements(1, Lut);

                for (int i = 0; i < Lut.Length; i++)
                {
                    if (Lut[i] < 0) { Lut[i] = 0; }
                    else if (Lut[i] > 1) { Lut[i] = 1; }
                }
            }
            else //if out in range [0 1] or [1 0]
            {
                //low_in & low_out
                //(arr < low_in) .* low_out
                double[] partOne = new double[256];
                for (int i = 0; i < partOne.Length; i++)
                {
                    if (temp[i] < In[0])
                    {
                        partOne[i] = Out[0];
                    }
                    else { partOne[i] = 0; }
                }

                //arr >= low_in & arr < high_in
                double[] partTwo = new double[256];
                for (int i = 0; i < partTwo.Length; i++)
                {
                    if (temp[i] >= In[0] & temp[i] < In[1])
                    {
                        partTwo[i] = 1;
                    }
                    else { partTwo[i] = 0; }
                }

                //(arr >= hign_in) .* high_out
                double[] partThree = new double[256];
                for (int i = 0; i < partThree.Length; i++)
                {
                    if (temp[i] >= In[1])
                    {
                        partThree[i] = Out[1];
                    }
                    else { partThree[i] = 0; }
                }

                //  1) = (arr < low_in) .* low_out
                //  2) = (1) + (arr >= low_in & arr < high_in) .* (low_out + (high_out - low_out) .* ((arr - low_in) ./ (high_in - low_in)) .^ gamma)
                //  3) = (2) + (arr >= hign_in) .* high_out

                Lut = ArrOp.PowVectorElements(ArrOp.VectorDivByConst(ArrOp.VectorSubConst(temp, In[0]), (In[1] - In[0])), gamma);
                Lut = ArrOp.VectorSumConst(ArrOp.VectorMultByConst(Lut, (Out[1] - Out[0])), Out[0]);

                //sum with arr >= low_in & arr < high_in
                Lut = ArrOp.MultVectors(Lut, partTwo);
                //sum with (arr < low_in) .* low_out
                Lut = ArrOp.SumVectors(partOne, Lut);
                //sum with (arr >= hign_in) .* high_out
                Lut = ArrOp.SumVectors(Lut, partThree);
            }


            return Lut;
        }


        //redefine array using look up table 
        public static int[,] Inlut(int[,] im, double[] lut)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int[,] lutResult = new int[im.GetLength(0), im.GetLength(1)];
            var luts = ArrOp.ImageVectorToUint8(lut);

            for (int i = 0; i < im.GetLength(0); i++)
            {
                for (int j = 0; j < im.GetLength(1); j++)
                {
                    if (im[i, j] == 255) //bad condition
                    {
                        lutResult[i, j] = luts[im[i, j]];
                    }
                    else
                    {
                        lutResult[i, j] = luts[im[i, j] + 1];
                    }
                }
            }

            return lutResult;
        }
    }
}
