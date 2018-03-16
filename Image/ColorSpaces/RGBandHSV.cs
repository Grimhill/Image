using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image.ColorSpaces
{
    public static class RGBandHSV
    {
        #region rgb2hsv

        //
        public static List<ArraysListDouble> RGB2HSV(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            hsvList = RGB2HSVCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return hsvList;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2HSV(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2hsv operation -> rgb2hsv(List<arraysListInt> rgbList) <-");
            }
            else
            {
                hsvList = RGB2HSVCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return hsvList;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2HSV(int[,] r, int[,] g, int[,] b)
        {
            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            if (r.Length != g.Length || r.Length != b.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2hsv operation -> rgb2hsv(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                hsvList = RGB2HSVCount(r, g, b);
            }

            return hsvList;
        }

        //H in degrees, S and V in divided by 100% values
        private static List<ArraysListDouble> RGB2HSVCount(int[,] r, int[,] g, int[,] b)
        {
            int width  = r.GetLength(1);
            int height = r.GetLength(0);

            const double HSVang = 60;
            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            //for count
            double[,] Hd = new double[height, width];   //Hue (цветовой тон)
            double[,] Sd = new double[height, width];   //Saturation (насыщеность)
            double[,] Vd = new double[height, width];   //Value (Brightness/яркость)

            //here R,G,B values are divided by 255 to change the range from 0..255 to 0..1:
            var Rcd = r.ImageUint8ToDouble();
            var Gcd = g.ImageUint8ToDouble();
            var Bcd = b.ImageUint8ToDouble();

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

        //if image is 24bpp; with H\S\V convert in range [0...255] as rgb
        //H in degrees, S and V in divided by 100% values
        public static List<ArraysListInt> HSV2RGB(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);

            var H = (ColorList[0].Color).ImageUint8ToDouble().ArrayMultByConst(360);
            var S = (ColorList[1].Color).ImageUint8ToDouble();
            var V = (ColorList[2].Color).ImageUint8ToDouble();

            List<ArraysListInt> rgbList = new List<ArraysListInt>();
            rgbList = HSV2RGBCount(H, S, V);

            return rgbList;
        }

        //H in degrees; S and V in divided by 100% values
        //List with H S V arrays in In the following order H-S-V
        public static List<ArraysListInt> HSV2RGB(List<ArraysListDouble> hsvList)
        {
            List<ArraysListInt> rgbList = new List<ArraysListInt>();

            if (hsvList[0].Color.Length != hsvList[1].Color.Length || hsvList[0].Color.Length != hsvList[2].Color.Length)
            {
                Console.WriteLine("H S V arrays size dismatch in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else if (hsvList[0].Color.Cast<double>().ToArray().Max() < 1 || hsvList[1].Color.Cast<double>().ToArray().Max() > 1 || hsvList[2].Color.Cast<double>().ToArray().Max() > 1)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees); S & V values in range [0..1]; look like /100% in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else if (hsvList[0].Color.Cast<double>().ToArray().Min() < 0 || hsvList[1].Color.Cast<double>().ToArray().Min() < 0 || hsvList[2].Color.Cast<double>().ToArray().Min() < 0)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees); S & V values in range [0..1]; look like /100%; and not negative in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else
            {
                rgbList = HSV2RGBCount(hsvList[0].Color, hsvList[1].Color, hsvList[2].Color);
            }

            return rgbList;
        }

        //H in degrees; S and V in divided by 100% values
        //H S V arrays in In the following order H-S-V
        public static List<ArraysListInt> HSV2RGB(double[,] h, double[,] s, double[,] v)
        {
            List<ArraysListInt> rgbList = new List<ArraysListInt>();

            if (h.Length != s.Length || h.Length != v.Length)
            {
                Console.WriteLine("H S V arrays size dismatch in hsv2rgb operation -> hsv2rgb(double[,] H, double[,] S, double[,] V) <-");
            }
            else if (h.Cast<double>().ToArray().Max() < 1 || s.Cast<double>().ToArray().Max() > 1 || v.Cast<double>().ToArray().Max() > 1)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1]; look like /100% in hsv2rgb operation -> hsv2rgb(double[,] H, double[,] S, double[,] V) <-");
            }
            else if (h.Cast<double>().ToArray().Min() < 0 || s.Cast<double>().ToArray().Min() < 0 || v.Cast<double>().ToArray().Min() < 0)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1]; look like /100%; and not negative in hsv2rgb operation -> hsv2rgb(double[,] H, double[,] S, double[,] V) <-");
            }
            else
            {
                rgbList = HSV2RGBCount(h, s, v);
            }

            return rgbList;
        }

        //H in degrees; S and V in divided by 100% values
        private static List<ArraysListInt> HSV2RGBCount(double[,] h, double[,] s, double[,] v)
        {
            int width  = h.GetLength(1);
            int height = h.GetLength(0);

            const double HSVang = 60;

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            //back result [0 .. 255]
            int[,] R = new int[height, width];
            int[,] G = new int[height, width];
            int[,] B = new int[height, width];

            var C = v.ArrayMultElements(s);

            var X = h.ArrayDivByConst(HSVang).ModArrayElements(2).ArraySubWithConst(1).AbsArrayElements();
            X = X.ConstSubArrayElements(1).ArrayMultElements(C);

            var m = v.SubArrays(C);

            //R G B count
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (h[i, j] >= 0 & h[i, j] < 60)
                    {
                        R[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                        G[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)(m[i, j] * 255);
                    }
                    else if (h[i, j] >= 00 & h[i, j] < 120)
                    {
                        R[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        G[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)(m[i, j] * 255);
                    }
                    else if (h[i, j] >= 120 & h[i, j] < 180)
                    {
                        R[i, j] = (int)(m[i, j] * 255);
                        G[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                    }
                    else if (h[i, j] >= 180 & h[i, j] < 240)
                    {
                        R[i, j] = (int)(m[i, j] * 255);
                        G[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        B[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                    }
                    else if (h[i, j] >= 240 & h[i, j] < 300)
                    {
                        R[i, j] = (int)((X[i, j] + m[i, j]) * 255);
                        G[i, j] = (int)(m[i, j] * 255);
                        B[i, j] = (int)((C[i, j] + m[i, j]) * 255);
                    }
                    else if (h[i, j] >= 300 & h[i, j] < 360)
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
    }
}
