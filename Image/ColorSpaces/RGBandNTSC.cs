using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image.ColorSpaces
{
    public static class RGBandNTSC
    {
        #region rgb2ntsc

        //
        public static List<ArraysListDouble> RGB2NTSC(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            ntscResult = RGB2NTSCCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return ntscResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2NTSC(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2ntsc operation -> rgb2ntsc(List<arraysListInt> rgbList) <-");
            }
            else
            {
                ntscResult = RGB2NTSCCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return ntscResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2NTSC(int[,] r, int[,] g, int[,] b)
        {
            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            if (r.Length != g.Length || r.Length != b.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2ntsc operation -> rgb2ntsc(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                ntscResult = RGB2NTSCCount(r, g, b);
            }

            return ntscResult;
        }

        //Y I Q result - double values, not in range [0 1]; include negative
        private static List<ArraysListDouble> RGB2NTSCCount(int[,] r, int[,] g, int[,] b)
        {
            int width  = r.GetLength(1);
            int height = r.GetLength(0);

            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            double[,] Y = new double[height, width]; //luma (яркостная составляющая)
            double[,] I = new double[height, width]; //Chroma (color difference)
            double[,] Q = new double[height, width]; //Chroma (color difference)

            //converting coef
            double[] Ycon = new double[3] { 0.299, 0.587, 0.114 };
            double[] Icon = new double[3] { 0.596, -0.274, -0.322 };
            double[] Qcon = new double[3] { 0.211, -0.523, 0.312 };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { r[i, j], g[i, j], b[i, j] };

                    Y[i, j] = Ycon.MultVectors(temp).Sum();
                    I[i, j] = Icon.MultVectors(temp).Sum();
                    Q[i, j] = Qcon.MultVectors(temp).Sum();
                }
            }

            ntscResult.Add(new ArraysListDouble() { Color = Y });
            ntscResult.Add(new ArraysListDouble() { Color = I });
            ntscResult.Add(new ArraysListDouble() { Color = Q });

            return ntscResult;
        }

        #endregion rgb2ntsc

        #region ntsc2rgb

        //bad when from file, coz lost negative values in I & Q when saving ntsc result in file
        public static List<ArraysListInt> NTSC2RGB(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);

            double[,] Y = (ColorList[0].Color).ArrayToDouble();
            double[,] I = (ColorList[1].Color).ArrayToDouble();
            double[,] Q = (ColorList[2].Color).ArrayToDouble();

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            rgbResult = NTSC2RGBCount(Y, I, Q);

            return rgbResult;
        }

        //Y I Q in double values (as after convert rgb2ntsc; not in range [-1 1])
        //list Y I Q arrays in In the following order Y-I-Q
        public static List<ArraysListInt> NTSC2RGB(List<ArraysListDouble> ntscList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (ntscList[0].Color.Length != ntscList[1].Color.Length || ntscList[0].Color.Length != ntscList[2].Color.Length)
            {
                Console.WriteLine("Y I Q arrays size dismatch in ntsc2rgb operation -> ntsc2rgb(List<arraysListDouble> ntscList) <-");
            }
            else
            {
                rgbResult = NTSC2RGBCount(ntscList[0].Color, ntscList[1].Color, ntscList[2].Color);
            }

            return rgbResult;
        }

        //Y I Q in double values (as after convert rgb2ntsc, not in range [-1 1])
        //Y I Q arrays in In the following order Y-I-Q
        public static List<ArraysListInt> NTSC2RGB(double[,] y, double[,] i, double[,] q)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (y.Length != i.Length || y.Length != q.Length)
            {
                Console.WriteLine("Y I Q arrays size dismatch in ntsc2rgb operation -> ntsc2rgb(double[,] Y, double[,] I, double[,] Q) <-");
            }
            else
            {
                rgbResult = NTSC2RGBCount(y, i, q);
            }

            return rgbResult;
        }

        private static List<ArraysListInt> NTSC2RGBCount(double[,] y, double[,] i, double[,] q)
        {
            int width  = y.GetLength(1);
            int height = y.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            double[,] R = new double[height, width];
            double[,] G = new double[height, width];
            double[,] B = new double[height, width];

            //converting coef
            double[] Ycon = new double[3] { 1, 0.956, 0.621 };
            double[] Icon = new double[3] { 1, -0.272, -0.647 };
            double[] Qcon = new double[3] { 1, -1.106, 1.703 };

            for (int k = 0; k < height; k++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { y[k, j], i[k, j], q[k, j] };

                    R[k, j] = Ycon.MultVectors(temp).Sum();
                    G[k, j] = Icon.MultVectors(temp).Sum();
                    B[k, j] = Qcon.MultVectors(temp).Sum();
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = R.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = G.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = B.ArrayToUint8() });

            return rgbResult;
        }

        #endregion ntsc2rgb
    }
}
