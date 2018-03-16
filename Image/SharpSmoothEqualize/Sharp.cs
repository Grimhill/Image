using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;
using Image.ColorSpaces;

namespace Image
{
    public static class Sharping
    {
        private static List<string> SharpVariants = new List<string>()
        { "_unSharpRGB_", "_unSharpHSVi_", "_unSharpHSVd_", "_unSharpLabi_", "_unSharpLabd_", "_unSharpfakeCIE1976Labi_", "_unSharpfakeCIE1976Labd_" };
        
        //Sharpen the image and save to file
        public static void Sharp(Bitmap img, UnSharpInColorSpace cSpace, SharpFilterType filterType)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);           
            string defPath      = GetImageInfo.MyPath("Sharp\\unSharp");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = SharpHelper(img, cSpace, filterType);

            string outName = defPath + imgName + SharpVariants.ElementAt((int)cSpace) + filterType.ToString() + imgExtension;            
            
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        //return Sharpen bitmap
        public static Bitmap SharpBitmap(Bitmap img, UnSharpInColorSpace cSpace, SharpFilterType filterType)
        {
            return SharpHelper(img, cSpace, filterType);            
        }

        //Sharpen process in selected color space
        private static Bitmap SharpHelper(Bitmap img, UnSharpInColorSpace cSpace, SharpFilterType filterType)
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
                    case UnSharpInColorSpace.RGB:
                        if (Depth == 8)
                        {
                            var bw = UnSharpHelperInt(ColorList[0].Color, filterType.ToString());
                            Result.Add(new ArraysListInt() { Color = bw }); Result.Add(new ArraysListInt() { Color = bw });
                            Result.Add(new ArraysListInt() { Color = bw });
                        }
                        else
                        {
                            Result.Add(new ArraysListInt() { Color = UnSharpHelperInt(ColorList[0].Color, filterType.ToString()) }); //R
                            Result.Add(new ArraysListInt() { Color = UnSharpHelperInt(ColorList[1].Color, filterType.ToString()) }); //G
                            Result.Add(new ArraysListInt() { Color = UnSharpHelperInt(ColorList[2].Color, filterType.ToString()) }); //B  
                        }
                        break;

                    case UnSharpInColorSpace.HSVi:
                        var hsvi = RGBandHSV.RGB2HSV(img);
                        var hsvi_temp = UnSharpHelperInt(((hsvi[2].Color).ArrayMultByConst(100).ArrayToUint8()), filterType.ToString());

                        //Filter by V - Value (Brightness/яркость)                  
                        Resultemp = hsvi_temp.ArrayToDouble().ArrayDivByConst(100).ToBorderGreaterZero(1);
                        Result = RGBandHSV.HSV2RGB(hsvi[0].Color, hsvi[1].Color, Resultemp);
                        break;

                    case UnSharpInColorSpace.HSVd:
                        var hsvd = RGBandHSV.RGB2HSV(img);
                        var hsvd_temp = UnSharpHelperDouble((hsvd[2].Color).ArrayMultByConst(100), filterType.ToString());

                        //Filter by V - Value (Brightness/яркость)
                        //artificially if V > 1, make him 1
                        Resultemp = hsvd_temp.ArrayDivByConst(100).ToBorderGreaterZero(1);
                        Result = RGBandHSV.HSV2RGB(hsvd[0].Color, hsvd[1].Color, Resultemp);
                        break;

                    case UnSharpInColorSpace.Labi:
                        var labi = RGBandLab.RGB2Lab(img);
                        var labi_temp = UnSharpHelperInt((labi[0].Color).ArrayToUint8(), filterType.ToString());

                        //Filter by L - lightness                                    
                        Result = RGBandLab.Lab2RGB(labi_temp.ArrayToDouble(), labi[1].Color, labi[2].Color);
                        break;

                    case UnSharpInColorSpace.Labd:
                        var labd = RGBandLab.RGB2Lab(img);
                        var labd_temp = UnSharpHelperDouble(labd[0].Color, filterType.ToString());

                        //Filter by L - lightness                    
                        Result = RGBandLab.Lab2RGB(labd_temp.ToBorderGreaterZero(255), labd[1].Color, labd[2].Color);
                        break;

                    case UnSharpInColorSpace.fakeCIE1976Labi:
                        var fakeCIE1976abLi = RGBandLab.RGB2Lab1976(img);
                        var fakeCIE1976Labi_temp = UnSharpHelperInt((fakeCIE1976abLi[0].Color).ArrayToUint8(), filterType.ToString());

                        //Filter by L - lightness
                        Result = RGBandLab.Lab1976toRGB(fakeCIE1976Labi_temp.ArrayToDouble(), fakeCIE1976abLi[1].Color, fakeCIE1976abLi[2].Color);
                        break;

                    case UnSharpInColorSpace.fakeCIE1976Labd:
                        var fakeCIE1976Labd = RGBandLab.RGB2Lab1976(img);
                        var fakeCIE1976Labd_temp = UnSharpHelperDouble((fakeCIE1976Labd[0].Color), filterType.ToString());

                        //Filter by L - lightness
                        Result = RGBandLab.Lab1976toRGB(fakeCIE1976Labd_temp.ToBorderGreaterZero(255), fakeCIE1976Labd[1].Color, fakeCIE1976Labd[2].Color);
                        break;
                }

                image = Helpers.SetPixels(image, Result[0].Color, Result[1].Color, Result[2].Color);

                if (Depth == 8)
                { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
            }
            else { Console.WriteLine("What did you expected to sharp binary image? Return black rectangle."); }

            return image;
        }

        //filtering by int
        private static int[,] UnSharpHelperInt(int[,] cPlane, string fType)
        {
            int[,] Result;

            if (fType == "unsharp")
            {
                Result = (ImageFilter.Filter_double(cPlane, ImageFilter.Dx3FWindow(fType))).ArrayToUint8();
            }
            else
            {
                Result = cPlane.SubArrays(ImageFilter.Filter_int(cPlane, ImageFilter.Ix3FWindow(fType), PadType.replicate)).Uint8Range();
            }

            return Result;
        }

        //filtering by double
        private static double[,] UnSharpHelperDouble(double[,] cPlane, string fType)
        {
            double[,] Result;
            if (fType == "unsharp")
            {
                Result = ImageFilter.Filter_double(cPlane, ImageFilter.Dx3FWindow(fType), PadType.replicate);
            }
            else
            {
                Result = cPlane.SubArrays(ImageFilter.Filter_double(cPlane, ImageFilter.Dx3FWindow(fType), PadType.replicate));
            }

            return Result;
        }
    }

    public enum UnSharpInColorSpace
    {
        RGB,
        HSVi,
        HSVd,
        Labi,
        Labd,
        fakeCIE1976Labi,
        fakeCIE1976Labd
    }

    public enum SharpFilterType
    {
        unsharp,
        Laplacian1,
        Laplacian2
    }
}
