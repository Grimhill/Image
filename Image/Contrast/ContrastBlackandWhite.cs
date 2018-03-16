using System;
using System.Drawing;
using System.Drawing.Imaging;
using Image.CotrastProcess;

//BW and RGB contrast operation
namespace Image
{
    public static class ContrastBlackandWhite
    {        
        //only for b&w image
        
        //default BW contrast low and high intensities of image in 1% of image
        public static void ContrastBlackWhite(Bitmap img)
        {
            ContrastBlackWhiteHelper(img, 0.98, 0.99, 0.99, 0.99, 1);
        }

        public static Bitmap ContrastBlackWhiteBitmap(Bitmap img)
        {
            return ContrastBlackWhiteProcess(img, 0.98, 0.99, 0.99, 0.99, 1);
        }

        //low & high Adjust the grayscale image, specifying the contrast limits
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in)
        {
            ContrastBlackWhiteHelper(img, low_in, high_in, 0, 0, 1);            
        }

        public static Bitmap ContrastBlackWhiteBitmap(Bitmap img, double low_in, double high_in)
        {
            return ContrastBlackWhiteProcess(img, low_in, high_in, 0, 0, 1);         
        }

        //low_in & high_in Adjust the grayscale image, specifying the contrast limits at input
        //low_out & high_out make Out intensity to values between low_out and high_out
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in, double low_out, double high_out)
        {
            ContrastBlackWhiteHelper(img, low_in, high_in, low_out, high_out, 1);          
        }

        public static Bitmap ContrastBlackWhiteBitmap(Bitmap img, double low_in, double high_in, double low_out, double high_out)
        {
            return ContrastBlackWhiteProcess(img, low_in, high_in, low_out, high_out, 1);         
        }

        //low & high Adjust the grayscale image, specifying the contrast limits.
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in, double gamma)
        {
            ContrastBlackWhiteHelper(img, low_in, high_in, 0, 0, gamma);           
        }

        public static Bitmap ContrastBlackWhiteBitmap(Bitmap img, double low_in, double high_in, double gamma)
        {
            return ContrastBlackWhiteProcess(img, low_in, high_in, 0, 0, gamma);           
        }

        //low_in & high_in Adjust the grayscale image, specifying the contrast limits at input
        //low_out & high_out make Out intensity to values between low_out and high_out
        //GAMMA specifies the shape of the curve describing the relationship between the values in Input and Output Image
        public static void ContrastBlackWhite(Bitmap img, double low_in, double high_in, double low_out, double high_out, double gamma)
        {
            ContrastBlackWhiteHelper(img, low_in, high_in, low_out, high_out, gamma);           
        }

        public static Bitmap ContrastBlackWhiteBitmap(Bitmap img, double low_in, double high_in, double low_out, double high_out, double gamma)
        {
            return ContrastBlackWhiteProcess(img, low_in, high_in, low_out, high_out, gamma);          
        }

        //form output name and save contrast process image to file
        private static void ContrastBlackWhiteHelper(Bitmap img, double low_in, double high_in, double low_out, double high_out, double gamma)
        { 
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("Contrast\\BlackandWhite");            

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;

            image = ContrastBlackWhiteProcess(img, low_in, high_in, low_out, high_out, gamma);

            if (low_in == 0.98 && high_in == 0.99 && low_out == 0.99 && high_out == 0.99)            
                outName = defPath + imgName + "_ContrastDefault.png";
            
            else if (low_out == 0 && high_out == 0)
            {
                if (gamma != 1)                
                    outName = defPath + imgName + "_ContrastIn[" + low_in + ";" + high_in + "]Gam_" + gamma + ".png";
                
                else
                    outName = defPath + imgName + "_ContrastIn[" + low_in + ";" + high_in + "]" + ".png";
            }
            else
            {              
                if (gamma != 1)                
                    outName = defPath + imgName + "_ContrastIn[" + low_in + ";" + high_in + "]Out[" + low_out + ";" + high_out + "]Gam_" + gamma + ".png";
                
                else
                    outName = defPath + imgName + "_ContrastIn[" + low_in + ";" + high_in + "]Out[" + low_out + ";" + high_out + "]" + ".png";
            }             
           
            Helpers.SaveOptions(image, outName, ".png");        
        }

        private static Bitmap ContrastBlackWhiteProcess(Bitmap img, double low_in, double high_in, double low_out, double high_out, double gamma)
        {
            int[,] GrayC = new int[img.Height, img.Width];
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8 || Checks.BlackandWhite24bppCheck(img))
            {
                GrayC = MoreHelpers.BlackandWhiteProcessHelper(img);

                //default
                //gamma - linear mapping gamma = 1

                //Make In intensity to values between low_in and high_in
                //Only positive values in range [0:1]
                double[] In = new double[2] { 0, 1 };

                //make Out intensity to values between low_out and high_out
                //Only positive values in range [0:1; 0:1]
                //If high_out < low_out, the output image is reversed, as in a photographic negative. 
                double[] Out = new double[2] { 0, 1 };

                int[,] Cont = new int[img.Height, img.Width];                

                if (low_in > 1 || high_in > 1 || low_in < 0 || high_in < 0 ||
                    low_out > 1 || high_out > 1 || low_out < 0 || high_out < 0)
                {
                    Console.WriteLine("low_in and high_in limits must be in range [0:1] \n" +
                        "low_out and high_out limits must be in range [1:0] or [0 1]");
                }
                else if (low_in >= high_in)
                {
                    Console.WriteLine("low_in must be less then high_in");
                }
                else
                {
                    In = new double[2] { low_in, high_in }; //low and high specifying the contrast limits

                    //Find In limits to contrast image Based on it`s intensity
                    //No sence for values 0.5 and more. 0.01 - here use only default value
                    if (low_in == 0.98 && high_in == 0.99 && low_out == 0.99 && high_out == 0.99) //so bad                   
                        In = ContrastProcess.Stretchlims(GrayC, 0.01); //number - intensity in % pixels saturated at low and high intensities of image                         
                                      
                    else if(low_out != 0 || high_out != 0)            
                        Out = new double[2] { low_out, high_out };
                    //prevent obtain black square if low_out = high_out = 0  in this case used default Out { 0, 1 };

                    Cont = ContrastProcess.InlutContrast(GrayC, ContrastProcess.CountLut(In, Out, gamma));  //Convert integer values using lookup table

                    image = Helpers.SetPixels(image, Cont, Cont, Cont); 
                    if (Depth != 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }                    
                }
            }
            else            
                Console.WriteLine("There non 8bit or 24bit black and white image at input. Method: ContrastBlackandWhite()");            

            return image;
        }       
    }
}
