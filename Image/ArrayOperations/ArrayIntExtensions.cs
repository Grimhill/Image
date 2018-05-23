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

        //Sum array elements with const
        public static int[,] ArraySumWithConst(this int[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] + (int)conts;
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

        //returns array raised to the specified power
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

        //transpose array
        public static int[,] Transpose(this int[,] arr)
        {
            int w = arr.GetLength(0);
            int h = arr.GetLength(1);

            int[,] result = new int[h, w];

            for (int i = 0; i < w; i++)
            {
                for (int j = 0; j < h; j++)
                {
                    result[j, i] = arr[i, j];
                }
            }

            return result;
        }

        //return type int array
        public static int[,] DoubleArrayToInt(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] res = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    res[k, m] = Convert.ToInt32(x[k, m]);
                }
            }

            return res;
        }

        //convert uint8 to binary values by level
        public static int[,] Uint8ArrayToBinary(this int[,] x, double level)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] res = new int[rows, cols];

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (x[i, j] > 255 * level) //0..255 - uint8 range
                    {
                        res[i, j] = 1;
                    }
                    else
                    {
                        res[i, j] = 0;
                    }
                }
            }
            return res;
        }
    }
}
