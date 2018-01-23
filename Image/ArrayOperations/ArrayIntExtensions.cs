using System;

namespace Image.ArrayOperations
{
    public static class ArrayIntExtensions
    {
        //Mult 2 array [i,j] elements
        public static int[,] ArrayMultElements(this int[,] x, int[,] y)
        {
            int rows = x.GetLength(0);
            int cols = y.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] * y[k, m];
                }
            }
            return z;
        }

        //Const Sub array elements
        public static int[,] ConstSubArrayElements(this int[,] x, int conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = conts - x[k, m];
                }
            }
            return z;
        }

        //Sub array elements with const
        public static int[,] ArraySubWithConst(this int[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] - (int)conts;
                }
            }
            return z;
        }

        //mult array elements with const
        public static int[,] ArrayMultByConst(this int[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] * (int)conts;
                }
            }
            return z;
        }

        //Sub 2 arrays [i,j] elements
        public static int[,] SubArrays(this int[,] x, int[,] y)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] sub = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    sub[k, m] = x[k, m] - y[k, m];
                }
            }
            return sub;
        }

        //sumarray elements [i,j]
        public static int[,] SumArrays(this int[,] x, int[,] y)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] sum = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    sum[k, m] = x[k, m] + y[k, m];
                }
            }

            return sum;
        }

        //to 0..255 range
        public static int[,] Uint8Range(this int[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] res = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    if (x[k, m] < 0)
                    {
                        res[k, m] = 0;
                    }
                    else if (x[k, m] > 255)
                    {
                        res[k, m] = 255;
                    }
                    else { res[k, m] = x[k, m]; }
                }
            }
            return res;
        }

        //abs array elements
        public static int[,] AbsArrayElements(this int[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] res = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    res[k, m] = Math.Abs(x[k, m]);
                }
            }
            return res;
        }

        public static int[,] PowArrayElements(this int[,] x, double pow)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] y = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = (int)Math.Pow(x[k, m], pow);
                }
            }
            return y;
        }
    }
}
