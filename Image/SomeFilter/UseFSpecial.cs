using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;
using Image.ColorSpaces;

namespace Image
{
    public static class UseFSpecial
    {       
        private static List<string> SharpVariants = new List<string>()
        { "_FSpecialRGB_", "_FSpecialHSV_", "_FSpecialLab_" };        

        //
        public static void ApplyFilter(Bitmap img, double[,] filter, FSpecialColorSpace cSpace, FSpecialFilterType filterType)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("FSpecial");            

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = FSpecialHelper(img, filter, cSpace, filterType);

            string outName = defPath + imgName + SharpVariants.ElementAt((int)cSpace) + filterType.ToString() + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        //
        public static Bitmap ApplyFilterBitmap(Bitmap img, double[,] filter, FSpecialColorSpace cSpace, FSpecialFilterType filterType)
        {
            return FSpecialHelper(img, filter, cSpace, filterType);
        }

        //
        private static Bitmap FSpecialHelper(Bitmap img,  double[,] filter, FSpecialColorSpace cSpace, FSpecialFilterType filterType)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            List<ArraysListInt> Result = new List<ArraysListInt>();
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            double[,] Resultemp;

            if (!Checks.BinaryInput(img))
            {
                List<ArraysListInt> ColorList = Helpers.GetPixels(img);

                //sharp in choosen color space
                switch (cSpace)
                {
                    case FSpecialColorSpace.RGB:
                        if (Depth == 8)
                        {
                            var bw = FSpecialFilterHelper(ColorList[0].Color.ArrayToDouble(), filter, filterType).ArrayToUint8();
                            Result.Add(new ArraysListInt() { Color = bw }); Result.Add(new ArraysListInt() { Color = bw });
                            Result.Add(new ArraysListInt() { Color = bw });
                        }
                        else
                        {
                            Result.Add(new ArraysListInt()
                            { Color = FSpecialFilterHelper(ColorList[0].Color.ArrayToDouble(), filter, filterType).ArrayToUint8() }); //R
                            Result.Add(new ArraysListInt()
                            { Color = FSpecialFilterHelper(ColorList[1].Color.ArrayToDouble(), filter, filterType).ArrayToUint8() }); //G
                            Result.Add(new ArraysListInt()
                            { Color = FSpecialFilterHelper(ColorList[2].Color.ArrayToDouble(), filter, filterType).ArrayToUint8() }); //B   
                        }
                        break;                   

                    case FSpecialColorSpace.HSV:
                        var hsvd = RGBandHSV.RGB2HSV(img);
                        var hsvd_temp = FSpecialFilterHelper((hsvd[2].Color).ArrayMultByConst(100), filter, filterType);

                        //Filter by V - Value (Brightness/яркость)
                        //artificially if V > 1, make him 1
                        Resultemp = hsvd_temp.ArrayDivByConst(100).ToBorderGreaterZero(1);
                        Result = RGBandHSV.HSV2RGB(hsvd[0].Color, hsvd[1].Color, Resultemp);
                        break;

                    case FSpecialColorSpace.Lab:
                        var labd = RGBandLab.RGB2Lab(img);
                        var labd_temp = FSpecialFilterHelper(labd[0].Color, filter, filterType);

                        //Filter by L - lightness                    
                        Result = RGBandLab.Lab2RGB(labd_temp.ToBorderGreaterZero(255), labd[1].Color, labd[2].Color);
                        break;
                }

                image = Helpers.SetPixels(image, Result[0].Color, Result[1].Color, Result[2].Color);

                if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
            }
            else { Console.WriteLine("I don`t wont process binary image. Return black rectangle."); }

            return image;
        }    

        //filtering by double
        private static double[,] FSpecialFilterHelper(double[,] cPlane, double[,] filter, FSpecialFilterType filterType)
        {
            double[,] Result;

            if (filterType == FSpecialFilterType.laplacian || filterType == FSpecialFilterType.laplacofGauss)
            {
                Result = cPlane.SubArrays(ImageFilter.Filter_double(cPlane, filter, PadType.replicate));
            }
            else 
            {
                Result = ImageFilter.Filter_double(cPlane, filter, PadType.replicate);
            }

            return Result;
        }        
    }       

    public enum FSpecialColorSpace
    {
        RGB,        
        HSV,        
        Lab      
    }

    public enum FSpecialFilterType
    {
        average,
        gaussian,
        laplacian,
        laplacofGauss,
        unsharp,
        motion,
        disk
    }
}
