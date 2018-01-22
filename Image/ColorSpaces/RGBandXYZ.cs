﻿using System;
using System.Collections.Generic;
using System.Drawing;

namespace Image.ColorSpaces
{
    public static class RGBandXYZ
    {
        #region rgb2xyz

        //sRGB to XYZ D65/2
        public static List<ArraysListDouble> RGB2XYZ(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            xyzResult = RGB2XYZCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return xyzResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2XYZ(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2xyz operation -> rgb2xyz(List<arraysListInt> rgbList) <-");
            }
            else
            {
                xyzResult = RGB2XYZCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return xyzResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2XYZ(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2xyz operation -> rgb2xyz(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                xyzResult = RGB2XYZCount(R, G, B);
            }

            return xyzResult;
        }

        //X Y Z values - double > 1, can be <1 if represent small R G B values
        public static List<ArraysListDouble> RGB2XYZCount(int[,] R, int[,] G, int[,] B)
        {
            //ArrayOperations ArrOp = new ArrayOperations();
            int width  = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            var Rcd = R.ImageUint8ToDouble(); //ArrOp.ImageUint8ToDouble(R);
            var Gcd = G.ImageUint8ToDouble();//ArrOp.ImageUint8ToDouble(G);
            var Bcd = B.ImageUint8ToDouble(); //ArrOp.ImageUint8ToDouble(B);

            double[,] R_temp = new double[height, width];
            double[,] G_temp = new double[height, width];
            double[,] B_temp = new double[height, width];

            double[,] X = new double[height, width];
            double[,] Y = new double[height, width];
            double[,] Z = new double[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (Rcd[i, j] > 0.04045)
                    {
                        R_temp[i, j] = Math.Pow((Rcd[i, j] + 0.055) / 1.055, 2.4);
                    }
                    else
                    {
                        R_temp[i, j] = Rcd[i, j] / 12.92;
                    }

                    if (Gcd[i, j] > 0.04045)
                    {
                        G_temp[i, j] = Math.Pow((Gcd[i, j] + 0.055) / 1.055, 2.4);
                    }
                    else
                    {
                        G_temp[i, j] = Gcd[i, j] / 12.92;
                    }

                    if (Bcd[i, j] > 0.04045)
                    {
                        B_temp[i, j] = Math.Pow((Bcd[i, j] + 0.055) / 1.055, 2.4);
                    }
                    else
                    {
                        B_temp[i, j] = Bcd[i, j] / 12.92;
                    }

                    R_temp[i, j] = R_temp[i, j] * 100;
                    G_temp[i, j] = G_temp[i, j] * 100;
                    B_temp[i, j] = B_temp[i, j] * 100;

                    //rgb2xyz_D65
                    X[i, j] = R_temp[i, j] * 0.4124 + G_temp[i, j] * 0.3576 + B_temp[i, j] * 0.1805;
                    Y[i, j] = R_temp[i, j] * 0.2126 + G_temp[i, j] * 0.7152 + B_temp[i, j] * 0.0722;
                    Z[i, j] = R_temp[i, j] * 0.0193 + G_temp[i, j] * 0.1192 + B_temp[i, j] * 0.9505;
                }
            }

            xyzResult.Add(new ArraysListDouble() { Color = X });
            xyzResult.Add(new ArraysListDouble() { Color = Y });
            xyzResult.Add(new ArraysListDouble() { Color = Z });

            return xyzResult;
        }

        #endregion rgb2xyz

        #region xyz2rgb

        //XYZ D65/2 to sRGB
        public static List<ArraysListInt> XYZ2RGB(Bitmap img)
        {
            //ArrayOperations ArrOp = new ArrayOperations(); 
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            var X = (ColorList[0].Color).ArrayToDouble(); //ArrOp.ArrayToDouble(ColorList[0].Color);
            var Y = (ColorList[1].Color).ArrayToDouble();  //ArrOp.ArrayToDouble(ColorList[1].Color);
            var Z = (ColorList[2].Color).ArrayToDouble();  //ArrOp.ArrayToDouble(ColorList[2].Color);

            rgbResult = XYZ2RGBbCount(X, Y, Z);

            return rgbResult;
        }

        //X Y Z in double values (as after convert rgb2XYZ, not in range [0 1], only if represent small R G B values)
        //list X Y Z arrays in In the following order X-Y-Z
        public static List<ArraysListInt> XYZ2RGB(List<ArraysListDouble> xyzList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (xyzList[0].Color.Length != xyzList[1].Color.Length || xyzList[0].Color.Length != xyzList[2].Color.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2rgb operation -> xyz2rgb(List<arraysListDouble> xyzList) <-");
            }
            else
            {
                rgbResult = XYZ2RGBbCount(xyzList[0].Color, xyzList[1].Color, xyzList[2].Color);
            }

            return rgbResult;
        }

        //X Y Z in double values (as after convert rgb2XYZ, not in range [0 1], only if represent small R G B values)
        //X Y Z arrays in In the following order X-Y-Z
        public static List<ArraysListInt> XYZ2RGB(double[,] X, double[,] Y, double[,] Z)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (X.Length != Y.Length || X.Length != Z.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2rgb operation -> xyz2rgb(double[,] X, double[,] Y, double[,] Z) <-");
            }
            else
            {
                rgbResult = XYZ2RGBbCount(X, Y, Z);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> XYZ2RGBbCount(double[,] X, double[,] Y, double[,] Z)
        {
            //ArrayOperations ArrOp = new ArrayOperations();
            int width  = X.GetLength(1);
            int height = X.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            double[,] R_temp = new double[height, width];
            double[,] G_temp = new double[height, width];
            double[,] B_temp = new double[height, width];

            double[,] R = new double[height, width];
            double[,] G = new double[height, width];
            double[,] B = new double[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    X[i, j] = X[i, j] / 100;
                    Y[i, j] = Y[i, j] / 100;
                    Z[i, j] = Z[i, j] / 100;

                    R_temp[i, j] = X[i, j] * 3.2406 + Y[i, j] * (-1.5372) + Z[i, j] * (-0.4986);
                    G_temp[i, j] = X[i, j] * (-0.9689) + Y[i, j] * 1.8758 + Z[i, j] * 0.0415;
                    B_temp[i, j] = X[i, j] * 0.0557 + Y[i, j] * (-0.2040) + Z[i, j] * 1.057;

                    if (R_temp[i, j] > 0.0031308)
                    {
                        R[i, j] = 1.055 * (Math.Pow(R_temp[i, j], (1 / 2.4))) - 0.055;
                    }
                    else
                    {
                        R[i, j] = R_temp[i, j] * 12.92;
                    }

                    if (G_temp[i, j] > 0.0031308)
                    {
                        G[i, j] = 1.055 * (Math.Pow(G_temp[i, j], (1 / 2.4))) - 0.055;
                    }
                    else
                    {
                        G[i, j] = G_temp[i, j] * 12.92;
                    }

                    if (B_temp[i, j] > 0.0031308)
                    {
                        B[i, j] = 1.055 * (Math.Pow(B_temp[i, j], (1 / 2.4))) - 0.055;
                    }
                    else
                    {
                        B[i, j] = B_temp[i, j] * 12.92;
                    }
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = R.ImageArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = G.ImageArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = B.ImageArrayToUint8() });

            return rgbResult;
        }

        #endregion xyz2rgb
    }
}
