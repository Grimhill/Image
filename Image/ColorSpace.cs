using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

//Obtain presented color space, and write them to file
namespace Image
{
    public static class ColorSpace
    {
        #region rgb2hsv

        //
        public static List<ArraysListDouble> rgb2hsv(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            hsvList = rgb2hsvCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return hsvList;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2hsv(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2hsv operation -> rgb2hsv(List<arraysListInt> rgbList) <-");
            }
            else
            {
                hsvList = rgb2hsvCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return hsvList;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2hsv(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2hsv operation -> rgb2hsv(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                hsvList = rgb2hsvCount(R, G, B);
            }

            return hsvList;
        }

        //H in degrees, S and V in divided by 100% values
        public static List<ArraysListDouble> rgb2hsvCount(int[,] R, int[,] G, int[,] B)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            const double HSVang = 60;

            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            //for count
            double[,] Hd = new double[height, width];   //Hue (цветовой тон)
            double[,] Sd = new double[height, width];   //Saturation (насыщеность)
            double[,] Vd = new double[height, width];   //Value (Brightness/яркость)

            //he R,G,B values are divided by 255 to change the range from 0..255 to 0..1:
            var Rcd = ArrOp.ImageUint8ToDouble(R);
            var Gcd = ArrOp.ImageUint8ToDouble(G);
            var Bcd = ArrOp.ImageUint8ToDouble(B);

            //count Hue in degrees values
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //Max of R G B
                    double Cmax = Math.Max(Rcd[i, j], Math.Max(Gcd[i, j], Bcd[i, j]));
                    //Min of R G B
                    double Cmin = Math.Min(Rcd[i, j], Math.Min(Gcd[i, j], Bcd[i, j]));

                    double delta = Cmax - Cmin;

                    if (delta == 0)
                    {
                        Hd[i, j] = 0;
                    }
                    else if (Rcd[i, j] == Cmax && Gcd[i, j] >= Bcd[i, j])
                    {
                        Hd[i, j] = HSVang * ((Gcd[i, j] - Bcd[i, j]) / delta);
                    }
                    else if (Rcd[i, j] == Cmax && Gcd[i, j] < Bcd[i, j])
                    {
                        Hd[i, j] = (HSVang * ((Gcd[i, j] - Bcd[i, j]) / delta)) + (double)360;
                    }
                    else if (Gcd[i, j] == Cmax)
                    {
                        Hd[i, j] = (HSVang * ((Bcd[i, j] - Rcd[i, j]) / delta)) + (double)120;
                    }
                    else if (Bcd[i, j] == Cmax)
                    {
                        Hd[i, j] = (HSVang * ((Rcd[i, j] - Gcd[i, j]) / delta)) + (double)240;
                    }
                }
            }

            //count Saturation in divide by 100% values
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //Max of R G B
                    double Cmax = Math.Max(Rcd[i, j], Math.Max(Gcd[i, j], Bcd[i, j]));
                    //Min of R G B
                    double Cmin = Math.Min(Rcd[i, j], Math.Min(Gcd[i, j], Bcd[i, j]));

