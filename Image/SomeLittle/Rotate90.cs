using System;
using System.Linq;
using Image.ArrayOperations;

namespace Image
{
    public static class Rotate90
    {
        //negative value - rotate in another way
        public static int[,] RotateArray90(int[,] arr, int howmanytimes)
        {
            return RotateArray90(arr.ArrayToDouble(), howmanytimes).DoubleArrayToInt();
        }

        public static double[,] RotateArray90(double[,] arr, int howmanytimes) 
        {
            var a = Math.Round(Math.PI / 180 * (-90 * howmanytimes), 4);

            double[,] Tform = new double[3, 3] {
                { Math.Cos(a),  Math.Sin(a), 0 },
                { (-1) * Math.Sin(a), Math.Cos(a), 0 },
                { 0, 0, 1 } };

            int r = (int)Math.Round(Math.Abs(Math.Cos(a) * arr.GetLength(0)) + Math.Abs(Math.Sin(a) * arr.GetLength(1)));
            int c = (int)Math.Round(Math.Abs(Math.Cos(a) * arr.GetLength(1)) + Math.Abs(Math.Sin(a) * arr.GetLength(0)));

            double[,] result = new double[r, c];

            if (howmanytimes > -4 || howmanytimes < 4)
            {
                result = Rotate90Helper(arr, Tform, r, c);
            }
            else
            { Console.WriteLine("Thre no point to rotate 90 more, than 3 times."); }

            return result;
        }

        private static double[,] Rotate90Helper(double[,] arr, double[,] tform, int r, int c)
        {
            double[,] result = new double[r, c];
            int[,] X = new int[arr.GetLength(0), arr.GetLength(1)];
            int[,] Y = new int[arr.GetLength(0), arr.GetLength(1)];

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    double[,] temp = new double[1, 3] { { i, j, 1 } };
                    var tempRes = temp.MultArrays(tform);

                    var x = (int)Math.Round(tempRes[0, 0]);
                    var y = (int)Math.Round(tempRes[0, 1]);

                    X[i, j] = x;
                    Y[i, j] = y;
                }
            }

            if (X.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(X.Cast<int>().Min());
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        X[i, j] = X[i, j] + min;
                    }
                }

            }

            if (Y.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(Y.Cast<int>().Min());
                for (int i = 0; i < Y.GetLength(0); i++)
                {
                    for (int j = 0; j < Y.GetLength(1); j++)
                    {
                        Y[i, j] = Y[i, j] + min;
                    }
                }
            }
           
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    result[X[i, j], Y[i, j]] = arr[i, j];
                }
            }

            return result;
        }
    }
}
