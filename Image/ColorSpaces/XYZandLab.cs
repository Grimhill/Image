using System;
using System.Drawing;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image.ColorSpaces
{
    public static class XYZandLab
    {
        #region xyz2lab

        //bad when from file, coz lost a lot after round values in all planes; when saved xyz result to file
        public static List<ArraysListDouble> XYZ2Lab(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            var X = (ColorList[0].Color).ArrayToDouble();
            var Y = (ColorList[1].Color).ArrayToDouble();
            var Z = (ColorList[2].Color).ArrayToDouble();

            labResult = XYZ2LabCount(X, Y, Z);

            return labResult;
        }

        //List with X Y Z arrays in In the following order X Y Z
        public static List<ArraysListDouble> XYZ2Lab(List<ArraysListDouble> xyzList)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (xyzList[0].Color.Length != xyzList[1].Color.Length || xyzList[0].Color.Length != xyzList[2].Color.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2lab operation -> xyz2lab(List<arraysListInt> xyzList) <-");
            }
            else
            {
                labResult = XYZ2LabCount(xyzList[0].Color, xyzList[1].Color, xyzList[2].Color);
            }

            return labResult;
        }

        //X Y Z arrays in In the following order X Y Z
        public static List<ArraysListDouble> XYZ2Lab(double[,] x, double[,] y, double[,] z)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (x.Length != y.Length || x.Length != z.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2lab operation -> xyz2lab(double[,] X, double[,] Y, double[,] Z) <-");
            }
            else
            {
                labResult = XYZ2LabCount(x, y, z);
            }

            return labResult;
        }

        //L values - double, not in [0 1] range; a & b - same, but have negative values
        private static List<ArraysListDouble> XYZ2LabCount(double[,] x, double[,] y, double[,] z)
        {
            int width  = x.GetLength(1);
            int height = x.GetLength(0);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            const double X_D65 = 95.047;
            const double Y_D65 = 100;
            const double Z_D65 = 108.883;

            x = x.ArrayDivByConst(X_D65);
            y = y.ArrayDivByConst(Y_D65);
            z = z.ArrayDivByConst(Z_D65);

            double[,] X_temp = new double[height, width];
            double[,] Y_temp = new double[height, width];
            double[,] Z_temp = new double[height, width];

            double[,] L = new double[height, width]; //lightness
            double[,] a = new double[height, width]; //color opponent green–red
            double[,] b = new double[height, width]; //color opponent and blue–yellow

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (x[i, j] > 0.008856)
                    {
                        X_temp[i, j] = Math.Pow(x[i, j], (1d / 3d));
                    }
                    else
                    {
                        X_temp[i, j] = (7.787 * x[i, j]) + (16d / 116d);
                    }

                    if (y[i, j] > 0.008856)
                    {
                        Y_temp[i, j] = Math.Pow(y[i, j], (1d / 3d));
                    }
                    else
                    {
                        Y_temp[i, j] = (7.787 * y[i, j]) + (16d / 116d);
                    }

                    if (z[i, j] > 0.008856)
                    {
                        Z_temp[i, j] = Math.Pow(z[i, j], (1d / 3d));
                    }
                    else
                    {
                        Z_temp[i, j] = (7.787 * z[i, j]) + (16d / 116d);
                    }

                    L[i, j] = (116 * Y_temp[i, j]) - 16;
                    a[i, j] = 500 * (X_temp[i, j] - Y_temp[i, j]);
                    b[i, j] = 200 * (Y_temp[i, j] - Z_temp[i, j]);
                }
            }

            labResult.Add(new ArraysListDouble() { Color = L });
            labResult.Add(new ArraysListDouble() { Color = a });
            labResult.Add(new ArraysListDouble() { Color = b });

            return labResult;
        }

        #endregion xyz2lab

        #region lab2xyz

        //bad when from file, coz lost a and b negative value when save to file
        public static List<ArraysListDouble> Lab2XYZ(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            double[,] L = (ColorList[0].Color).ArrayToDouble();
            double[,] a = (ColorList[1].Color).ArrayToDouble();
            double[,] b = (ColorList[2].Color).ArrayToDouble();

            xyzResult = Lab2XYZCount(L, a, b);

            return xyzResult;
        }

        //L a b in double values (as after convert rgb2XYZ, not in range [0 1])
        //list L a b arrays in In the following order L-a-b
        public static List<ArraysListDouble> Lab2XYZ(List<ArraysListDouble> labList)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (labList[0].Color.Length != labList[1].Color.Length || labList[0].Color.Length != labList[2].Color.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2xyz operation -> lab2xyz(List<arraysListDouble> labList) <-");
            }
            else
            {
                xyzResult = Lab2XYZCount(labList[0].Color, labList[1].Color, labList[2].Color);
            }

            return xyzResult;
        }

        //L a b in double values (as after convert rgb2XYZ, not in range [0 1])
        //L a b arrays in In the following order L-a-b
        public static List<ArraysListDouble> Lab2XYZ(double[,] l, double[,] a, double[,] b)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (l.Length != a.Length || l.Length != b.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2xyz operation -> lab2xyz(double[,] L, double[,] a, double[,] b) <-");
            }
            else
            {
                xyzResult = Lab2XYZCount(l, a, b);
            }

            return xyzResult;
        }

        //X Y Z values - double > 1; can be <1 if represent small R G B values
        private static List<ArraysListDouble> Lab2XYZCount(double[,] l, double[,] a, double[,] b)
        {
            int width  = l.GetLength(1);
            int height = l.GetLength(0);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            const double X_D65 = 95.047;
            const double Y_D65 = 100;
            const double Z_D65 = 108.883;

            double[,] X_temp = new double[height, width];
            double[,] Y_temp = new double[height, width];
            double[,] Z_temp = new double[height, width];

            double[,] X = new double[height, width];
            double[,] Y = new double[height, width];
            double[,] Z = new double[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Y_temp[i, j] = (l[i, j] + 16) / 116;
                    X_temp[i, j] = a[i, j] / 500 + Y_temp[i, j];
                    Z_temp[i, j] = Y_temp[i, j] - b[i, j] / 200;

                    if (Math.Pow(Y_temp[i, j], 3) > 0.008856)
                    {
                        Y_temp[i, j] = Math.Pow(Y_temp[i, j], 3);
                    }
                    else
                    {
                        Y_temp[i, j] = (Y_temp[i, j] - 16d / 116d) / 7.787;
                    }

                    if (Math.Pow(X_temp[i, j], 3) > 0.008856)
                    {
                        X_temp[i, j] = Math.Pow(X_temp[i, j], 3);
                    }
                    else
                    {
                        X_temp[i, j] = (X_temp[i, j] - 16d / 116d) / 7.787;
                    }

                    if (Math.Pow(Z_temp[i, j], 3) > 0.008856)
                    {
                        Z_temp[i, j] = Math.Pow(Z_temp[i, j], 3);
                    }
                    else
                    {
                        Z_temp[i, j] = (Z_temp[i, j] - 16d / 116d) / 7.787;
                    }

                    X[i, j] = X_temp[i, j] * X_D65;
                    Y[i, j] = Y_temp[i, j] * Y_D65;
                    Z[i, j] = Z_temp[i, j] * Z_D65;
                }
            }

            xyzResult.Add(new ArraysListDouble() { Color = X });
            xyzResult.Add(new ArraysListDouble() { Color = Y });
            xyzResult.Add(new ArraysListDouble() { Color = Z });

            return xyzResult;
        }

        #endregion lab2xyz
    }
}
