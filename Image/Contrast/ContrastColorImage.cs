using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.CotrastProcess;

namespace Image
{
    public static class ContrastColorImage
    {       
        //If don`t want to change contrast for some plane - set in\out parametes: in - 0, out - 1

        //low_in & high_in specifying the contrast limits at input for each color component        
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in)
        {
            ContrastRGBHelper(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in, 0, 0, 0, 0, 0, 0, 1);
        } //double [] L_in, double [] H_in

        public static Bitmap ContrastRGBBitmap(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in)
        {
            return ContrastRGBProcess(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in, 0, 0, 0, 0, 0, 0, 1);
        }

        //low_in & high_in specifying the contrast limits at input for each color component
        //low_out & high_out make Out intensity to values between low_out and high_out for each color component
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out)
        {
            ContrastRGBHelper(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in,
                Rc_low_out, Rc_high_out, Gc_low_out, Gc_high_out, Bc_low_out, Bc_high_out, 1);
        }  //double [,] Lh_in, double [,] Lh_out

        public static Bitmap ContrastRGBBitmap(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out)
        {
            return ContrastRGBProcess(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in,
                Rc_low_out, Rc_high_out, Gc_low_out, Gc_high_out, Bc_low_out, Bc_high_out, 1);
        }

        //low_in & high_in specifying the contrast limits at input for each color component    
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in, double gamma)
        {
            ContrastRGBHelper(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in, 0, 0, 0, 0, 0, 0, gamma);
        } //double [] L_in, double [] H_in

        public static Bitmap ContrastRGBBitmap(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in, double gamma)
        {
            return ContrastRGBProcess(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in, 0, 0, 0, 0, 0, 0, gamma);
        }

        //low_in & high_in specifying the contrast limits at input for each color component
        //low_out & high_out make Out intensity to values between low_out and high_out for each color component
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastRGB(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out, double gamma)
        {
            ContrastRGBHelper(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in,
                Rc_low_out, Rc_high_out, Gc_low_out, Gc_high_out, Bc_low_out, Bc_high_out, gamma);
        } //double [,] Lh_in, double [,] Lh_out

        public static Bitmap ContrastRGBBitmap(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out, double gamma)
        {
            return ContrastRGBProcess(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in,
                Rc_low_out, Rc_high_out, Gc_low_out, Gc_high_out, Bc_low_out, Bc_high_out, gamma);
        }     

        //form output name and save contrast process image to file
        private static void ContrastRGBHelper(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out, double gamma)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Contrast\\RGB");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            string outName = string.Empty;

            image = ContrastRGBProcess(img, Rc_low_in, Rc_high_in, Gc_low_in, Gc_high_in, Bc_low_in, Bc_high_in,
                Rc_low_out, Rc_high_out, Gc_low_out, Gc_high_out, Bc_low_out, Bc_high_out, gamma);

            if (Rc_low_out == 0 && Rc_high_out == 0 && Gc_low_out == 0 && Gc_high_out == 0
                        && Bc_low_out == 0 && Bc_high_out == 0)
            {
                if (gamma != 1)
                    outName = defPath + imgName + "_ContrastRGBIn[" + Rc_low_in + "," + Gc_low_in + "," + Bc_low_in
                         + ";" + Rc_high_in + "," + Gc_high_in + "," + Bc_high_in + "]Gam_" + gamma + imgExtension;

                else
                    outName = defPath + imgName + "_ContrastRGBIn[" + Rc_low_in + "," + Gc_low_in + "," + Bc_low_in
                        + ";" + Rc_high_in + "," + Gc_high_in + "," + Bc_high_in + "]" + imgExtension;
            }
            else
            {
                if (gamma != 1)
                {
                    outName = defPath + imgName + "_ContrastRGBIn[" + Rc_low_in + "," + Gc_low_in + "," + Bc_low_in
                        + ";" + Rc_high_in + "," + Gc_high_in + "," + Bc_high_in + "]Out[";
                    outName = outName + Rc_low_out + "," + Gc_low_out + "," + Bc_low_out
                        + ";" + Rc_high_out + "," + Gc_high_out + "," + Bc_high_out + "]Gam_" + gamma + imgExtension;
                }
                else
                {
                    outName = defPath + imgName + "_ContrastRGBIn[" + Rc_low_in + "," + Gc_low_in + "," + Bc_low_in
                        + ";" + Rc_high_in + "," + Gc_high_in + "," + Bc_high_in + "]Out[";
                    outName = outName + Rc_low_out + "," + Gc_low_out + "," + Bc_low_out
                        + ";" + Rc_high_out + "," + Gc_high_out + "," + Bc_high_out + "]" + imgExtension;
                }
            }

            Helpers.SaveOptions(image, outName, imgExtension);
        }

        private static Bitmap ContrastRGBProcess(Bitmap img, double Rc_low_in, double Rc_high_in, double Gc_low_in, double Gc_high_in, double Bc_low_in, double Bc_high_in,
            double Rc_low_out, double Rc_high_out, double Gc_low_out, double Gc_high_out, double Bc_low_out, double Bc_high_out, double gamma)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            if (Checks.RGBinput(img))
            {
                List<ArraysListInt> ColorList = Helpers.GetPixels(img);
                string outName = String.Empty;

                //default
                //gamma - linear mapping gamma = 1

                //In values between low_in and high_in 
                //Only positive values in range [0:1]
                //low and high specifying the In contrast limits                              
                double[] RcIn = new double[2] { 0, 1 };
                double[] GcIn = new double[2] { 0, 1 };
                double[] BcIn = new double[2] { 0, 1 };

                //make Out intensity to values between low_out and high_out
                //Only positive values in range [0:1; 0:1]
                // If high_out < low_out, the output image is reversed, as in a photographic negative.                                           
                double[] RcOut = new double[2] { 0, 1 };
                double[] GcOut = new double[2] { 0, 1 };
                double[] BcOut = new double[2] { 0, 1 };

                if (Rc_low_in > 1 || Rc_high_in > 1 || Gc_low_in > 1 || Gc_high_in > 1 || Bc_low_in > 1 || Bc_high_in > 1
                    || Rc_low_in < 0 || Rc_high_in < 0 || Gc_low_in < 0 || Gc_high_in < 0 || Bc_low_in < 0 || Bc_high_in < 0
                    || Rc_low_out > 1 || Rc_high_out > 1 || Gc_low_out > 1 || Gc_high_out > 1 || Bc_low_out > 1 || Bc_high_out > 1
                    || Rc_low_out < 0 || Rc_high_out < 0 || Gc_low_out < 0 || Gc_high_out < 0 || Bc_low_out < 0 || Bc_high_out < 0)
                {
                    Console.WriteLine("low_in limit and high_in limits must be in range [0:1]\n" +
                        "low_out and high_out limits must be in range [0:1] or [1 0]");
                }
                else if (Rc_low_in >= Rc_high_in || Gc_low_in >= Gc_high_in || Bc_low_in >= Bc_high_in)
                {
                    Console.WriteLine("low_in limit must be less then high_in limit for each color plane");
                }
                else
                {
                    RcIn = new double[2] { Rc_low_in, Rc_high_in };
                    GcIn = new double[2] { Gc_low_in, Gc_high_in };
                    BcIn = new double[2] { Bc_low_in, Bc_high_in };

                    if (Rc_low_out != 0 || Rc_high_out != 0 || Gc_low_out != 0 || Gc_high_out != 0
                        || Bc_low_out != 0 || Bc_high_out != 0)
                    {
                        RcOut = new double[2] { Rc_low_out, Rc_high_out };
                        GcOut = new double[2] { Gc_low_out, Gc_high_out };
                        BcOut = new double[2] { Bc_low_out, Bc_high_out };
                    }
                    //prevent obtain black square if all low_out = high_out = 0  in this case used default Out { 0, 1 };

                    //Convert integer values using lookup table
                    var ContRc = ContrastProcess.InlutContrast(ColorList[0].Color, ContrastProcess.CountLut(RcIn, RcOut, gamma));
                    var ContGc = ContrastProcess.InlutContrast(ColorList[1].Color, ContrastProcess.CountLut(GcIn, GcOut, gamma));
                    var ContBc = ContrastProcess.InlutContrast(ColorList[2].Color, ContrastProcess.CountLut(BcIn, BcOut, gamma));

                    image = Helpers.SetPixels(image, ContRc, ContGc, ContBc);
                }
            }

            return image;
        }       
    }
}
