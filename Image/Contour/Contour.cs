using System;
using System.Drawing;
using System.Reflection;
using System.ComponentModel;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    public static class Contour //obtain contour of image
    {      
        //save into file contour after process
        public static void FindContour(Bitmap img, CountourVariant variant)
        {        
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Contour");            

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb); 
            image = ContourHelper(img, variant);

            string outName = defPath + imgName + variant.GetEnumDescription() + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        //return contour after process as bitmap object
        public static Bitmap ContourBitmap(Bitmap img, CountourVariant variant)
        {     
            return ContourHelper(img, variant);
        }

        private static Bitmap ContourHelper(Bitmap img, CountourVariant variant)
        {           
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            //arrays, where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            bool type = true;

            //filtered values storage
            List<ArraysListDouble> filt = new List<ArraysListDouble>();

            //obtain color components. form 8bpp works too, but not recommended to use 8-bit .jpeg\tif\jpg images
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            var Red   = ColorList[0].Color;
            var Green = ColorList[1].Color;
            var Blue  = ColorList[2].Color;
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            
            if (Depth == 8 || Checks.BlackandWhite24bppCheck(ColorList))
            { type = false; }

            //variants 1-4 black & white. Variants 5, 6 - colored
            if (variant == CountourVariant.Variant1_BW || variant == CountourVariant.Variant5_RGB || variant == CountourVariant.Variant2_BW)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with double
                if (!type)
                {
                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Red, "Sobel") } ); //b&w x
                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Red, "SobelT") }); //b&w y                   
                }
                else
                {
                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Red, "Sobel")  }); //Rx
                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Red, "SobelT") }); //Ry

                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Green, "Sobel")  }); //Gx
                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Green, "SobelT") }); //Gy

                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Blue, "Sobel")  }); //Bx
                    filt.Add(new ArraysListDouble() { Color = ImageFilter.Filter_double(Blue, "SobelT") }); //By
                }

                if (variant == CountourVariant.Variant1_BW)
                {
                    //gradient for one color component B&W result
                    if (!type)
                    {
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color).ImageArrayToUint8();
                    }
                    else
                    {
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color, filt[2].Color,
                            filt[3].Color, filt[4].Color, filt[5].Color).ImageArrayToUint8();
                    }
                    resultG = resultR; resultB = resultR; //Black & White result                  
                }
                else if (variant == CountourVariant.Variant2_BW)
                {
                    //gradient for one color component B&W result
                    if (!type)
                    {
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color).ArrayMultByConst(2).ImageArrayToUint8();
                    }
                    else
                    {
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color, filt[2].Color,
                           filt[3].Color, filt[4].Color, filt[5].Color).ArrayMultByConst(2).ImageArrayToUint8();
                    }
                    resultG = resultR; resultB = resultR; //Black & White result                   
                }
                else
                {
                    if (type)
                    {
                        //RGB gradients       
                        var RG = (filt[0].Color).PowArrayElements(2).SumArrays((filt[1].Color).PowArrayElements(2)).SqrtArrayElements(); //R gradient               
                        var GG = (filt[2].Color).PowArrayElements(2).SumArrays((filt[3].Color).PowArrayElements(2)).SqrtArrayElements(); //G gradient                   
                        var BG = (filt[4].Color).PowArrayElements(2).SumArrays((filt[5].Color).PowArrayElements(2)).SqrtArrayElements(); //B gradient

                        resultR = RG.ArrayToUint8(); resultG = GG.ArrayToUint8(); resultB = BG.ArrayToUint8();                   
                    }
                    else
                    {
                        Console.WriteLine("Need RGB image for Variant5_RGB as input. Contour Method.");
                        return image;
                    }
                }
            }

            else if (variant == CountourVariant.Variant3_BW || variant == CountourVariant.Variant4_BW)
            {
                //convert image into gray scale
                var gray = MoreHelpers.BlackandWhiteProcessHelper(img);

                double[,] GG = new double[img.Height, img.Height]; //gray gradient

                if (variant == CountourVariant.Variant3_BW)
                {
                    var Gx = ImageFilter.Filter_double(gray, "Sobel");
                    var Gy = ImageFilter.Filter_double(gray, "SobelT");

                    GG = Gx.PowArrayElements(2).SumArrays(Gy.PowArrayElements(2)).SqrtArrayElements();                  
                }
                else
                {
                    var Gx = ImageFilter.Filter_int(gray, "Sobel");
                    var Gy = ImageFilter.Filter_int(gray, "SobelT");

                    GG = Gx.ArrayToDouble().PowArrayElements(2).SumArrays(Gy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements();                  
                }

                resultR = GG.ArrayToUint8(); resultG = resultR; resultB = resultR;
            }
            else if (variant == CountourVariant.Variant6_RGB)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with int
                if (type)
                {
                    var Rix = ImageFilter.Filter_int(Red, "Sobel");
                    var Riy = ImageFilter.Filter_int(Red, "SobelT");

                    var Gix = ImageFilter.Filter_int(Green, "Sobel");
                    var Giy = ImageFilter.Filter_int(Green, "SobelT");

                    var Bix = ImageFilter.Filter_int(Blue, "Sobel");
                    var Biy = ImageFilter.Filter_int(Blue, "SobelT");

                    var RG = Rix.ArrayToDouble().PowArrayElements(2).SumArrays(Riy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //R gradient               
                    var GG = Gix.ArrayToDouble().PowArrayElements(2).SumArrays(Giy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //G gradient             
                    var BG = Bix.ArrayToDouble().PowArrayElements(2).SumArrays(Biy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //B gradient

                    resultR = RG.ArrayToUint8(); resultG = GG.ArrayToUint8(); resultB = BG.ArrayToUint8();                  
                }
                else
                {
                    Console.WriteLine("Need RGB image for Variant6_RGB as input. Contour Method.");
                    return image;
                }
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);           

            if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }

            return image;
        }
    }

    public enum CountourVariant
    {
        [Description("_ContourV1_BW")]
        Variant1_BW,
        [Description("_ContourV2_BW")]
        Variant2_BW,
        [Description("_ContourV3_BW")]
        Variant3_BW,
        [Description("_ContourV4_BW")]
        Variant4_BW,
        [Description("_ContourV5_RGB")]
        Variant5_RGB,
        [Description("_ContourV6_RGB")]
        Variant6_RGB
    }

    //experiment with enum description. for now only at this class
    public static class EnumExtensions
    {
        public static string GetEnumDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return value.ToString();
        }
    }
}
