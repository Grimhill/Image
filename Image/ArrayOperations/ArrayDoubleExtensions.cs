using System;

namespace Image.ArrayOperations
{
    public static class ArrayDoubleExtensions
    {
        //Mult 2 arrays by rule
        public static double[,] MultArrays(this double[,] x, double[,] y)
        {
            int rows = x.GetLength(0);
            int cols = y.GetLength(1);
            double[,] z = new double[rows, cols];

            if (x.GetLength(1) == y.GetLength(0))
            {
                for (int k = 0; k < rows; k++)
                {
                    for (int m = 0; m < cols; m++)
                    {
                        z[k, m] = 0;
                        for (int l = 0; l < x.GetLength(1); l++)
                        {
                            z[k, m] += x[k, l] * y[l, m];
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Number of cols in firts array dismatch with number of rows in second. Operation -> MultArrays(double[,] x, double[,] y) <-");
            }

            return z;
        }

        //Mult 2 arrays [i,j] elements
        public static double[,] ArrayMultElements(this double[,] x, double[,] y)
        {
            int rows = x.GetLength(0);
            int cols = y.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] * y[k, m];
                }
            }
            return z;
        }

        //Sum 2 arrays [i,j] elements
        public static double[,] SumArrays(this double[,] x, double[,] y)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] sum = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    sum[k, m] = x[k, m] + y[k, m];
                }
            }

            return sum;
        }

        //Sub 2 arrays [i,j] elements
        public static double[,] SubArrays(this double[,] x, double[,] y)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] sub = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    sub[k, m] = x[k, m] - y[k, m];
                }
            }

            return sub;
        }

        //Div 2 arrays [i,j] elements
        public static double[,] ArraydivElements(this double[,] x, double[,] y)
        {
            int rows = x.GetLength(0);
            int cols = y.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    if (y[k, m] == 0)
                    {
                        z[k, m] = 0; //retarding div by zero evision
                    }
                    else
                    {
                        z[k, m] = x[k, m] / y[k, m];
                    }
                }
            }
            return z;
        }

        //Mult array elements by const
        public static double[,] ArrayMultByConst(this double[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] * conts;
                }
            }
            return z;
        }

        //Sum array elements with const
        public static double[,] ArraySumWithConst(this double[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] + conts;
                }
            }
            return z;
        }

        //Sub array elements with const
        public static double[,] ArraySubWithConst(this double[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = x[k, m] - conts;
                }
            }
            return z;
        }

        //Div array elements by const
        public static double[,] ArrayDivByConst(this double[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    if (conts == 0)
                    {
                        Console.WriteLine("Cant div by 0");
                    }
                    else
                    {
                        z[k, m] = x[k, m] / conts;
                    }
                }
            }
            return z;
        }

        //Div const on array elements
        public static double[,] ConstDivByArrayElements(this double[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    if (x[k, m] == 0)
                    {
                        z[k, m] = 0; //retarding div by zero evision
                    }
                    else
                    {
                        z[k, m] = conts / x[k, m];
                    }
                }
            }
            return z;
        }

        //Sub const with array elements
        public static double[,] ConstSubArrayElements(this double[,] x, double conts)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = conts - x[k, m];
                }
            }
            return z;
        }

        //Pow array elements
        public static double[,] PowArrayElements(this double[,] x, double pow)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] y = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = Math.Pow(x[k, m], pow);
                }
            }
            return y;
        }

        //Sqrt array elements
        public static double[,] SqrtArrayElements(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] y = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = Math.Sqrt(x[k, m]);
                }
            }
            return y;
        }

        //Take natural log of array elements
        public static double[,] LogArrayElements(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] y = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = Math.Log(x[k, m]);
                }
            }
            return y;
        }

        //Exponent array elements
        public static double[,] ExpArrayElements(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] y = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = Math.Exp(x[k, m]);
                }
            }
            return y;
        }

        //obtain module of div array elements by const
        public static double[,] ModArrayElements(this double[,] x, double module)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] mod = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    mod[k, m] = x[k, m] % module;
                }
            }

            return mod;
        }

        //Abs array elements
        public static double[,] AbsArrayElements(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] res = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    res[k, m] = Math.Abs(x[k, m]);
                }
            }

            return res;
        }

        #region trigonometrical opeations
        //Atan, Sin & Cos array elements
        public static double[,] AtanArrayElements(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] y = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = Math.Atan(x[k, m]);
                }
            }
            return y;
        }

        public static double[,] SinArrayElements(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] y = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = Math.Sin(x[k, m]);
                }
            }
            return y;
        }

        public static double[,] CosArrayElements(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] y = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    y[k, m] = Math.Cos(x[k, m]);
                }
            }
            return y;
        }
        #endregion

        //Sum 3 arrays [i,j] elements
        public static double[,] SumThreeArrays(double[,] x, double[,] y, double[,] z)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] sum = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    sum[k, m] = x[k, m] + y[k, m] + z[k, m];
                }
            }

            return sum;
        }

        //Max 2 arrays [i,j] elements
        public static double[,] MaxTwoArrays(this double[,] x, double[,] y)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] arrayMax = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    arrayMax[k, m] = Math.Max(x[k, m], y[k, m]);
                }
            }

            return arrayMax;
        }

        //Prevent more than border. if more, take the border value
        public static double[,] ToBorderGreaterZero(this double[,] x, double border)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    if (x[k, m] > border)
                    {
                        z[k, m] = border;
                    }
                    else if (x[k, m] < 0)
                    {
                        z[k, m] = 0;
                    }
                    else
                    {
                        z[k, m] = x[k, m];
                    }

                }
            }
            return z;
        }

        public static double[] MinInColumns(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);

            double[] arr = new double[cols];

            int count = 0;
            for (int i = 0; i < cols; i++)
            {
                arr[count] = x[0, i];
                for (int j = 0; j < rows; j++)
                {
                    if (arr[count] > x[j, i])
                    {
                        arr[count] = x[j, i];
                    }
                }
                count++;
            }
            return arr;
        }

        //Area
        public static double[,] ArrayToDouble(this int[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = (double)x[k, m];
                }
            }
            return z;
        }

        public static int[,] ArrayToUint8(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    //if ((int)Math.Round(x[k, m]) > 255) { z[k, m] = 255; } else { z[k, m] = (int)Math.Round(x[k, m]); }
                    if (Convert.ToInt32(x[k, m]) > 255) { z[k, m] = 255; }
                    else if (Convert.ToInt32(x[k, m]) < 0) { z[k, m] = 0; }
                    else { z[k, m] = Convert.ToInt32(x[k, m]); }
                }
            }
            return z;
        }

        public static double[,] ImageUint8ToDouble(this int[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = (double)x[k, m] / (double)255;
                }
            }
            return z;
        }

        public static double[,] ImageUint16ToDouble(this int[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            double[,] z = new double[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = (double)x[k, m] / (double)65535;
                }
            }
            return z;
        }

        public static int[,] ImageArrayToUint8(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    if ((int)(x[k, m] * 255) < 0) { z[k, m] = 0; }
                    else if ((int)(x[k, m] * 255) > 255) { z[k, m] = 255; }
                    else { z[k, m] = (int)(x[k, m] * 255); }
                }
            }
            return z;
        }

        public static int[,] ImageArrayToUint16(this double[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    if ((int)(x[k, m] * 65535) < 0) { z[k, m] = 0; }
                    else if ((int)(x[k, m] * 65535) > 65535) { z[k, m] = 65535; }
                    else { z[k, m] = (int)(x[k, m] * 65535); }
                }
            }
            return z;
        }

        public static int[,] ImageUint16ToUint8(this int[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = (int)(((double)x[k, m] / (double)65535) * (double)255);
                }
            }
            return z;
        }

        public static int[,] ImageUint8ToUint16(this int[,] x)
        {
            int rows = x.GetLength(0);
            int cols = x.GetLength(1);
            int[,] z = new int[rows, cols];

            for (int k = 0; k < rows; k++)
            {
                for (int m = 0; m < cols; m++)
                {
                    z[k, m] = (int)(((double)x[k, m] / (double)255) * (double)65535);
                }
            }
            return z;
        }
    }
}