                    if (Cmax == 0)
                    {
                        Sd[i, j] = 0;
                    }
                    else
                    {
                        Sd[i, j] = (double)1 - (Cmin / Cmax);
                    }
                }
            }

            //count Value \ Brightness in divide by 100% values
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //Max of R G B
                    double Cmax = Math.Max(Rcd[i, j], Math.Max(Gcd[i, j], Bcd[i, j]));

                    Vd[i, j] = Cmax;
                }
            }

            //H in degrees, S and V in divided by 100% values
            hsvList.Add(new ArraysListDouble() { Color = Hd });
            hsvList.Add(new ArraysListDouble() { Color = Sd });
            hsvList.Add(new ArraysListDouble() { Color = Vd });

            return hsvList;
        }

        #endregion rgb2hsv

        #region hsv2rgb

        //if image is 24bpp, with H\S\V convert in range [0...255] as rgb
        //H in degrees, S and V in divided by 100% values
        public static List<ArraysListInt> hsv2rgb(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            var ColorList = Helpers.GetPixels(img);

            var H = ArrOp.ArrayMultByConst(ArrOp.ImageUint8ToDouble(ColorList[0].Color), 360);
            var S = ArrOp.ImageUint8ToDouble(ColorList[1].Color);
            var V = ArrOp.ImageUint8ToDouble(ColorList[2].Color);

            List<ArraysListInt> rgbList = new List<ArraysListInt>();

            rgbList = hsv2rgbCount(H, S, V);

            return rgbList;
        }

        //H in degrees, S and V in divided by 100% values
        //List with H S V arrays in In the following order H-S-V
        public static List<ArraysListInt> hsv2rgb(List<ArraysListDouble> hsvList)
        {
            List<ArraysListInt> rgbList = new List<ArraysListInt>();

            if (hsvList[0].Color.Length != hsvList[1].Color.Length || hsvList[0].Color.Length != hsvList[2].Color.Length)
            {
                Console.WriteLine("H S V arrays size dismatch in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else if (hsvList[0].Color.Cast<double>().ToArray().Max() < 1 || hsvList[1].Color.Cast<double>().ToArray().Max() > 1 || hsvList[2].Color.Cast<double>().ToArray().Max() > 1)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1], look like /100% in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else if (hsvList[0].Color.Cast<double>().ToArray().Min() < 0 || hsvList[1].Color.Cast<double>().ToArray().Min() < 0 || hsvList[2].Color.Cast<double>().ToArray().Min() < 0)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1], look like /100%, and not negative in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else
            {
                rgbList = hsv2rgbCount(hsvList[0].Color, hsvList[1].Color, hsvList[2].Color);
            }

            return rgbList;
        }

        //H in degrees, S and V in divided by 100% values
        //H S V arrays in In the following order H-S-V
        public static List<ArraysListInt> hsv2rgb(double[,] H, double[,] S, double[,] V)
        {
            List<ArraysListInt> rgbList = new List<ArraysListInt>();

            if (H.Length != S.Length || H.Length != V.Length)
            {
                Console.WriteLine("H S V arrays size dismatch in hsv2rgb operation -> hsv2rgb(double[,] H, double[,] S, double[,] V) <-");
            }
            else if (H.Cast<double>().ToArray().Max() < 1 || S.Cast<double>().ToArray().Max() > 1 || V.Cast<double>().ToArray().Max() > 1)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1], look like /100% in hsv2rgb operation -> hsv2rgb(double[,] H, double[,] S, double[,] V) <-");
            }
            else if (H.Cast<double>().ToArray().Min() < 0 || S.Cast<double>().ToArray().Min() < 0 || V.Cast<double>().ToArray().Min() < 0)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1], look like /100%, and not negative in hsv2rgb operation -> hsv2rgb(double[,] H, double[,] S, double[,] V) <-");
            }
            else
            {
                rgbList = hsv2rgbCount(H, S, V);
            }

            return rgbList;
        }

        //H in degrees, S and V in divided by 100% values
        public static List<ArraysListInt> hsv2rgbCount(double[,] H, double[,] S, double[,] V)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = H.GetLength(1);
            int height = H.GetLength(0);

            const double HSVang = 60;

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            //back result [0 .. 255]
            int[,] R = new int[height, width];
            int[,] G = new int[height, width];
            int[,] B = new int[height, width];

            var C = ArrOp.ArrayMultElements(V, S);

            var X = ArrOp.AbsArrayElements(ArrOp.ArraySubWithConst(ArrOp.ModArrayElements(ArrOp.ArrayDivByConst(H, HSVang), 2), 1));
            X = ArrOp.ArrayMultElements(C, ArrOp.ConstSubArrayElements(1, X));

            var m = ArrOp.SubArrays(V, C);

            //R G B count
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (H[i, j] >= 0 & H[i, j] < 60)
                    {
                        R[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                        G[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)(m[i, j] * 255);
                    }
                    else if (H[i, j] >= 00 & H[i, j] < 120)
                    {
                        R[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        G[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)(m[i, j] * 255);
                    }
                    else if (H[i, j] >= 120 & H[i, j] < 180)
                    {
                        R[i, j] = (int)(m[i, j] * 255);
                        G[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                    }
                    else if (H[i, j] >= 180 & H[i, j] < 240)
                    {
                        R[i, j] = (int)(m[i, j] * 255);
                        G[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                    }
                    else if (H[i, j] >= 240 & H[i, j] < 300)
                    {
                        R[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        G[i, j] = (int)(m[i, j] * 255);
                        B[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                    }
                    else if (H[i, j] >= 300 & H[i, j] < 360)
                    {
                        R[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                        G[i, j] = (int)(m[i, j] * 255);
                        B[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                    }
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = R });
            rgbResult.Add(new ArraysListInt() { Color = G });
            rgbResult.Add(new ArraysListInt() { Color = B });

            return rgbResult;
        }


        #endregion hsv2rgb

        #region rgb2ntsc

        //
        public static List<ArraysListDouble> rgb2ntsc(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            ntscResult = rgb2ntscCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return ntscResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2ntsc(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2ntsc operation -> rgb2ntsc(List<arraysListInt> rgbList) <-");
            }
            else
            {
                ntscResult = rgb2ntscCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return ntscResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2ntsc(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2ntsc operation -> rgb2ntsc(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                ntscResult = rgb2ntscCount(R, G, B);
            }

            return ntscResult;
        }

        //Y I Q result - double values, not in range [0 1], include negative
        public static List<ArraysListDouble> rgb2ntscCount(int[,] R, int[,] G, int[,] B)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = R.GetLength(1);
            int height = R.GetLength(0);

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
                    double[] temp = new double[3] { R[i, j], G[i, j], B[i, j] };

                    Y[i, j] = ArrOp.MultVectors(Ycon, temp).Sum();
                    I[i, j] = ArrOp.MultVectors(Icon, temp).Sum();
                    Q[i, j] = ArrOp.MultVectors(Qcon, temp).Sum();
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
        public static List<ArraysListInt> ntsc2rgb(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            var ColorList = Helpers.GetPixels(img);

            double[,] Y = ArrOp.ArrayToDouble(ColorList[0].Color);
            double[,] I = ArrOp.ArrayToDouble(ColorList[1].Color);
            double[,] Q = ArrOp.ArrayToDouble(ColorList[2].Color);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            rgbResult = ntsc2rgbCount(Y, I, Q);

            return rgbResult;
        }

        //Y I Q in double values (as after convert rgb2ntsc, not in range [-1 1])
        //list Y I Q arrays in In the following order Y-I-Q
        public static List<ArraysListInt> ntsc2rgb(List<ArraysListDouble> ntscList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (ntscList[0].Color.Length != ntscList[1].Color.Length || ntscList[0].Color.Length != ntscList[2].Color.Length)
            {
                Console.WriteLine("Y I Q arrays size dismatch in ntsc2rgb operation -> ntsc2rgb(List<arraysListDouble> ntscList) <-");
            }
            else
            {
                rgbResult = ntsc2rgbCount(ntscList[0].Color, ntscList[1].Color, ntscList[2].Color);
            }

            return rgbResult;
        }

        //Y I Q in double values (as after convert rgb2ntsc, not in range [-1 1])
        //Y I Q arrays in In the following order Y-I-Q
        public static List<ArraysListInt> ntsc2rgb(double[,] Y, double[,] I, double[,] Q)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (Y.Length != I.Length || Y.Length != Q.Length)
            {
                Console.WriteLine("Y I Q arrays size dismatch in ntsc2rgb operation -> ntsc2rgb(double[,] Y, double[,] I, double[,] Q) <-");
            }
            else
            {
                rgbResult = ntsc2rgbCount(Y, I, Q);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> ntsc2rgbCount(double[,] Y, double[,] I, double[,] Q)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = Y.GetLength(1);
            int height = Y.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            double[,] R = new double[height, width];
            double[,] G = new double[height, width];
            double[,] B = new double[height, width];

            //converting coef
            double[] Ycon = new double[3] { 1, 0.956, 0.621 };
            double[] Icon = new double[3] { 1, -0.272, -0.647 };
            double[] Qcon = new double[3] { 1, -1.106, 1.703 };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { Y[i, j], I[i, j], Q[i, j] };

                    R[i, j] = ArrOp.MultVectors(Ycon, temp).Sum();
                    G[i, j] = ArrOp.MultVectors(Icon, temp).Sum();
                    B[i, j] = ArrOp.MultVectors(Qcon, temp).Sum();
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ArrayToUint8(R) });
            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ArrayToUint8(G) });
            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ArrayToUint8(B) });

            return rgbResult;
        }

        #endregion ntsc2rgb

        #region rgb2cmy

        public static List<ArraysListDouble> rgb2cmy(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();

            cmyResult = rgb2cmyCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return cmyResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2cmy(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2cmy operation -> rgb2cmy(List<arraysListInt> rgbList) <-");
            }
            else
            {
                cmyResult = rgb2cmyCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return cmyResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2cmy(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();
            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2cmy operation -> rgb2cmy(int[,] R, int[,] G, int[,]B) <-");
            }
            else
            {
                cmyResult = rgb2cmyCount(R, G, B);
            }

            return cmyResult;
        }

        //C M Y values - double in range [0:1]
        public static List<ArraysListDouble> rgb2cmyCount(int[,] R, int[,] G, int[,] B)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();

            double[,] C = new double[height, width];  //Cyan (голубой)
            double[,] M = new double[height, width];  //Magenta (пурпурный)
            double[,] Y = new double[height, width];  //Yellow

            C = ArrOp.ConstSubArrayElements(1, ArrOp.ImageUint8ToDouble(R)); //Cyan (голубой)
            M = ArrOp.ConstSubArrayElements(1, ArrOp.ImageUint8ToDouble(G)); //Magenta (пурпурный)
            Y = ArrOp.ConstSubArrayElements(1, ArrOp.ImageUint8ToDouble(B)); //Yellow

            cmyResult.Add(new ArraysListDouble() { Color = C });
            cmyResult.Add(new ArraysListDouble() { Color = M });
            cmyResult.Add(new ArraysListDouble() { Color = Y });

            return cmyResult;
        }

        #endregion rgb2cmy

        #region cmy2rgb

        //
        public static List<ArraysListInt> cmy2rgb(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            var ColorList = Helpers.GetPixels(img);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            double[,] C = ArrOp.ImageUint8ToDouble(ColorList[0].Color);
            double[,] M = ArrOp.ImageUint8ToDouble(ColorList[1].Color);
            double[,] Y = ArrOp.ImageUint8ToDouble(ColorList[2].Color);

            rgbResult = cmy2rgbCount(C, M, Y);

            return rgbResult;
        }

        //C M Y in double values (as after convert rgb2cmy, in range [0 1])
        //list C M Y arrays in In the following order C-M-Y
        public static List<ArraysListInt> cmy2rgb(List<ArraysListDouble> cmyList)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (cmyList[0].Color.Length != cmyList[1].Color.Length || cmyList[0].Color.Length != cmyList[2].Color.Length)
            {
                Console.WriteLine("C M Y arrays size dismatch in cmy2rgb operation -> cmy2rgb(List<arraysListDouble> cmyList) <-");
            }
            else if (cmyList[0].Color.Cast<double>().ToArray().Max() > 1)
            {
                //cmyList[0].c = ArrOp.ArrayDivByConst(cmyList[0].c, 255);
                //cmyList[1].c = ArrOp.ArrayDivByConst(cmyList[1].c, 255);
                //cmyList[2].c = ArrOp.ArrayDivByConst(cmyList[2].c, 255);
                Console.WriteLine("C M Y arrays Values must be in range [0 1], in cmy2rgb operation -> cmy2rgb(List<arraysListDouble> cmyList) <-");
            }
            else
            {
                rgbResult = cmy2rgbCount(cmyList[0].Color, cmyList[1].Color, cmyList[2].Color);
            }

            return rgbResult;
        }

        //C M Y in double values (as after convert rgb2cmy, in range [0 1])
        //C M Y arrays in In the following order C-M-Y
        public static List<ArraysListInt> cmy2rgb(double[,] C, double[,] M, double[,] Y)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (C.Length != M.Length || C.Length != Y.Length)
            {
                Console.WriteLine("C M Y arrays size dismatch in cmy2rgb operation -> cmy2rgb(double[,] C, double[,] M, double[,] Y) <-");
            }
            else if (C.Cast<double>().ToArray().Max() > 1)
            {
                //C = ArrOp.ArrayDivByConst(C, 255);
                //M = ArrOp.ArrayDivByConst(M, 255);
                //Y = ArrOp.ArrayDivByConst(Y, 255);
                Console.WriteLine("C M Y arrays Values must be in range [0 1], in cmy2rgb operation -> cmy2rgb(double[,] C, double[,] M, double[,] Y) <-");
            }
            else
            {
                rgbResult = cmy2rgbCount(C, M, Y);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> cmy2rgbCount(double[,] C, double[,] M, double[,] Y)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = C.GetLength(1);
            int height = C.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            int[,] R = new int[height, width];
            int[,] G = new int[height, width];
            int[,] B = new int[height, width];

            R = ArrOp.ImageArrayToUint8(ArrOp.ConstSubArrayElements(1, C));
            G = ArrOp.ImageArrayToUint8(ArrOp.ConstSubArrayElements(1, M));
            B = ArrOp.ImageArrayToUint8(ArrOp.ConstSubArrayElements(1, Y));

            rgbResult.Add(new ArraysListInt() { Color = R });
            rgbResult.Add(new ArraysListInt() { Color = G });
            rgbResult.Add(new ArraysListInt() { Color = B });

            return rgbResult;
        }

        #endregion cmy2rgb

        #region rgb2YCbCr

        public static List<ArraysListDouble> rgb2YCbCr(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            YCbCrResult = rgb2YCbCrCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return YCbCrResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2YCbCr(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2YCbCr operation -> rgb2YCbCr(List<arraysListInt> rgbList) <-");
            }
            else
            {
                YCbCrResult = rgb2YCbCrCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return YCbCrResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2YCbCr(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2YCbCr operation -> rgb2YCbCr(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                YCbCrResult = rgb2YCbCrCount(R, G, B);
            }

            return YCbCrResult;
        }

        //Y Cb Cr values - double, not in range [0 1]
        public static List<ArraysListDouble> rgb2YCbCrCount(int[,] R, int[,] G, int[,] B)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            var Rcd = ArrOp.ImageUint8ToDouble(R);
            var Gcd = ArrOp.ImageUint8ToDouble(G);
            var Bcd = ArrOp.ImageUint8ToDouble(B);

            double[,] Y = new double[height, width]; //luma (яркостная составляющая)
            double[,] Cb = new double[height, width]; //difference between B and Y ?
            double[,] Cr = new double[height, width]; //difference between R and Y ?

            double[] Ycon = new double[3] { 65.481, 128.553, 24.966 };
            double[] Cbcon = new double[3] { -37.797, -74.203, 112 };
            double[] Crcon = new double[3] { 112, -93.786, -18.214 };
            double[] Coef = new double[3] { 16, 128, 128 };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { Rcd[i, j], Gcd[i, j], Bcd[i, j] };

                    Y[i, j] = Coef[0] + ArrOp.MultVectors(Ycon, temp).Sum();
                    Cb[i, j] = Coef[1] + ArrOp.MultVectors(Cbcon, temp).Sum();
                    Cr[i, j] = Coef[2] + ArrOp.MultVectors(Crcon, temp).Sum();
                }
            }

            YCbCrResult.Add(new ArraysListDouble() { Color = Y });
            YCbCrResult.Add(new ArraysListDouble() { Color = Cb });
            YCbCrResult.Add(new ArraysListDouble() { Color = Cr });

            return YCbCrResult;
        }

        #endregion rgb2YCbCr

        #region YCbCr2rgb

        public static List<ArraysListInt> YCbCr2rgb(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            var ColorList = Helpers.GetPixels(img);
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            var Y = ArrOp.ArrayToDouble(ColorList[0].Color);
            var Cb = ArrOp.ArrayToDouble(ColorList[1].Color);
            var Cr = ArrOp.ArrayToDouble(ColorList[2].Color);

            rgbResult = YCbCr2rgbCount(Y, Cb, Cr);

            return rgbResult;
        }

        //Y Cb Cr in double values (as after convert rgb2YCbCr, not in range [-1 1])
        //list Y Cb Cr arrays in In the following order Y-Cb-Cr
        public static List<ArraysListInt> YCbCr2rgb(List<ArraysListDouble> YCbCrList)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (YCbCrList[0].Color.Length != YCbCrList[1].Color.Length || YCbCrList[0].Color.Length != YCbCrList[2].Color.Length)
            {
                Console.WriteLine("Y Cb Cr arrays size dismatch in YCbCr2rgb operation -> YCbCr2rgb(List<arraysListDouble> YCbCrList) <-");
            }
            else if (YCbCrList[0].Color.Cast<double>().ToArray().Max() < 1)
            {
                //YCbCrList[0].c = ArrOp.ArrayMultByConst(YCbCrList[0].c, 255);
                //YCbCrList[1].c = ArrOp.ArrayMultByConst(YCbCrList[1].c, 255);
                //YCbCrList[2].c = ArrOp.ArrayMultByConst(YCbCrList[2].c, 255);
                Console.WriteLine("Y Cb Cr arrays Values must be not in range [-1 1], in YCbCr2rgb operation -> YCbCr2rgb(List<arraysListDouble> YCbCrList) <-");
            }
            else
            {
                rgbResult = YCbCr2rgbCount(YCbCrList[0].Color, YCbCrList[1].Color, YCbCrList[2].Color);
            }

            return rgbResult;
        }

        //Y Cb Cr in double values (as after convert rgb2YCbCr, not in range [-1 1])
        //Y Cb Cr arrays in In the following order Y-Cb-Cr
        public static List<ArraysListInt> YCbCr2rgb(double[,] Y, double[,] Cb, double[,] Cr)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (Y.Length != Cb.Length || Y.Length != Cr.Length)
            {
                Console.WriteLine("Y Cb Cr arrays size dismatch in YCbCr2rgb operation -> YCbCr2rgb(double[,] Y, double[,] Cb, double[,] Cr) <-");
            }
            else if (Y.Cast<double>().ToArray().Max() < 1)
            {
                //Y  = ArrOp.ArrayMultByConst(Y, 255);
                //Cb = ArrOp.ArrayMultByConst(Cb, 255);
                //Cr = ArrOp.ArrayMultByConst(Cr, 255);
                Console.WriteLine("Y Cb Cr arrays Values must be not in range [-1 1], in YCbCr2rgb operation -> YCbCr2rgb(double[,] Y, double[,] Cb, double[,] Cr) <-");
            }
            else
            {
                rgbResult = YCbCr2rgbCount(Y, Cb, Cr);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> YCbCr2rgbCount(double[,] Y, double[,] Cb, double[,] Cr)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = Y.GetLength(1);
            int height = Y.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            Y = ArrOp.ArrayDivByConst(Y, 255);
            Cb = ArrOp.ArrayDivByConst(Cb, 255);
            Cr = ArrOp.ArrayDivByConst(Cr, 255);

            double[,] R = new double[height, width];
            double[,] G = new double[height, width];
            double[,] B = new double[height, width];

            double[] Ycon = new double[3] { 298.082, 0, 408.583 };
            double[] Cbcon = new double[3] { 298.082, -100.291, -208.12 };
            double[] Crcon = new double[3] { 298.082, 516.412, 0 };
            double[] Coef = new double[3] { -222.921, 135.576, -276.836 };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { Y[i, j], Cb[i, j], Cr[i, j] };

                    R[i, j] = Coef[0] + ArrOp.MultVectors(Ycon, temp).Sum();
                    G[i, j] = Coef[1] + ArrOp.MultVectors(Cbcon, temp).Sum();
                    B[i, j] = Coef[2] + ArrOp.MultVectors(Crcon, temp).Sum();
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ArrayToUint8(R) });
            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ArrayToUint8(G) });
            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ArrayToUint8(B) });

            return rgbResult;
        }

        #endregion YCbCr2rgb

        #region rgb2xyz

        //sRGB to XYZ D65/2
        public static List<ArraysListDouble> rgb2xyz(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            xyzResult = rgb2xyzCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return xyzResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2xyz(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2xyz operation -> rgb2xyz(List<arraysListInt> rgbList) <-");
            }
            else
            {
                xyzResult = rgb2xyzCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return xyzResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2xyz(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2xyz operation -> rgb2xyz(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                xyzResult = rgb2xyzCount(R, G, B);
            }

            return xyzResult;
        }

        //X Y Z values - double > 1, can be <1 if represent small R G B values
        public static List<ArraysListDouble> rgb2xyzCount(int[,] R, int[,] G, int[,] B)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            var Rcd = ArrOp.ImageUint8ToDouble(R);
            var Gcd = ArrOp.ImageUint8ToDouble(G);
            var Bcd = ArrOp.ImageUint8ToDouble(B);

            double[,] R_temp = new double[height, width];
            double[,] G_temp = new double[height, width];
            double[,] B_temp = new double[height, width];

            double[,] X = new double[height, width];
            double[,] Y = new double[height, width];
            double[,] Z = new double[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (Rcd[i, j] > 0.04045)
                    {
                        R_temp[i, j] = Math.Pow((Rcd[i, j] + 0.055) / 1.055, 2.4);
                    }
                    else
                    {
                        R_temp[i, j] = Rcd[i, j] / 12.92;
                    }

                    if (Gcd[i, j] > 0.04045)
                    {
                        G_temp[i, j] = Math.Pow((Gcd[i, j] + 0.055) / 1.055, 2.4);
                    }
                    else
                    {
                        G_temp[i, j] = Gcd[i, j] / 12.92;
                    }

                    if (Bcd[i, j] > 0.04045)
                    {
                        B_temp[i, j] = Math.Pow((Bcd[i, j] + 0.055) / 1.055, 2.4);
                    }
                    else
                    {
                        B_temp[i, j] = Bcd[i, j] / 12.92;
                    }

                    R_temp[i, j] = R_temp[i, j] * 100;
                    G_temp[i, j] = G_temp[i, j] * 100;
                    B_temp[i, j] = B_temp[i, j] * 100;

                    //rgb2xyz_D65
                    X[i, j] = R_temp[i, j] * 0.4124 + G_temp[i, j] * 0.3576 + B_temp[i, j] * 0.1805;
                    Y[i, j] = R_temp[i, j] * 0.2126 + G_temp[i, j] * 0.7152 + B_temp[i, j] * 0.0722;
                    Z[i, j] = R_temp[i, j] * 0.0193 + G_temp[i, j] * 0.1192 + B_temp[i, j] * 0.9505;
                }
            }

            xyzResult.Add(new ArraysListDouble() { Color = X });
            xyzResult.Add(new ArraysListDouble() { Color = Y });
            xyzResult.Add(new ArraysListDouble() { Color = Z });

            return xyzResult;
        }

        #endregion rgb2xyz

        #region xyz2rgb

        //XYZ D65/2 to sRGB
        public static List<ArraysListInt> xyz2rgb(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            var X = ArrOp.ArrayToDouble(ColorList[0].Color);
            var Y = ArrOp.ArrayToDouble(ColorList[1].Color);
            var Z = ArrOp.ArrayToDouble(ColorList[2].Color);

            rgbResult = xyz2rgbCount(X, Y, Z);

            return rgbResult;
        }

        //X Y Z in double values (as after convert rgb2XYZ, not in range [0 1], only if represent small R G B values)
        //list X Y Z arrays in In the following order X-Y-Z
        public static List<ArraysListInt> xyz2rgb(List<ArraysListDouble> xyzList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (xyzList[0].Color.Length != xyzList[1].Color.Length || xyzList[0].Color.Length != xyzList[2].Color.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2rgb operation -> xyz2rgb(List<arraysListDouble> xyzList) <-");
            }
            else
            {
                rgbResult = xyz2rgbCount(xyzList[0].Color, xyzList[1].Color, xyzList[2].Color);
            }

            return rgbResult;
        }

        //X Y Z in double values (as after convert rgb2XYZ, not in range [0 1], only if represent small R G B values)
        //X Y Z arrays in In the following order X-Y-Z
        public static List<ArraysListInt> xyz2rgb(double[,] X, double[,] Y, double[,] Z)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (X.Length != Y.Length || X.Length != Z.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2rgb operation -> xyz2rgb(double[,] X, double[,] Y, double[,] Z) <-");
            }
            else
            {
                rgbResult = xyz2rgbCount(X, Y, Z);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> xyz2rgbCount(double[,] X, double[,] Y, double[,] Z)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = X.GetLength(1);
            int height = X.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            double[,] R_temp = new double[height, width];
            double[,] G_temp = new double[height, width];
            double[,] B_temp = new double[height, width];

            double[,] R = new double[height, width];
            double[,] G = new double[height, width];
            double[,] B = new double[height, width];

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    X[i, j] = X[i, j] / 100;
                    Y[i, j] = Y[i, j] / 100;
                    Z[i, j] = Z[i, j] / 100;

                    R_temp[i, j] = X[i, j] * 3.2406 + Y[i, j] * (-1.5372) + Z[i, j] * (-0.4986);
                    G_temp[i, j] = X[i, j] * (-0.9689) + Y[i, j] * 1.8758 + Z[i, j] * 0.0415;
                    B_temp[i, j] = X[i, j] * 0.0557 + Y[i, j] * (-0.2040) + Z[i, j] * 1.057;

                    if (R_temp[i, j] > 0.0031308)
                    {
                        R[i, j] = 1.055 * (Math.Pow(R_temp[i, j], (1 / 2.4))) - 0.055;
                    }
                    else
                    {
                        R[i, j] = R_temp[i, j] * 12.92;
                    }

                    if (G_temp[i, j] > 0.0031308)
                    {
                        G[i, j] = 1.055 * (Math.Pow(G_temp[i, j], (1 / 2.4))) - 0.055;
                    }
                    else
                    {
                        G[i, j] = G_temp[i, j] * 12.92;
                    }

                    if (B_temp[i, j] > 0.0031308)
                    {
                        B[i, j] = 1.055 * (Math.Pow(B_temp[i, j], (1 / 2.4))) - 0.055;
                    }
                    else
                    {
                        B[i, j] = B_temp[i, j] * 12.92;
                    }
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ImageArrayToUint8(R) });
            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ImageArrayToUint8(G) });
            rgbResult.Add(new ArraysListInt() { Color = ArrOp.ImageArrayToUint8(B) });

            return rgbResult;
        }

        #endregion xyz2rgb

        #region xyz2lab

        //bad when from file, coz lost a lot after round values in all planes, when saved xyz result to file
        public static List<ArraysListDouble> xyz2lab(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            var X = ArrOp.ArrayToDouble(ColorList[0].Color);
            var Y = ArrOp.ArrayToDouble(ColorList[1].Color);
            var Z = ArrOp.ArrayToDouble(ColorList[2].Color);

            labResult = xyz2labCount(X, Y, Z);

            return labResult;
        }

        //List with X Y Z arrays in In the following order X Y Z
        public static List<ArraysListDouble> xyz2lab(List<ArraysListDouble> xyzList)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (xyzList[0].Color.Length != xyzList[1].Color.Length || xyzList[0].Color.Length != xyzList[2].Color.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2lab operation -> xyz2lab(List<arraysListInt> xyzList) <-");
            }
            else
            {
                labResult = xyz2labCount(xyzList[0].Color, xyzList[1].Color, xyzList[2].Color);
            }

            return labResult;
        }

        //X Y Z arrays in In the following order X Y Z
        public static List<ArraysListDouble> xyz2lab(double[,] X, double[,] Y, double[,] Z)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (X.Length != Y.Length || X.Length != Z.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2lab operation -> xyz2lab(double[,] X, double[,] Y, double[,] Z) <-");
            }
            else
            {
                labResult = xyz2labCount(X, Y, Z);
            }

            return labResult;
        }

        //L values - double, not in [0 1] range, a & b - same, but have negative values
        public static List<ArraysListDouble> xyz2labCount(double[,] X, double[,] Y, double[,] Z)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = X.GetLength(1);
            int height = X.GetLength(0);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            const double X_D65 = 95.047;
            const double Y_D65 = 100;
            const double Z_D65 = 108.883;

            X = ArrOp.ArrayDivByConst(X, X_D65);
            Y = ArrOp.ArrayDivByConst(Y, Y_D65);
            Z = ArrOp.ArrayDivByConst(Z, Z_D65);

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
                    if (X[i, j] > 0.008856)
                    {
                        X_temp[i, j] = Math.Pow(X[i, j], (1d / 3d));
                    }
                    else
                    {
                        X_temp[i, j] = (7.787 * X[i, j]) + (16d / 116d);
                    }

                    if (Y[i, j] > 0.008856)
                    {
                        Y_temp[i, j] = Math.Pow(Y[i, j], (1d / 3d));
                    }
                    else
                    {
                        Y_temp[i, j] = (7.787 * Y[i, j]) + (16d / 116d);
                    }

                    if (Z[i, j] > 0.008856)
                    {
                        Z_temp[i, j] = Math.Pow(Z[i, j], (1d / 3d));
                    }
                    else
                    {
                        Z_temp[i, j] = (7.787 * Z[i, j]) + (16d / 116d);
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
        public static List<ArraysListDouble> lab2xyz(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            double[,] L = ArrOp.ArrayToDouble(ColorList[0].Color);
            double[,] a = ArrOp.ArrayToDouble(ColorList[1].Color);
            double[,] b = ArrOp.ArrayToDouble(ColorList[2].Color);

            xyzResult = lab2xyzCount(L, a, b);

            return xyzResult;
        }

        //L a b in double values (as after convert rgb2XYZ, not in range [0 1])
        //list L a b arrays in In the following order L-a-b
        public static List<ArraysListDouble> lab2xyz(List<ArraysListDouble> labList)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (labList[0].Color.Length != labList[1].Color.Length || labList[0].Color.Length != labList[2].Color.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2xyz operation -> lab2xyz(List<arraysListDouble> labList) <-");
            }
            else
            {
                xyzResult = lab2xyzCount(labList[0].Color, labList[1].Color, labList[2].Color);
            }

            return xyzResult;
        }

        //L a b in double values (as after convert rgb2XYZ, not in range [0 1])
        //L a b arrays in In the following order L-a-b
        public static List<ArraysListDouble> lab2xyz(double[,] L, double[,] a, double[,] b)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (L.Length != a.Length || L.Length != b.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2xyz operation -> lab2xyz(double[,] L, double[,] a, double[,] b) <-");
            }
            else
            {
                xyzResult = lab2xyzCount(L, a, b);
            }

            return xyzResult;
        }

        //X Y Z values - double > 1, can be <1 if represent small R G B values
        public static List<ArraysListDouble> lab2xyzCount(double[,] L, double[,] a, double[,] b)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = L.GetLength(1);
            int height = L.GetLength(0);

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
                    Y_temp[i, j] = (L[i, j] + 16) / 116;
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

        #region rgb2lab

        public static List<ArraysListDouble> rgb2lab(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;

            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            var xyz = ColorSpace.rgb2xyz(ColorList);
            labResult = ColorSpace.xyz2lab(xyz);

            return labResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2lab(List<ArraysListInt> rgbList)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = rgbList[0].Color.GetLength(1);
            int height = rgbList[0].Color.GetLength(0);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("list R G B arrays size dismatch in rgb2lab operation -> rgb2lab(List<arraysListInt> rgbList) <-");
            }
            else
            {
                var xyz = ColorSpace.rgb2xyz(rgbList);
                labResult = ColorSpace.xyz2lab(xyz);
            }

            return labResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> rgb2lab(int[,] R, int[,] G, int[,] B)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            List<ArraysListInt> rgbList = new List<ArraysListInt>();

            rgbList.Add(new ArraysListInt() { Color = R });
            rgbList.Add(new ArraysListInt() { Color = G });
            rgbList.Add(new ArraysListInt() { Color = B });

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2lab operation -> rgb2lab(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                var xyz = ColorSpace.rgb2xyz(rgbList);
                labResult = ColorSpace.xyz2lab(xyz);
            }

            return labResult;
        }

        #endregion rgb2lab

        #region lab2rgb

        //bad, when from file. Lost a lot from converting and round
        public static List<ArraysListInt> lab2rgb(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;

            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> lablist = new List<ArraysListDouble>();

            lablist.Add(new ArraysListDouble() { Color = ArrOp.ArrayToDouble(ColorList[0].Color) });
            lablist.Add(new ArraysListDouble() { Color = ArrOp.ArrayToDouble(ColorList[1].Color) });
            lablist.Add(new ArraysListDouble() { Color = ArrOp.ArrayToDouble(ColorList[2].Color) });

            var labxyz = ColorSpace.lab2xyz(lablist);
            var xyzrgb = ColorSpace.xyz2rgb(labxyz);

            return xyzrgb;
        }

        //L a b in double values (as after convert XYZ2lab, not in range [0 1])
        //list L a b arrays in In the following order L-a-b
        public static List<ArraysListInt> lab2rgb(List<ArraysListDouble> labList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (labList[0].Color.Length != labList[1].Color.Length || labList[0].Color.Length != labList[2].Color.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2rgb operation -> lab2rgb(List<arraysListDouble> labList) <-");
            }
            else
            {
                var labxyz = ColorSpace.lab2xyz(labList);
                var xyzrgb = ColorSpace.xyz2rgb(labxyz);

                rgbResult = xyzrgb;
            }

            return rgbResult;
        }

        //L a b in double values (as after convert XYZ2lab, not in range [0 1])
        //L a b arrays in In the following order L-a-b
        public static List<ArraysListInt> lab2rgb(double[,] L, double[,] a, double[,] b)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            List<ArraysListDouble> labList = new List<ArraysListDouble>();

            labList.Add(new ArraysListDouble() { Color = L });
            labList.Add(new ArraysListDouble() { Color = a });
            labList.Add(new ArraysListDouble() { Color = b });

            if (L.Length != a.Length || L.Length != b.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2rgb operation -> lab2rgb(double[,] L, double[,] a, double[,] b) <-");
            }
            else
            {
                var labxyz = ColorSpace.lab2xyz(labList);
                var xyzrgb = ColorSpace.xyz2rgb(labxyz);

                rgbResult = xyzrgb;
            }

            return rgbResult;
        }

        #endregion lab2rgb

        public static double[,] FakeCIE1976L(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            var lab = ColorSpace.rgb2lab(img);

            double[,] L = lab[0].Color;
            L = ArrOp.ArrayMultByConst(L, 2.57);

            return L;
        }
    }

    public static class ColorSpaceToFile
    {
        //if image is 24bpp, with H\S\V convert in range [0...255] as rgb etc
        public static void colorSpaceToFile(int[,] R, int[,] G, int[,] B, ColorSpaceType colorSpace)
        {

        }
        public static void colorSpaceToFile(double[,] R, double[,] G, double[,] B, ColorSpaceType colorSpace)
        {

        }

        //all 2rgb looks good, if obtained not from file, but from rgb and made some filtering or another process, and saved as rgb back
        public static void AnothercolorSpacetoRGBXYZLabtoFile(List<ArraysListDouble> colorPlane, CPtoRGBaXYZLab colorSpace) //how generic arraysListT here?
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = colorPlane[0].Color.GetLength(1);
            int height = colorPlane[0].Color.GetLength(0);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne = new int[height, width];
            int[,] colorPlaneTwo = new int[height, width];
            int[,] colorPlaneThree = new int[height, width];

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            string outName = String.Empty;

            if (colorPlane[0].Color.Length != colorPlane[1].Color.Length || colorPlane[0].Color.Length != colorPlane[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> colorSpaceToFile(List<arraysListInt> Colors, ColorSpaceType colorSpace) <-");
            }
            else
            {
                switch (colorSpace.ToString())
                {
                    case "hsv2rgb":
                        rgbResult = ColorSpace.hsv2rgb(colorPlane);

                        colorPlaneOne = rgbResult[0].Color;
                        colorPlaneTwo = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\hsv2rgb.jpg";
                        break;

                    case "ntsc2rgb":
                        rgbResult = ColorSpace.ntsc2rgb(colorPlane);

                        colorPlaneOne = rgbResult[0].Color;
                        colorPlaneTwo = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //when ntsc2rgb from file
                        //approximate result in file, coz we lost negative values in I and Q when saving ntsc result in file [0..255]
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\ntsc2rgb.jpg";
                        break;

                    case "cmy2rgb":
                        rgbResult = ColorSpace.cmy2rgb(colorPlane);

                        colorPlaneOne = rgbResult[0].Color;
                        colorPlaneTwo = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\cmy2rgb.jpg";
                        break;

                    case "YCbCr2rgb":
                        rgbResult = ColorSpace.YCbCr2rgb(colorPlane);

                        colorPlaneOne = rgbResult[0].Color;
                        colorPlaneTwo = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\YCbCr2rgb.jpg";
                        break;

                    case "xyz2rgb":
                        rgbResult = ColorSpace.xyz2rgb(colorPlane);

                        colorPlaneOne = rgbResult[0].Color;
                        colorPlaneTwo = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //bad when from file, coz using heavy rounded X Y Z values, when writing them to file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2rgb.jpg";
                        break;

                    case "xyz2lab":
                        var xyzlabResult = ColorSpace.xyz2lab(colorPlane);

                        colorPlaneOne = ArrOp.ArrayToUint8(xyzlabResult[0].Color);
                        colorPlaneTwo = ArrOp.ArrayToUint8(xyzlabResult[1].Color);
                        colorPlaneThree = ArrOp.ArrayToUint8(xyzlabResult[2].Color);

                        //bad when from file, coz xyz values rounded, and lost negative value in a & b when saving in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2lab.jpg";
                        break;

                    case "lab2xyz":
                        var labxyzResult = ColorSpace.lab2xyz(colorPlane);

                        colorPlaneOne = ArrOp.ArrayToUint8(labxyzResult[0].Color);
                        colorPlaneTwo = ArrOp.ArrayToUint8(labxyzResult[1].Color);
                        colorPlaneThree = ArrOp.ArrayToUint8(labxyzResult[2].Color);

                        //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2xyz.jpg";
                        break;

                    case "lab2rgb":
                        rgbResult = ColorSpace.lab2rgb(colorPlane);

                        colorPlaneOne = rgbResult[0].Color;
                        colorPlaneTwo = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //if from file
                        //very bad, coz lost a lot in converting and round everywhere...
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2rgb.jpg";
                        break;

                    default:

                        colorPlaneOne = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneTwo = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneThree = Helpers.RandArray(height, width, 0, 255);

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\defaultNonColorSpace.jpg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            //dont forget, that directory ColorSpace must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //some rgb2 looks good, some lost negative values, when ranged to [0..255] for saving
        public static void RGBtoAnothercolorSpacetoFile(List<ArraysListInt> colorPlane, RGBtoCP colorSpace) //how generic arraysListT here?
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = colorPlane[0].Color.GetLength(1);
            int height = colorPlane[0].Color.GetLength(0);
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne = new int[height, width];
            int[,] colorPlaneTwo = new int[height, width];
            int[,] colorPlaneThree = new int[height, width];

            string outName = String.Empty;

            if (colorPlane[0].Color.Length != colorPlane[1].Color.Length || colorPlane[0].Color.Length != colorPlane[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> colorSpaceToFile(List<arraysListInt> Colors, ColorSpaceType colorSpace) <-");
            }
            else
            {
                switch (colorSpace.ToString())
                {
                    case "rgb2hsv":
                        var hsvResult = ColorSpace.rgb2hsv(colorPlane);

                        colorPlaneOne = ArrOp.ImageArrayToUint8(ArrOp.ArrayDivByConst(hsvResult[0].Color, 360));
                        colorPlaneTwo = ArrOp.ImageArrayToUint8(hsvResult[1].Color);
                        colorPlaneThree = ArrOp.ImageArrayToUint8(hsvResult[2].Color);

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2hsv.jpg";
                        break;

                    case "rgb2ntsc":
                        var ntscResult = ColorSpace.rgb2ntsc(colorPlane);

                        colorPlaneOne = ArrOp.ArrayToUint8(ntscResult[0].Color);
                        colorPlaneTwo = ArrOp.ArrayToUint8(ntscResult[1].Color);
                        colorPlaneThree = ArrOp.ArrayToUint8(ntscResult[2].Color);

                        //if we want to save rgb2ntsc result in file
                        //approximate result in file, coz we lost negative values in I and Q
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2ntsc.jpg";
                        break;

                    case "rgb2cmy":
                        var cmyResult = ColorSpace.rgb2cmy(colorPlane);

                        colorPlaneOne = ArrOp.ImageArrayToUint8(cmyResult[0].Color);
                        colorPlaneTwo = ArrOp.ImageArrayToUint8(cmyResult[1].Color);
                        colorPlaneThree = ArrOp.ImageArrayToUint8(cmyResult[2].Color);

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2cmy.jpg";
                        break;

                    case "rgb2YCbCr":
                        var YCbCrResult = ColorSpace.rgb2YCbCr(colorPlane);

                        colorPlaneOne = ArrOp.ArrayToUint8(YCbCrResult[0].Color);
                        colorPlaneTwo = ArrOp.ArrayToUint8(YCbCrResult[1].Color);
                        colorPlaneThree = ArrOp.ArrayToUint8(YCbCrResult[2].Color);

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2YCbCr.jpg";
                        break;

                    case "rgb2xyz":
                        var xyzrgbResult = ColorSpace.rgb2xyz(colorPlane);

                        colorPlaneOne = ArrOp.ArrayToUint8(xyzrgbResult[0].Color);
                        colorPlaneTwo = ArrOp.ArrayToUint8(xyzrgbResult[1].Color);
                        colorPlaneThree = ArrOp.ArrayToUint8(xyzrgbResult[2].Color);

                        //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2xyz.jpg";
                        break;

                    case "rgb2lab":
                        var rgblabResult = ColorSpace.rgb2lab(colorPlane);

                        colorPlaneOne = ArrOp.ArrayToUint8(rgblabResult[0].Color);
                        colorPlaneTwo = ArrOp.ArrayToUint8(rgblabResult[1].Color);
                        colorPlaneThree = ArrOp.ArrayToUint8(rgblabResult[2].Color);

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2lab.jpg";
                        break;

                    default:
                        colorPlaneOne = colorPlane[0].Color;
                        colorPlaneTwo = colorPlane[1].Color;
                        colorPlaneThree = colorPlane[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\defaultNonColorSpace.jpg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            //dont forget, that directory ColorSpace must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //if direct from file
        public static void colorSpaceToFile(Bitmap img, ColorSpaceType colorSpace)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne = new int[height, width];
            int[,] colorPlaneTwo = new int[height, width];
            int[,] colorPlaneThree = new int[height, width];

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            string outName = String.Empty;

            switch (colorSpace.ToString())
            {
                case "rgb2hsv":
                    var hsvResult = ColorSpace.rgb2hsv(img);

                    colorPlaneOne = ArrOp.ImageArrayToUint8(ArrOp.ArrayDivByConst(hsvResult[0].Color, 360));
                    colorPlaneTwo = ArrOp.ImageArrayToUint8(hsvResult[1].Color);
                    colorPlaneThree = ArrOp.ImageArrayToUint8(hsvResult[2].Color);

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2hsv.jpg";
                    break;

                case "hsv2rgb":
                    rgbResult = ColorSpace.hsv2rgb(img);

                    colorPlaneOne = rgbResult[0].Color;
                    colorPlaneTwo = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\hsv2rgb.jpg";
                    break;

                case "rgb2ntsc":
                    var ntscResult = ColorSpace.rgb2ntsc(img);

                    colorPlaneOne = ArrOp.ArrayToUint8(ntscResult[0].Color);
                    colorPlaneTwo = ArrOp.ArrayToUint8(ntscResult[1].Color);
                    colorPlaneThree = ArrOp.ArrayToUint8(ntscResult[2].Color);

                    //if we want to save rgb2ntsc result in file
                    //approximate result in file, coz we lost negative values in I and Q
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2ntsc.jpg";
                    break;

                case "ntsc2rgb":
                    rgbResult = ColorSpace.ntsc2rgb(img);

                    colorPlaneOne = rgbResult[0].Color;
                    colorPlaneTwo = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //when ntsc2rgb from file
                    //approximate result in file, coz we lost negative values in I and Q when saving ntsc result in file [0..255]
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\ntsc2rgb.jpg";
                    break;

                case "rgb2cmy":
                    var cmyResult = ColorSpace.rgb2cmy(img);

                    colorPlaneOne = ArrOp.ImageArrayToUint8(cmyResult[0].Color);
                    colorPlaneTwo = ArrOp.ImageArrayToUint8(cmyResult[1].Color);
                    colorPlaneThree = ArrOp.ImageArrayToUint8(cmyResult[2].Color);

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2cmy.jpg";
                    break;

                case "cmy2rgb":
                    rgbResult = ColorSpace.cmy2rgb(img);

                    colorPlaneOne = rgbResult[0].Color;
                    colorPlaneTwo = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\cmy2rgb.jpg";
                    break;

                case "rgb2YCbCr":
                    var YCbCrResult = ColorSpace.rgb2YCbCr(img);

                    colorPlaneOne = ArrOp.ArrayToUint8(YCbCrResult[0].Color);
                    colorPlaneTwo = ArrOp.ArrayToUint8(YCbCrResult[1].Color);
                    colorPlaneThree = ArrOp.ArrayToUint8(YCbCrResult[2].Color);

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2YCbCr.jpg";
                    break;

                case "YCbCr2rgb":
                    rgbResult = ColorSpace.YCbCr2rgb(img);

                    colorPlaneOne = rgbResult[0].Color;
                    colorPlaneTwo = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\YCbCr2rgb.jpg";
                    break;

                case "rgb2xyz":
                    var xyzrgbResult = ColorSpace.rgb2xyz(img);

                    colorPlaneOne = ArrOp.ArrayToUint8(xyzrgbResult[0].Color);
                    colorPlaneTwo = ArrOp.ArrayToUint8(xyzrgbResult[1].Color);
                    colorPlaneThree = ArrOp.ArrayToUint8(xyzrgbResult[2].Color);

                    //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2xyz.jpg";
                    break;

                case "xyz2rgb":
                    rgbResult = ColorSpace.xyz2rgb(img);

                    colorPlaneOne = rgbResult[0].Color;
                    colorPlaneTwo = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //bad when from file, coz using heavy rounded X Y Z values, when writing them to file
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2rgb.jpg";
                    break;

                case "xyz2lab":
                    var xyzlabResult = ColorSpace.xyz2lab(img);

                    colorPlaneOne = ArrOp.ArrayToUint8(xyzlabResult[0].Color);
                    colorPlaneTwo = ArrOp.ArrayToUint8(xyzlabResult[1].Color);
                    colorPlaneThree = ArrOp.ArrayToUint8(xyzlabResult[2].Color);

                    //bad when from file, coz xyz values rounded, and lost negative value in a & b when saving in [0..255] range into file
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2lab.jpg";
                    break;

                case "lab2xyz":
                    var labxyzResult = ColorSpace.lab2xyz(img);

                    colorPlaneOne = ArrOp.ArrayToUint8(labxyzResult[0].Color);
                    colorPlaneTwo = ArrOp.ArrayToUint8(labxyzResult[1].Color);
                    colorPlaneThree = ArrOp.ArrayToUint8(labxyzResult[2].Color);

                    //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2xyz.jpg";
                    break;

                case "rgb2lab":
                    var rgblabResult = ColorSpace.rgb2lab(img);

                    colorPlaneOne = ArrOp.ArrayToUint8(rgblabResult[0].Color);
                    colorPlaneTwo = ArrOp.ArrayToUint8(rgblabResult[1].Color);
                    colorPlaneThree = ArrOp.ArrayToUint8(rgblabResult[2].Color);

                    //bad, coz lost negative value in a & b when saving in [0..255] range into file
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2lab.jpg";
                    break;

                case "lab2rgb":
                    rgbResult = ColorSpace.lab2rgb(img);

                    colorPlaneOne = rgbResult[0].Color;
                    colorPlaneTwo = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //very bad, coz lost a lot in converting and round everywhere...
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2rgb.jpg";
                    break;

                default:
                    colorPlaneOne = Helpers.GetPixels(img)[0].Color;
                    colorPlaneTwo = Helpers.GetPixels(img)[1].Color;
                    colorPlaneThree = Helpers.GetPixels(img)[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\defaultNonColorSpace.jpg";
                    break;
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            //dont forget, that directory ColorSpace must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static void FakeCIE1976LtoFile(Bitmap img)
        {
            var fake = ColorSpace.FakeCIE1976L(img);

            string Name = "FakeCIE1976L";

            Helpers.WriteImageToFile(fake, fake, fake, Name);
        }
    }

    public enum ColorSpaceType
    {
        rgb2hsv,
        hsv2rgb,
        rgb2ntsc,
        ntsc2rgb,
        rgb2cmy,
        cmy2rgb,
        rgb2YCbCr,
        YCbCr2rgb,
        rgb2xyz,
        xyz2rgb,
        xyz2lab,
        lab2xyz,
        rgb2lab,
        lab2rgb
    }

    //RGB to another color plane
    public enum RGBtoCP
    {
        rgb2hsv,
        rgb2ntsc,
        rgb2cmy,
        rgb2YCbCr,
        rgb2xyz,
        rgb2lab
    }

    //
    public enum CPtoRGBaXYZLab
    {
        hsv2rgb,
        ntsc2rgb,
        cmy2rgb,
        YCbCr2rgb,
        xyz2rgb,
        xyz2lab,
        lab2xyz,
        lab2rgb
    }
    
}
