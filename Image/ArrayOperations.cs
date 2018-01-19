using System;

//Some array helping array operations
namespace Image
{
    public class ArrayOperations
    {

    }

    public class ArrGen<T>
    {
        //arrGen<int> d;
        //d = new arrGen<int>();
        //var p = d.arrOfSingle(4, 5, 1);
        public T[,] ArrOfSingle(int r, int c, T value)
        {
            T[,] arr = new T[r, c];
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    arr[i, j] = value;
                }
            }

            return arr;
        }

        public T[,] VecorToArrayRowByRow(int r, int c, T[] inVector)
        {
            T[,] arr = new T[r, c];

            int count = 0;
            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    arr[i, j] = inVector[count];
                    count++;
                }
            }

            return arr;
        }

        public T[,] VecorToArrayColbyCol(int r, int c, T[] inVector)
        {
            T[,] arr = new T[r, c];

            int count = 0;
            for (int i = 0; i < c; i++)
            {
                for (int j = 0; j < r; j++)
                {
                    arr[j, i] = inVector[count];
                    count++;
                }
            }

            return arr;
        }

        public T[,] TransposeArray(T[,] inArr)
        {
            //for transpose!
            int rows = inArr.GetLength(1);
            int cols = inArr.GetLength(0);

            T[,] arr = new T[rows, cols];

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    arr[j, i] = inArr[i, j];
                }
            }

            return arr;
        }

        public T[] ArrayToVectorColByCol(T[,] inArr)
        {
            //for transpose!
            int rows = inArr.GetLength(0);
            int cols = inArr.GetLength(1);

            T[] arr = new T[inArr.Length];
            int count = 0;

            for (int i = 0; i < cols; i++)
            {
                for (int j = 0; j < rows; j++)
                {
                    arr[count] = inArr[j, i];
                    count++;
                }
            }

            return arr;
        }
    }

    public static class ArrayDoubleExtenshions
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
                    z[k, m] = x[k, m] + (int)conts;
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

        //obtain module of dive array elements by const
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

    public static class ArrayIntExtenshions
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

    public static class ArrayVectorExtenshions
    {
        //Mult vector elements by const
        public static double[] VectorMultByConst(this double[] x, double conts)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = x[k] * conts;
            }
            return z;
        }

        //Div vector elements by const
        public static double[] VectorDivByConst(this double[] x, double conts)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                if (conts == 0)
                {
                    Console.WriteLine("Cant div by 0");
                }
                else
                {
                    z[k] = x[k] / conts;
                }
            }
            return z;
        }

        //Sum vector elements with const
        public static double[] VectorSumConst(this double[] x, double consts)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = x[k] + consts;
            }
            return z;
        }

        //Sub vector elements with const
        public static double[] VectorSubConst(this double[] x, double consts)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = x[k] - consts;
            }
            return z;
        }

        //Const Sub vector elements
        public static double[] ConstSubVectorElements(this double[] x, double consts)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = consts - x[k];
            }
            return z;
        }

        //Pow Vector elements
        public static double[] PowVectorElements(this double[] x, double pow)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = Math.Pow(x[k], pow);
            }
            return z;
        }

        //Mult 2 vectors [i] elements
        public static double[] MultVectors(this double[] x, double[] y)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = x[k] * y[k];
            }
            return z;
        }

        //Sum 2 vectors [i] elements
        public static double[] SumVectors(this double[] x, double[] y)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = x[k] + y[k];
            }
            return z;
        }

        //Area
        public static double[] VectorToDouble(this int[] x)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = x[k];
            }
            return z;
        }

        public static int[] ImageVectorToUint8(this double[] x)
        {
            int[] z = new int[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                if ((int)(x[k] * 255) > 255) { z[k] = 255; } else { z[k] = (int)(x[k] * 255); }
            }
            return z;
        }
    }
}
