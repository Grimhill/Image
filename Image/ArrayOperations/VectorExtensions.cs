using System;

namespace Image.ArrayOperations
{
    public static class VectorExtensions
    {
        //Mult vector elements by const
        public static double[] VectorMultByConst(this double[] x, double consts)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                z[k] = x[k] * consts;
            }
            return z;
        }

        //Div vector elements by const
        public static double[] VectorDivByConst(this double[] x, double consts)
        {
            double[] z = new double[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                if (consts == 0)
                {
                    Console.WriteLine("Cant div by 0");
                }
                else
                {
                    z[k] = x[k] / consts;
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
