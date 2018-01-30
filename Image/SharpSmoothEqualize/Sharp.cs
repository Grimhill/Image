using Image.ArrayOperations;
using Image.ColorSpaces;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Image.SharpSmoothEqualize
{
    public static class Sharping
    {
        //only for RGB images, b&w 24bbp.
        public static void Sharp(Bitmap img, UnSharpInColorSpace cSpace, FilterType filterType, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string defPass = Directory.GetCurrentDirectory() + "\\Sharp\\unSharp\\";
            Checks.DirectoryExistance(defPass);

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            List<ArraysListInt> Result = new List<ArraysListInt>();
            double[,] Resultemp;
            string outName = String.Empty;

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (Depth == 8) { ImgExtension = ".png"; }

            switch (cSpace)
            {
                case UnSharpInColorSpace.RGB:
                    Result.Add(new ArraysListInt() { Color = UnSharpHelperInt(ColorList[0].Color, filterType.ToString()) }); //R
                    Result.Add(new ArraysListInt() { Color = UnSharpHelperInt(ColorList[1].Color, filterType.ToString()) }); //G
                    Result.Add(new ArraysListInt() { Color = UnSharpHelperInt(ColorList[2].Color, filterType.ToString()) }); //B             

                    outName = defPass + fileName + "_unSharpRGB_" + filterType.ToString() + ImgExtension;
                    break;

                case UnSharpInColorSpace.HSVi:
                    var hsvi = RGBandHSV.RGB2HSV(img);
                    var hsvi_temp = UnSharpHelperInt(((hsvi[2].Color).ArrayMultByConst(100).ArrayToUint8()), filterType.ToString());

                    //Filter by V - Value (Brightness/яркость)                  
                    Resultemp = hsvi_temp.ArrayToDouble().ArrayDivByConst(100).ToBorderGreaterZero(1);
                    Result = RGBandHSV.HSV2RGB(hsvi[0].Color, hsvi[1].Color, Resultemp);

                    outName = defPass + fileName + "_unSharpHSVi_" + filterType.ToString() + ImgExtension;
                    break;

                case UnSharpInColorSpace.HSVd:
                    var hsvd = RGBandHSV.RGB2HSV(img);
                    var hsvd_temp = UnSharpHelperDouble((hsvd[2].Color).ArrayMultByConst(100), filterType.ToString());

                    //Filter by V - Value (Brightness/яркость)
                    //artificially if V > 1, make him 1
                    Resultemp = hsvd_temp.ArrayDivByConst(100).ToBorderGreaterZero(1);
                    Result = RGBandHSV.HSV2RGB(hsvd[0].Color, hsvd[1].Color, Resultemp);

                    outName = defPass + fileName + "_unSharpHSVd_" + filterType.ToString() + ImgExtension;
                    break;

                case UnSharpInColorSpace.Labi:
                    var labi = RGBandLab.RGB2Lab(img);
                    var labi_temp = UnSharpHelperInt((labi[0].Color).ArrayToUint8(), filterType.ToString());

                    //Filter by L - lightness                                    
                    Result = RGBandLab.Lab2RGB(labi_temp.ArrayToDouble(), labi[1].Color, labi[2].Color);

                    outName = defPass + fileName + "_unSharpLabi_" + filterType.ToString() + ImgExtension;
                    break;

                case UnSharpInColorSpace.Labd:
                    var labd = RGBandLab.RGB2Lab(img);
                    var labd_temp = UnSharpHelperDouble(labd[0].Color, filterType.ToString());

                    //Filter by L - lightness                    
                    Result = RGBandLab.Lab2RGB(labd_temp.ToBorderGreaterZero(255), labd[1].Color, labd[2].Color);

                    outName = defPass + fileName + "_unSharpLabd_" + filterType.ToString() + ImgExtension;
                    break;

                case UnSharpInColorSpace.fakeCIE1976L:
                    var fakeCIE1976L = RGBandLab.RGB2Lab(img);
                    var fakeCIE1976L_temp = UnSharpHelperInt((fakeCIE1976L[0].Color).ArrayMultByConst(2.57).ArrayToUint8(), filterType.ToString());

                    //Filter by L - lightness
                    Result = RGBandLab.Lab2RGB(fakeCIE1976L_temp.ArrayToDouble().ArrayDivByConst(2.57), fakeCIE1976L[1].Color, fakeCIE1976L[2].Color);

                    outName = defPass + fileName + "_unSharpfakeCIE1976L_" + filterType.ToString() + ImgExtension;
                    break;
            }

            image = Helpers.SetPixels(image, Result[0].Color, Result[1].Color, Result[2].Color);
            outName = Checks.OutputFileNames(outName);

            if (Depth == 8)
            { image = MoreHelpers.Bbp24Gray2Gray8bppHelper(image); }

            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        private static int[,] UnSharpHelperInt(int[,] cPlane, string fType)
        {
            int[,] Result;

            if (fType == "unsharp")
            {
                Result = (Filter.Filter_double(cPlane, Filter.Dx3FWindow(fType))).ArrayToUint8();
            }
            else
            {
                Result = cPlane.SubArrays(Filter.Filter_int(cPlane, Filter.Ix3FWindow(fType), PadType.replicate)).Uint8Range();
            }

            return Result;
        }

        private static double[,] UnSharpHelperDouble(double[,] cPlane, string fType)
        {
            double[,] Result;
            if (fType == "unsharp")
            {
                Result = Filter.Filter_double(cPlane, Filter.Dx3FWindow(fType), PadType.replicate);
            }
            else
            {
                Result = cPlane.SubArrays(Filter.Filter_double(cPlane, Filter.Dx3FWindow(fType), PadType.replicate));
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
        fakeCIE1976L
    }

    public enum FilterType
    {
        unsharp = 0,
        Laplassian1 = 1,
        Laplassian2 = 2,
    }
}
