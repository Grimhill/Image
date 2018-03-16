using System.Linq;
using Image.ArrayOperations;

namespace Image
{
    public static class Convolution
    {
        //Convolution of 2 vectors
        public static double[] Conv(double[] u, double[] v, Convback convback)
        {
            return ConvProcess(u, v, convback);
        }

        public static int[] Conv(int[] u, int[] v, Convback convback)
        {
            return ConvProcess(u.VectorToDouble(), v.VectorToDouble(), convback).VectorToInt();
        }

        public static double[] Conv(int[] u, double[] v, Convback convback)
        {
            return ConvProcess(u.VectorToDouble(), v, convback);
        }

        public static double[] Conv(double[] u, int[] v, Convback convback)
        {
            return ConvProcess(u, v.VectorToDouble(), convback);
        }

        private static double[] ConvProcess(double[] u, double[] v, Convback convback)
        {
            double[] result = new double[u.Length + v.Length - 1];
            double[] temp   = new double[result.Length];

            int r = 0;
            for (int i = 0; i < u.Length; i++)
            {
                r = i;
                temp = new double[result.Length];
                for (int j = 0; j < v.Length; j++)
                {
                    temp[r] = u[i] * v[j];
                    r++;
                }
                result = result.SumVectors(temp);
            }

            if (convback == Convback.same)
            {
                var resList = result.ToList();
                int index = 0;
                if (v.Length % 2 != 0)
                {
                    index = (v.Length - 1) / 2;
                }
                else
                {
                    index = v.Length / 2;
                }

                resList = resList.GetRange(index, u.Length);

                result = new double[resList.Count];
                result = resList.ToArray();
            }

            return result;
        }        
    }
}
