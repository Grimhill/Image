using System;
using System.Linq;

namespace Image.ArrayOperations
{
    public static class VectorExtensions
    {
        //Mult vector elements by const
        public static double[] VectorMultByConst(this double[] x, double consts)
        {
            return x.Select(r => r * consts).ToArray();
        }

        //Div vector elements by const
        public static double[] VectorDivByConst(this double[] x, double consts)
        {
            double[] z = new double[x.Length];
            if (consts == 0)
                throw new System.DivideByZeroException("Constant can`t be zero");
            //Console.WriteLine("Cant div by 0"),            
            else
                z = x.Select(r => r / consts).ToArray();

            return z;
        }

        //Sum vector elements with const
        public static double[] VectorSumConst(this double[] x, double consts)
        {
            return x.Select(r => r + consts).ToArray();
        }

        //Sub vector elements with const
        public static double[] VectorSubConst(this double[] x, double consts)
        {
            return x.Select(r => r - consts).ToArray();
        }

        //Const Sub vector elements
        public static double[] ConstSubVectorElements(this double[] x, double consts)
        {
            return x.Select(r => consts - r).ToArray();
        }

        //Pow Vector elements
        public static double[] PowVectorElements(this double[] x, double pow)
        {
            return x.Select(r => Math.Pow(r, pow)).ToArray();
        }

        //Mult 2 vectors [i] elements
        public static double[] MultVectors(this double[] x, double[] y)
        {
            return x.Zip(y, (a, b) => a * b).ToArray();
        }

        //Sum 2 vectors [i] elements
        public static double[] SumVectors(this double[] x, double[] y)
        {
            return x.Zip(y, (a, b) => a + b).ToArray();
        }

        //Sub 2 vectors [i] elements
        public static double[] SubVectors(this double[] x, double[] y)
        {
            return x.Zip(y, (a, b) => a - b).ToArray();
        }

        //Convert type from int to double
        public static double[] VectorToDouble(this int[] x)
        {
            return x.Select(r => (double)r).ToArray();
        }

        //Convert type from double to int
        public static int[] VectorToInt(this double[] x)
        {
            return x.Select(r => Convert.ToInt32(r)).ToArray();
        }

        //Convert double values to uint8 range
        public static int[] ImageVectorToUint8(this double[] x)
        {
            int[] z = new int[x.Length];

            for (int k = 0; k < x.Length; k++)
            {
                if ((int)(x[k] * 255) > 255) { z[k] = 255; }
                else if (x[k] < 0) { z[k] = 0; }
                else { z[k] = (int)(x[k] * 255); }
            }
            return z;
        }
    }
}
