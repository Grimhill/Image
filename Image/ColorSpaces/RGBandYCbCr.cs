﻿using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image.ColorSpaces
{
    public static class RGBandYCbCr
    {
        #region rgb2YCbCr

        public static List<ArraysListDouble> RGB2YCbCr(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            YCbCrResult = RGB2YCbCrCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return YCbCrResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2YCbCr(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2YCbCr operation -> rgb2YCbCr(List<arraysListInt> rgbList) <-");
            }
            else
            {
                YCbCrResult = RGB2YCbCrCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return YCbCrResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2YCbCr(int[,] r, int[,] g, int[,] b)
        {
            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            if (r.Length != g.Length || r.Length != b.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2YCbCr operation -> rgb2YCbCr(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                YCbCrResult = RGB2YCbCrCount(r, g, b);
            }

            return YCbCrResult;
        }

        //Y Cb Cr values - double, not in range [0 1]
        private static List<ArraysListDouble> RGB2YCbCrCount(int[,] r, int[,] g, int[,] b)
        {
            int width  = r.GetLength(1);
            int height = r.GetLength(0);

            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            var Rcd = r.ImageUint8ToDouble();
            var Gcd = g.ImageUint8ToDouble();
            var Bcd = b.ImageUint8ToDouble();

            double[,] Y  = new double[height, width]; //luma (яркостная составляющая)
            double[,] Cb = new double[height, width]; //difference between B and Y ?
            double[,] Cr = new double[height, width]; //difference between R and Y ?

            double[] Ycon  = new double[3] { 65.481, 128.553, 24.966 };
            double[] Cbcon = new double[3] { -37.797, -74.203, 112 };
            double[] Crcon = new double[3] { 112, -93.786, -18.214 };
            double[] Coef  = new double[3] { 16, 128, 128 };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { Rcd[i, j], Gcd[i, j], Bcd[i, j] };

                    Y[i, j]  = Coef[0] + Ycon.MultVectors(temp).Sum();
                    Cb[i, j] = Coef[1] + Cbcon.MultVectors(temp).Sum();
                    Cr[i, j] = Coef[2] + Crcon.MultVectors(temp).Sum();
                }
            }

            YCbCrResult.Add(new ArraysListDouble() { Color = Y });
            YCbCrResult.Add(new ArraysListDouble() { Color = Cb });
            YCbCrResult.Add(new ArraysListDouble() { Color = Cr });

            return YCbCrResult;
        }

        #endregion rgb2YCbCr

        #region YCbCr2rgb

        public static List<ArraysListInt> YCbCr2RGB(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            var Y  = (ColorList[0].Color).ArrayToDouble();
            var Cb = (ColorList[1].Color).ArrayToDouble();
            var Cr = (ColorList[2].Color).ArrayToDouble();

            rgbResult = YCbCr2RGBCount(Y, Cb, Cr);

            return rgbResult;
        }

        //Y Cb Cr in double values (as after convert rgb2YCbCr, not in range [-1 1])
        //list Y Cb Cr arrays in In the following order Y-Cb-Cr
        public static List<ArraysListInt> YCbCr2RGB(List<ArraysListDouble> YCbCrList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (YCbCrList[0].Color.Length != YCbCrList[1].Color.Length || YCbCrList[0].Color.Length != YCbCrList[2].Color.Length)
            {
                Console.WriteLine("Y Cb Cr arrays size dismatch in YCbCr2rgb operation -> YCbCr2rgb(List<arraysListDouble> YCbCrList) <-");
            }
            else if (YCbCrList[0].Color.Cast<double>().ToArray().Max() < 1)
            {
                //may be need transform?
                //YCbCrList[0].Color = (YCbCrList[0].Color).ArrayMultByConst(255);
                //YCbCrList[1].Color = (YCbCrList[1].Color).ArrayMultByConst(255);  
                //YCbCrList[2].Color = (YCbCrList[2].Color).ArrayMultByConst(255);  
                Console.WriteLine("Y Cb Cr arrays Values must be not in range [-1 1]; in YCbCr2rgb operation -> YCbCr2rgb(List<arraysListDouble> YCbCrList) <-");
            }
            else
            {
                rgbResult = YCbCr2RGBCount(YCbCrList[0].Color, YCbCrList[1].Color, YCbCrList[2].Color);
            }

            return rgbResult;
        }

        //Y Cb Cr in double values (as after convert rgb2YCbCr, not in range [-1 1])
        //Y Cb Cr arrays in In the following order Y-Cb-Cr
        public static List<ArraysListInt> YCbCr2RGB(double[,] y, double[,] cb, double[,] cr)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (y.Length != cb.Length || y.Length != cr.Length)
            {
                Console.WriteLine("Y Cb Cr arrays size dismatch in YCbCr2rgb operation -> YCbCr2rgb(double[,] Y, double[,] Cb, double[,] Cr) <-");
            }
            else if (y.Cast<double>().ToArray().Max() < 1)
            {
                //may be need transform?
                //Y  = Y.ArrayMultByConst(255);  
                //Cb = Cb.ArrayMultByConst(255); 
                //Cr = Cr.ArrayMultByConst(255); 
                Console.WriteLine("Y Cb Cr arrays Values must be not in range [-1 1]; in YCbCr2rgb operation -> YCbCr2rgb(double[,] Y, double[,] Cb, double[,] Cr) <-");
            }
            else
            {
                rgbResult = YCbCr2RGBCount(y, cb, cr);
            }

            return rgbResult;
        }

        private static List<ArraysListInt> YCbCr2RGBCount(double[,] y, double[,] cb, double[,] cr)
        {
            int width = y.GetLength(1);
            int height = y.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            y  = y.ArrayDivByConst(255);
            cb = cb.ArrayDivByConst(255);
            cr = cr.ArrayDivByConst(255);

            double[,] R = new double[height, width];
            double[,] G = new double[height, width];
            double[,] B = new double[height, width];

            double[] Ycon  = new double[3] { 298.082, 0, 408.583 };
            double[] Cbcon = new double[3] { 298.082, -100.291, -208.12 };
            double[] Crcon = new double[3] { 298.082, 516.412, 0 };
            double[] Coef  = new double[3] { -222.921, 135.576, -276.836 };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { y[i, j], cb[i, j], cr[i, j] };

                    R[i, j] = Coef[0] + Ycon.MultVectors(temp).Sum();
                    G[i, j] = Coef[1] + Cbcon.MultVectors(temp).Sum();
                    B[i, j] = Coef[2] + Crcon.MultVectors(temp).Sum();
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = R.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = G.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = B.ArrayToUint8() });

            return rgbResult;
        }

        #endregion YCbCr2rgb
    }
}
