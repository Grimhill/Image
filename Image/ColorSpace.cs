using System;
using System.Collections.Generic;
using System.Linq;
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
        public static List<ArraysListDouble> RGB2HSV(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

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
        public static List<ArraysListDouble> RGB2HSV(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2hsv operation -> rgb2hsv(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                hsvList = RGB2HSVCount(R, G, B);
            }

            return hsvList;
        }

        //H in degrees, S and V in divided by 100% values
        public static List<ArraysListDouble> RGB2HSVCount(int[,] R, int[,] G, int[,] B)
        {     
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            const double HSVang = 60;

            List<ArraysListDouble> hsvList = new List<ArraysListDouble>();

            //for count
            double[,] Hd = new double[height, width];   //Hue (цветовой тон)
            double[,] Sd = new double[height, width];   //Saturation (насыщеность)
            double[,] Vd = new double[height, width];   //Value (Brightness/яркость)

            //he R,G,B values are divided by 255 to change the range from 0..255 to 0..1:
            var Rcd = R.ImageUint8ToDouble();
            var Gcd = G.ImageUint8ToDouble();
            var Bcd = B.ImageUint8ToDouble();

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
        public static List<ArraysListInt> HSV2RGB(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            var H = (ColorList[0].Color).ImageUint8ToDouble().ArrayMultByConst(360); 
            var S = (ColorList[1].Color).ImageUint8ToDouble(); 
            var V = (ColorList[2].Color).ImageUint8ToDouble(); 

            List<ArraysListInt> rgbList = new List<ArraysListInt>();

            rgbList = HSV2RGBCount(H, S, V);

            return rgbList;
        }

        //H in degrees, S and V in divided by 100% values
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
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1], look like /100% in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else if (hsvList[0].Color.Cast<double>().ToArray().Min() < 0 || hsvList[1].Color.Cast<double>().ToArray().Min() < 0 || hsvList[2].Color.Cast<double>().ToArray().Min() < 0)
            {
                Console.WriteLine("Values of H array must be in 0..360 range (degrees), S & V values in range [0..1], look like /100%, and not negative in hsv2rgb operation -> hsv2rgb(List<arraysListDouble> hsvList) <-");
            }
            else
            {
                rgbList = HSV2RGBCount(hsvList[0].Color, hsvList[1].Color, hsvList[2].Color);
            }

            return rgbList;
        }

        //H in degrees, S and V in divided by 100% values
        //H S V arrays in In the following order H-S-V
        public static List<ArraysListInt> HSV2RGB(double[,] H, double[,] S, double[,] V)
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
                rgbList = HSV2RGBCount(H, S, V);
            }

            return rgbList;
        }

        //H in degrees, S and V in divided by 100% values
        public static List<ArraysListInt> HSV2RGBCount(double[,] H, double[,] S, double[,] V)
        { 
            int width = H.GetLength(1);
            int height = H.GetLength(0);

            const double HSVang = 60;

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            //back result [0 .. 255]
            int[,] R = new int[height, width];
            int[,] G = new int[height, width];
            int[,] B = new int[height, width];

            var C = V.ArrayMultElements(S); 
            
            var X = H.ArrayDivByConst(HSVang).ModArrayElements(2).ArraySubWithConst(1).AbsArrayElements();          
            X = X.ConstSubArrayElements(1).ArrayMultElements(C);

            var m = V.SubArrays(C);

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
        public static List<ArraysListDouble> RGB2NTSC(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

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
        public static List<ArraysListDouble> RGB2NTSC(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> ntscResult = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2ntsc operation -> rgb2ntsc(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                ntscResult = RGB2NTSCCount(R, G, B);
            }

            return ntscResult;
        }

        //Y I Q result - double values, not in range [0 1], include negative
        public static List<ArraysListDouble> RGB2NTSCCount(int[,] R, int[,] G, int[,] B)
        {         
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
            var ColorList = Helpers.GetPixels(img);

            double[,] Y = (ColorList[0].Color).ArrayToDouble(); 
            double[,] I = (ColorList[1].Color).ArrayToDouble(); 
            double[,] Q = (ColorList[2].Color).ArrayToDouble(); 

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            rgbResult = NTSC2RGBCount(Y, I, Q);

            return rgbResult;
        }

        //Y I Q in double values (as after convert rgb2ntsc, not in range [-1 1])
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
        public static List<ArraysListInt> NTSC2RGB(double[,] Y, double[,] I, double[,] Q)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (Y.Length != I.Length || Y.Length != Q.Length)
            {
                Console.WriteLine("Y I Q arrays size dismatch in ntsc2rgb operation -> ntsc2rgb(double[,] Y, double[,] I, double[,] Q) <-");
            }
            else
            {
                rgbResult = NTSC2RGBCount(Y, I, Q);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> NTSC2RGBCount(double[,] Y, double[,] I, double[,] Q)
        {      
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

                    R[i, j] = Ycon.MultVectors(temp).Sum(); 
                    G[i, j] = Icon.MultVectors(temp).Sum(); 
                    B[i, j] = Qcon.MultVectors(temp).Sum(); 
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = R.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = G.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = B.ArrayToUint8() });

            return rgbResult;
        }

        #endregion ntsc2rgb

        #region rgb2cmy

        public static List<ArraysListDouble> RGB2CMY(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();

            cmyResult = RGB2CMYCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return cmyResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2CMY(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2cmy operation -> rgb2cmy(List<arraysListInt> rgbList) <-");
            }
            else
            {
                cmyResult = RGB2CMYCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return cmyResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2CMY(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();
            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2cmy operation -> rgb2cmy(int[,] R, int[,] G, int[,]B) <-");
            }
            else
            {
                cmyResult = RGB2CMYCount(R, G, B);
            }

            return cmyResult;
        }

        //C M Y values - double in range [0:1]
        public static List<ArraysListDouble> RGB2CMYCount(int[,] R, int[,] G, int[,] B)
        {        
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();

            double[,] C = new double[height, width];  //Cyan (голубой)
            double[,] M = new double[height, width];  //Magenta (пурпурный)
            double[,] Y = new double[height, width];  //Yellow

            C = R.ImageUint8ToDouble().ConstSubArrayElements(1);  //Cyan (голубой)
            M = G.ImageUint8ToDouble().ConstSubArrayElements(1);  //Magenta (пурпурный)
            Y = B.ImageUint8ToDouble().ConstSubArrayElements(1);  //Yellow

            cmyResult.Add(new ArraysListDouble() { Color = C });
            cmyResult.Add(new ArraysListDouble() { Color = M });
            cmyResult.Add(new ArraysListDouble() { Color = Y });

            return cmyResult;
        }

        #endregion rgb2cmy

        #region cmy2rgb
        //
        public static List<ArraysListInt> CMY2RGB(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            double[,] C = (ColorList[0].Color).ImageUint8ToDouble(); 
            double[,] M = (ColorList[1].Color).ImageUint8ToDouble(); 
            double[,] Y = (ColorList[2].Color).ImageUint8ToDouble(); 

            rgbResult = CMY2RGBCount(C, M, Y);

            return rgbResult;
        }

        //C M Y in double values (as after convert rgb2cmy, in range [0 1])
        //list C M Y arrays in In the following order C-M-Y
        public static List<ArraysListInt> CMY2RGB(List<ArraysListDouble> cmyList)
        {
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
                rgbResult = CMY2RGBCount(cmyList[0].Color, cmyList[1].Color, cmyList[2].Color);
            }

            return rgbResult;
        }

        //C M Y in double values (as after convert rgb2cmy, in range [0 1])
        //C M Y arrays in In the following order C-M-Y
        public static List<ArraysListInt> CMY2RGB(double[,] C, double[,] M, double[,] Y)
        {        
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
                rgbResult = CMY2RGBCount(C, M, Y);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> CMY2RGBCount(double[,] C, double[,] M, double[,] Y)
        {
            int width = C.GetLength(1);
            int height = C.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            int[,] R = new int[height, width];
            int[,] G = new int[height, width];
            int[,] B = new int[height, width];

            R = C.ConstSubArrayElements(1).ImageArrayToUint8(); 
            G = M.ConstSubArrayElements(1).ImageArrayToUint8(); 
            B = Y.ConstSubArrayElements(1).ImageArrayToUint8();

            rgbResult.Add(new ArraysListInt() { Color = R });
            rgbResult.Add(new ArraysListInt() { Color = G });
            rgbResult.Add(new ArraysListInt() { Color = B });

            return rgbResult;
        }

        #endregion cmy2rgb

        #region rgb2YCbCr

        public static List<ArraysListDouble> RGB2YCbCr(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            YCbCrResult = RGB2YCbCrCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return YCbCrResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2YCbCr(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2YCbCr operation -> rgb2YCbCr(List<arraysListInt> rgbList) <-");
            }
            else
            {
                YCbCrResult = RGB2YCbCrCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return YCbCrResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2YCbCr(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2YCbCr operation -> rgb2YCbCr(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                YCbCrResult = RGB2YCbCrCount(R, G, B);
            }

            return YCbCrResult;
        }

        //Y Cb Cr values - double, not in range [0 1]
        public static List<ArraysListDouble> RGB2YCbCrCount(int[,] R, int[,] G, int[,] B)
        { 
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> YCbCrResult = new List<ArraysListDouble>();
                                              
            var Rcd = R.ImageUint8ToDouble(); 
            var Gcd = G.ImageUint8ToDouble(); 
            var Bcd = B.ImageUint8ToDouble(); 

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

                    Y[i, j]  = Coef[0] + Ycon.MultVectors(temp).Sum(); 
                    Cb[i, j] = Coef[1] + Cbcon.MultVectors(temp).Sum(); 
                    Cr[i, j] = Coef[2] + Crcon.MultVectors(temp).Sum(); 
                }
            }

            YCbCrResult.Add(new ArraysListDouble() { Color = Y });
            YCbCrResult.Add(new ArraysListDouble() { Color = Cb });
            YCbCrResult.Add(new ArraysListDouble() { Color = Cr });

            return YCbCrResult;
        }

        #endregion rgb2YCbCr

        #region YCbCr2rgb

        public static List<ArraysListInt> YCbCr2RGB(Bitmap img)
        {          
            var ColorList = Helpers.GetPixels(img);
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            var Y = (ColorList[0].Color).ArrayToDouble(); 
            var Cb = (ColorList[1].Color).ArrayToDouble();
            var Cr = (ColorList[2].Color).ArrayToDouble();

            rgbResult = YCbCr2RGBCount(Y, Cb, Cr);

            return rgbResult;
        }

        //Y Cb Cr in double values (as after convert rgb2YCbCr, not in range [-1 1])
        //list Y Cb Cr arrays in In the following order Y-Cb-Cr
        public static List<ArraysListInt> YCbCr2RGB(List<ArraysListDouble> YCbCrList)
        {          
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
                rgbResult = YCbCr2RGBCount(YCbCrList[0].Color, YCbCrList[1].Color, YCbCrList[2].Color);
            }

            return rgbResult;
        }

        //Y Cb Cr in double values (as after convert rgb2YCbCr, not in range [-1 1])
        //Y Cb Cr arrays in In the following order Y-Cb-Cr
        public static List<ArraysListInt> YCbCr2RGB(double[,] Y, double[,] Cb, double[,] Cr)
        {          
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
                rgbResult = YCbCr2RGBCount(Y, Cb, Cr);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> YCbCr2RGBCount(double[,] Y, double[,] Cb, double[,] Cr)
        {           
            int width = Y.GetLength(1);
            int height = Y.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            Y = Y.ArrayDivByConst(255); 
            Cb = Cb.ArrayDivByConst(255);
            Cr = Cr.ArrayDivByConst(255);

            double[,] R = new double[height, width];
            double[,] G = new double[height, width];
            double[,] B = new double[height, width];

            double[] Ycon  = new double[3] { 298.082, 0, 408.583 };
            double[] Cbcon = new double[3] { 298.082, -100.291, -208.12 };
            double[] Crcon = new double[3] { 298.082, 516.412, 0 };
            double[] Coef  = new double[3] { -222.921, 135.576, -276.836 };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double[] temp = new double[3] { Y[i, j], Cb[i, j], Cr[i, j] };

                    R[i, j] = Coef[0] + Ycon.MultVectors(temp).Sum(); 
                    G[i, j] = Coef[1] + Cbcon.MultVectors(temp).Sum(); 
                    B[i, j] = Coef[2] + Crcon.MultVectors(temp).Sum(); 
                }
            }

            rgbResult.Add(new ArraysListInt() { Color = R.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = G.ArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = B.ArrayToUint8() });

            return rgbResult;
        }

        #endregion YCbCr2rgb

        #region rgb2xyz

        //sRGB to XYZ D65/2
        public static List<ArraysListDouble> RGB2XYZ(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            xyzResult = RGB2XYZCount(ColorList[0].Color, ColorList[1].Color, ColorList[2].Color);

            return xyzResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2XYZ(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2xyz operation -> rgb2xyz(List<arraysListInt> rgbList) <-");
            }
            else
            {
                xyzResult = RGB2XYZCount(rgbList[0].Color, rgbList[1].Color, rgbList[2].Color);
            }

            return xyzResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2XYZ(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2xyz operation -> rgb2xyz(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                xyzResult = RGB2XYZCount(R, G, B);
            }

            return xyzResult;
        }

        //X Y Z values - double > 1, can be <1 if represent small R G B values
        public static List<ArraysListDouble> RGB2XYZCount(int[,] R, int[,] G, int[,] B)
        {
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            var Rcd = R.ImageUint8ToDouble();
            var Gcd = G.ImageUint8ToDouble();
            var Bcd = B.ImageUint8ToDouble();

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
        public static List<ArraysListInt> XYZ2RGB(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            var X = (ColorList[0].Color).ArrayToDouble();
            var Y = (ColorList[1].Color).ArrayToDouble();
            var Z = (ColorList[2].Color).ArrayToDouble();

            rgbResult = XYZ2RGBbCount(X, Y, Z);

            return rgbResult;
        }

        //X Y Z in double values (as after convert rgb2XYZ, not in range [0 1], only if represent small R G B values)
        //list X Y Z arrays in In the following order X-Y-Z
        public static List<ArraysListInt> XYZ2RGB(List<ArraysListDouble> xyzList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (xyzList[0].Color.Length != xyzList[1].Color.Length || xyzList[0].Color.Length != xyzList[2].Color.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2rgb operation -> xyz2rgb(List<arraysListDouble> xyzList) <-");
            }
            else
            {
                rgbResult = XYZ2RGBbCount(xyzList[0].Color, xyzList[1].Color, xyzList[2].Color);
            }

            return rgbResult;
        }

        //X Y Z in double values (as after convert rgb2XYZ, not in range [0 1], only if represent small R G B values)
        //X Y Z arrays in In the following order X-Y-Z
        public static List<ArraysListInt> XYZ2RGB(double[,] X, double[,] Y, double[,] Z)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (X.Length != Y.Length || X.Length != Z.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2rgb operation -> xyz2rgb(double[,] X, double[,] Y, double[,] Z) <-");
            }
            else
            {
                rgbResult = XYZ2RGBbCount(X, Y, Z);
            }

            return rgbResult;
        }

        public static List<ArraysListInt> XYZ2RGBbCount(double[,] X, double[,] Y, double[,] Z)
        {
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

            rgbResult.Add(new ArraysListInt() { Color = R.ImageArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = G.ImageArrayToUint8() });
            rgbResult.Add(new ArraysListInt() { Color = B.ImageArrayToUint8() });

            return rgbResult;
        }

        #endregion xyz2rgb

        #region xyz2lab

        //bad when from file, coz lost a lot after round values in all planes, when saved xyz result to file
        public static List<ArraysListDouble> XYZ2Lab(Bitmap img)
        {          
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();
                                                          
            var X = (ColorList[0].Color).ArrayToDouble(); 
            var Y = (ColorList[1].Color).ArrayToDouble(); 
            var Z = (ColorList[2].Color).ArrayToDouble(); 

            labResult = XYZ2LabCount(X, Y, Z);

            return labResult;
        }

        //List with X Y Z arrays in In the following order X Y Z
        public static List<ArraysListDouble> XYZ2Lab(List<ArraysListDouble> xyzList)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (xyzList[0].Color.Length != xyzList[1].Color.Length || xyzList[0].Color.Length != xyzList[2].Color.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2lab operation -> xyz2lab(List<arraysListInt> xyzList) <-");
            }
            else
            {
                labResult = XYZ2LabCount(xyzList[0].Color, xyzList[1].Color, xyzList[2].Color);
            }

            return labResult;
        }

        //X Y Z arrays in In the following order X Y Z
        public static List<ArraysListDouble> XYZ2Lab(double[,] X, double[,] Y, double[,] Z)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (X.Length != Y.Length || X.Length != Z.Length)
            {
                Console.WriteLine("X Y Z arrays size dismatch in xyz2lab operation -> xyz2lab(double[,] X, double[,] Y, double[,] Z) <-");
            }
            else
            {
                labResult = XYZ2LabCount(X, Y, Z);
            }

            return labResult;
        }

        //L values - double, not in [0 1] range, a & b - same, but have negative values
        public static List<ArraysListDouble> XYZ2LabCount(double[,] X, double[,] Y, double[,] Z)
        {            
            int width = X.GetLength(1);
            int height = X.GetLength(0);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            const double X_D65 = 95.047;
            const double Y_D65 = 100;
            const double Z_D65 = 108.883;

            X = X.ArrayDivByConst(X_D65); 
            Y = Y.ArrayDivByConst(Y_D65); 
            Z = Z.ArrayDivByConst(Z_D65); 

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
        public static List<ArraysListDouble> Lab2XYZ(Bitmap img)
        {           
            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            double[,] L = (ColorList[0].Color).ArrayToDouble(); 
            double[,] a = (ColorList[1].Color).ArrayToDouble(); 
            double[,] b = (ColorList[2].Color).ArrayToDouble(); 

            xyzResult = Lab2XYZCount(L, a, b);

            return xyzResult;
        }

        //L a b in double values (as after convert rgb2XYZ, not in range [0 1])
        //list L a b arrays in In the following order L-a-b
        public static List<ArraysListDouble> Lab2XYZ(List<ArraysListDouble> labList)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (labList[0].Color.Length != labList[1].Color.Length || labList[0].Color.Length != labList[2].Color.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2xyz operation -> lab2xyz(List<arraysListDouble> labList) <-");
            }
            else
            {
                xyzResult = Lab2XYZCount(labList[0].Color, labList[1].Color, labList[2].Color);
            }

            return xyzResult;
        }

        //L a b in double values (as after convert rgb2XYZ, not in range [0 1])
        //L a b arrays in In the following order L-a-b
        public static List<ArraysListDouble> Lab2XYZ(double[,] L, double[,] a, double[,] b)
        {
            List<ArraysListDouble> xyzResult = new List<ArraysListDouble>();

            if (L.Length != a.Length || L.Length != b.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2xyz operation -> lab2xyz(double[,] L, double[,] a, double[,] b) <-");
            }
            else
            {
                xyzResult = Lab2XYZCount(L, a, b);
            }

            return xyzResult;
        }

        //X Y Z values - double > 1, can be <1 if represent small R G B values
        public static List<ArraysListDouble> Lab2XYZCount(double[,] L, double[,] a, double[,] b)
        {         
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
        public static List<ArraysListDouble> RGB2Lab(Bitmap img)
        { 
            int width = img.Width;
            int height = img.Height;

            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            var xyz = ColorSpace.RGB2XYZ(ColorList);
            labResult = ColorSpace.XYZ2Lab(xyz);

            return labResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab(List<ArraysListInt> rgbList)
        {            
            int width = rgbList[0].Color.GetLength(1);
            int height = rgbList[0].Color.GetLength(0);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("list R G B arrays size dismatch in rgb2lab operation -> rgb2lab(List<arraysListInt> rgbList) <-");
            }
            else
            {
                var xyz = ColorSpace.RGB2XYZ(rgbList);
                labResult = ColorSpace.XYZ2Lab(xyz);
            }

            return labResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab(int[,] R, int[,] G, int[,] B)
        {    
            int width = R.GetLength(1);
            int height = R.GetLength(0);

            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            List<ArraysListInt> rgbList = new List<ArraysListInt>
            {
                new ArraysListInt() { Color = R },
                new ArraysListInt() { Color = G },
                new ArraysListInt() { Color = B }
            };

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2lab operation -> rgb2lab(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                var xyz = ColorSpace.RGB2XYZ(rgbList);
                labResult = ColorSpace.XYZ2Lab(xyz);
            }

            return labResult;
        }

        #endregion rgb2lab

        #region lab2rgb
        //bad, when from file. Lost a lot from converting and round
        public static List<ArraysListInt> Lab2RGB(Bitmap img)
        {          
            int width = img.Width;
            int height = img.Height;

            var ColorList = Helpers.GetPixels(img);

            List<ArraysListDouble> lablist = new List<ArraysListDouble>
            {
                new ArraysListDouble() { Color = (ColorList[0].Color).ArrayToDouble() },
                new ArraysListDouble() { Color = (ColorList[1].Color).ArrayToDouble() },
                new ArraysListDouble() { Color = (ColorList[2].Color).ArrayToDouble() }
            };

            var labxyz = ColorSpace.Lab2XYZ(lablist);
            var xyzrgb = ColorSpace.XYZ2RGB(labxyz);

            return xyzrgb;
        }

        //L a b in double values (as after convert XYZ2lab, not in range [0 1])
        //list L a b arrays in In the following order L-a-b
        public static List<ArraysListInt> Lab2RGB(List<ArraysListDouble> labList)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (labList[0].Color.Length != labList[1].Color.Length || labList[0].Color.Length != labList[2].Color.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2rgb operation -> lab2rgb(List<arraysListDouble> labList) <-");
            }
            else
            {
                var labxyz = ColorSpace.Lab2XYZ(labList);
                var xyzrgb = ColorSpace.XYZ2RGB(labxyz);

                rgbResult = xyzrgb;
            }

            return rgbResult;
        }

        //L a b in double values (as after convert XYZ2lab, not in range [0 1])
        //L a b arrays in In the following order L-a-b
        public static List<ArraysListInt> Lab2RGB(double[,] L, double[,] a, double[,] b)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            List<ArraysListDouble> labList = new List<ArraysListDouble>
            {
                new ArraysListDouble() { Color = L },
                new ArraysListDouble() { Color = a },
                new ArraysListDouble() { Color = b }
            };

            if (L.Length != a.Length || L.Length != b.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2rgb operation -> lab2rgb(double[,] L, double[,] a, double[,] b) <-");
            }
            else
            {
                var labxyz = ColorSpace.Lab2XYZ(labList);
                var xyzrgb = ColorSpace.XYZ2RGB(labxyz);

                rgbResult = xyzrgb;
            }

            return rgbResult;
        }

        #endregion lab2rgb

        public static double[,] FakeCIE1976L(Bitmap img)
        {  
            var lab = ColorSpace.RGB2Lab(img);

            double[,] L = lab[0].Color;
            L = L.ArrayMultByConst(2.57);

            return L;
        }
    }

    public static class ColorSpaceToFile
    {
        //if image is 24bpp, with H\S\V convert in range [0...255] as rgb etc
        public static void ColorSpaceToFileBlank(int[,] R, int[,] G, int[,] B, ColorSpaceType colorSpace)
        {

        }
        public static void ColorSpaceToFileBlank(double[,] R, double[,] G, double[,] B, ColorSpaceType colorSpace)
        {

        }

        //all 2rgb looks good, if obtained not from file, but from rgb and made some filtering or another process, and saved as rgb back
        public static void AnothercolorSpacetoRGBXYZLabtoFile(List<ArraysListDouble> colorPlane, AnotherColorSpacetoRGBaXYZLab colorSpace) //how generic arraysListT here?
        {         
            int width  = colorPlane[0].Color.GetLength(1);
            int height = colorPlane[0].Color.GetLength(0);
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[height, width];
            int[,] colorPlaneTwo   = new int[height, width];
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
                        rgbResult = ColorSpace.HSV2RGB(colorPlane);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\hsv2rgb.jpeg";
                        break;

                    case "ntsc2rgb":
                        rgbResult = ColorSpace.NTSC2RGB(colorPlane);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //when ntsc2rgb from file
                        //approximate result in file, coz we lost negative values in I and Q when saving ntsc result in file [0..255]
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\ntsc2rgb.jpeg";
                        break;

                    case "cmy2rgb":
                        rgbResult = ColorSpace.CMY2RGB(colorPlane);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\cmy2rgb.jpeg";
                        break;

                    case "YCbCr2rgb":
                        rgbResult = ColorSpace.YCbCr2RGB(colorPlane);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\YCbCr2rgb.jpeg";
                        break;

                    case "xyz2rgb":
                        rgbResult = ColorSpace.XYZ2RGB(colorPlane);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //bad when from file, coz using heavy rounded X Y Z values, when writing them to file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2rgb.jpeg";
                        break;

                    case "xyz2lab":
                        var xyzlabResult = ColorSpace.XYZ2Lab(colorPlane);

                        colorPlaneOne   = (xyzlabResult[0].Color).ArrayToUint8(); 
                        colorPlaneTwo   = (xyzlabResult[1].Color).ArrayToUint8(); 
                        colorPlaneThree = (xyzlabResult[2].Color).ArrayToUint8(); 

                        //bad when from file, coz xyz values rounded, and lost negative value in a & b when saving in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\xyz2lab.jpeg";
                        break;

                    case "lab2xyz":
                        var labxyzResult = ColorSpace.Lab2XYZ(colorPlane);

                        colorPlaneOne   = (labxyzResult[0].Color).ArrayToUint8(); 
                        colorPlaneTwo   = (labxyzResult[1].Color).ArrayToUint8(); 
                        colorPlaneThree = (labxyzResult[2].Color).ArrayToUint8(); 

                        //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2xyz.jpeg";
                        break;

                    case "lab2rgb":
                        rgbResult = ColorSpace.Lab2RGB(colorPlane);

                        colorPlaneOne   = rgbResult[0].Color;
                        colorPlaneTwo   = rgbResult[1].Color;
                        colorPlaneThree = rgbResult[2].Color;

                        //if from file
                        //very bad, coz lost a lot in converting and round everywhere...
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\lab2rgb.jpeg";
                        break;

                    default:

                        colorPlaneOne   = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneTwo   = Helpers.RandArray(height, width, 0, 255);
                        colorPlaneThree = Helpers.RandArray(height, width, 0, 255);

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\defaultNonColorSpace.jpeg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            outName = MoreHelpers.OutputFileNames(outName);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ".jpeg");
        }

        public static void AnothercolorSpacetoRGBXYZLabtoFile(Bitmap image, AnotherColorSpacetoRGBaXYZLab colorSpace)
        { }

        //some rgb2 looks good, some lost negative values, when ranged to [0..255] for saving
        public static void RGBtoAnothercolorSpacetoFile(List<ArraysListInt> colorPlane, RGBtoAnotherColorSpace colorSpace) //how generic arraysListT here?
        {
            int width  = colorPlane[0].Color.GetLength(1);
            int height = colorPlane[0].Color.GetLength(0);
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);

            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[height, width];
            int[,] colorPlaneTwo   = new int[height, width];
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
                        var hsvResult = ColorSpace.RGB2HSV(colorPlane);

                        colorPlaneOne   = (hsvResult[0].Color).ArrayDivByConst(360).ImageArrayToUint8(); 
                        colorPlaneTwo   = (hsvResult[1].Color).ImageArrayToUint8(); 
                        colorPlaneThree = (hsvResult[2].Color).ImageArrayToUint8();

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2hsv.jpeg";
                        break;

                    case "rgb2ntsc":
                        var ntscResult = ColorSpace.RGB2NTSC(colorPlane);

                        colorPlaneOne   = (ntscResult[0].Color).ArrayToUint8(); 
                        colorPlaneTwo   = (ntscResult[1].Color).ArrayToUint8(); 
                        colorPlaneThree = (ntscResult[2].Color).ArrayToUint8(); 

                        //if we want to save rgb2ntsc result in file
                        //approximate result in file, coz we lost negative values in I and Q
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2ntsc.jpeg";
                        break;

                    case "rgb2cmy":
                        var cmyResult = ColorSpace.RGB2CMY(colorPlane);

                        colorPlaneOne   = (cmyResult[0].Color).ImageArrayToUint8(); 
                        colorPlaneTwo   = (cmyResult[1].Color).ImageArrayToUint8(); 
                        colorPlaneThree = (cmyResult[2].Color).ImageArrayToUint8(); 

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2cmy.jpeg";
                        break;

                    case "rgb2YCbCr":
                        var YCbCrResult = ColorSpace.RGB2YCbCr(colorPlane);

                        colorPlaneOne   = (YCbCrResult[0].Color).ArrayToUint8(); 
                        colorPlaneTwo   = (YCbCrResult[1].Color).ArrayToUint8(); 
                        colorPlaneThree = (YCbCrResult[2].Color).ArrayToUint8(); 

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2YCbCr.jpeg";
                        break;

                    case "rgb2xyz":
                        var xyzrgbResult = ColorSpace.RGB2XYZ(colorPlane);

                        colorPlaneOne   = (xyzrgbResult[0].Color).ArrayToUint8(); 
                        colorPlaneTwo   = (xyzrgbResult[1].Color).ArrayToUint8(); 
                        colorPlaneThree = (xyzrgbResult[2].Color).ArrayToUint8(); 

                        //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2xyz.jpeg";
                        break;

                    case "rgb2lab":
                        var rgblabResult = ColorSpace.RGB2Lab(colorPlane);

                        colorPlaneOne   = (rgblabResult[0].Color).ArrayToUint8(); 
                        colorPlaneTwo   = (rgblabResult[1].Color).ArrayToUint8(); 
                        colorPlaneThree = (rgblabResult[2].Color).ArrayToUint8(); 

                        //bad, coz lost negative value in a & b when saving in [0..255] range into file
                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\rgb2lab.jpeg";
                        break;

                    default:
                        colorPlaneOne   = colorPlane[0].Color;
                        colorPlaneTwo   = colorPlane[1].Color;
                        colorPlaneThree = colorPlane[2].Color;

                        outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\defaultNonColorSpace.jpeg";
                        break;
                }
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            outName = MoreHelpers.OutputFileNames(outName);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ".jpeg");
        }

        public static void RGBtoAnothercolorSpacetoFile(Bitmap image, RGBtoAnotherColorSpace colorSpace)
        { }

        //if direct from file
        public static void ColorSpaceToFileDirectFromImage(Bitmap img, ColorSpaceType colorSpace, string fileName)
        {          
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            //back result [0 .. 255]
            int[,] colorPlaneOne   = new int[img.Height, img.Width];
            int[,] colorPlaneTwo   = new int[img.Height, img.Width];
            int[,] colorPlaneThree = new int[img.Height, img.Width];

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            string outName = String.Empty;

            switch (colorSpace.ToString())
            {
                case "rgb2hsv":
                    var hsvResult = ColorSpace.RGB2HSV(img);

                    colorPlaneOne   = (hsvResult[0].Color).ArrayDivByConst(360).ImageArrayToUint8();
                    colorPlaneTwo   = (hsvResult[1].Color).ImageArrayToUint8(); 
                    colorPlaneThree = (hsvResult[2].Color).ImageArrayToUint8(); 

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2hsv" + ImgExtension;
                    break;

                case "hsv2rgb":
                    rgbResult = ColorSpace.HSV2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_hsv2rgb" + ImgExtension;
                    break;

                case "rgb2ntsc":
                    var ntscResult = ColorSpace.RGB2NTSC(img);
                                                                            
                    colorPlaneOne   = (ntscResult[0].Color).ArrayToUint8(); 
                    colorPlaneTwo   = (ntscResult[1].Color).ArrayToUint8(); 
                    colorPlaneThree = (ntscResult[2].Color).ArrayToUint8(); 

                    //if we want to save rgb2ntsc result in file
                    //approximate result in file, coz we lost negative values in I and Q
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2ntsc" + ImgExtension;
                    break;

                case "ntsc2rgb":
                    rgbResult = ColorSpace.NTSC2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //when ntsc2rgb from file
                    //approximate result in file, coz we lost negative values in I and Q when saving ntsc result in file [0..255]
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_ntsc2rgb" + ImgExtension;
                    break;

                case "rgb2cmy":
                    var cmyResult = ColorSpace.RGB2CMY(img);

                    colorPlaneOne   = (cmyResult[0].Color).ImageArrayToUint8(); 
                    colorPlaneTwo   = (cmyResult[1].Color).ImageArrayToUint8(); 
                    colorPlaneThree = (cmyResult[2].Color).ImageArrayToUint8(); 

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2cmy" + ImgExtension;
                    break;

                case "cmy2rgb":
                    rgbResult = ColorSpace.CMY2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_cmy2rgb" + ImgExtension;
                    break;

                case "rgb2YCbCr":
                    var YCbCrResult = ColorSpace.RGB2YCbCr(img);

                    colorPlaneOne   = (YCbCrResult[0].Color).ArrayToUint8(); 
                    colorPlaneTwo   = (YCbCrResult[1].Color).ArrayToUint8(); 
                    colorPlaneThree = (YCbCrResult[2].Color).ArrayToUint8(); 

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2YCbCr" + ImgExtension;
                    break;

                case "YCbCr2rgb":
                    rgbResult = ColorSpace.YCbCr2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_YCbCr2rgb" + ImgExtension;
                    break;

                case "rgb2xyz":
                    var xyzrgbResult = ColorSpace.RGB2XYZ(img);

                    colorPlaneOne   = (xyzrgbResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (xyzrgbResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (xyzrgbResult[2].Color).ArrayToUint8();

                    //approximate result in file, coz we lost values after comma in saving ntsc result in file [0..255] and heavy round them                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2xyz" + ImgExtension;
                    break;

                case "xyz2rgb":
                    rgbResult = ColorSpace.XYZ2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //bad when from file, coz using heavy rounded X Y Z values, when writing them to file                  
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_xyz2rgb" + ImgExtension;
                    break;

                case "xyz2lab":
                    var xyzlabResult = ColorSpace.XYZ2Lab(img);

                    colorPlaneOne   = (xyzlabResult[0].Color).ArrayToUint8(); 
                    colorPlaneTwo   = (xyzlabResult[1].Color).ArrayToUint8(); 
                    colorPlaneThree = (xyzlabResult[2].Color).ArrayToUint8(); 

                    //bad when from file, coz xyz values rounded, and lost negative value in a & b when saving in [0..255] range into file                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_xyz2lab" + ImgExtension;
                    break;

                case "lab2xyz":
                    var labxyzResult = ColorSpace.Lab2XYZ(img);

                    colorPlaneOne   = (labxyzResult[0].Color).ArrayToUint8(); 
                    colorPlaneTwo   = (labxyzResult[1].Color).ArrayToUint8(); 
                    colorPlaneThree = (labxyzResult[2].Color).ArrayToUint8(); 

                    //bad when from file, coz lost a and b negative value when save to file. And lost X Y Z values when round before save in [0..255] range into file                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_lab2xyz" + ImgExtension;
                    break;

                case "rgb2lab":
                    var rgblabResult = ColorSpace.RGB2Lab(img);

                    colorPlaneOne   = (rgblabResult[0].Color).ArrayToUint8();
                    colorPlaneTwo   = (rgblabResult[1].Color).ArrayToUint8();
                    colorPlaneThree = (rgblabResult[2].Color).ArrayToUint8();

                    //bad, coz lost negative value in a & b when saving in [0..255] range into file                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_rgb2lab" + ImgExtension;
                    break;

                case "lab2rgb":
                    rgbResult = ColorSpace.Lab2RGB(img);

                    colorPlaneOne   = rgbResult[0].Color;
                    colorPlaneTwo   = rgbResult[1].Color;
                    colorPlaneThree = rgbResult[2].Color;

                    //very bad, coz lost a lot in converting and round everywhere...                    
                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_lab2rgb" + ImgExtension;
                    break;

                default:
                    colorPlaneOne   = Helpers.GetPixels(img)[0].Color;
                    colorPlaneTwo   = Helpers.GetPixels(img)[1].Color;
                    colorPlaneThree = Helpers.GetPixels(img)[2].Color;

                    outName = Directory.GetCurrentDirectory() + "\\ColorSpace\\" + fileName + "_defaultNonColorSpace" + ImgExtension;
                    break;
            }

            image = Helpers.SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

            outName = MoreHelpers.OutputFileNames(outName);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void FakeCIE1976LtoFile(Bitmap img, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\ColorSpace");

            var fake = ColorSpace.FakeCIE1976L(img);

            fileName = fileName + "_FakeCIE1976L" + ImgExtension;
            Helpers.WriteImageToFile(fake, fake, fake, fileName, "ColorSpace");
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
    public enum RGBtoAnotherColorSpace
    {
        rgb2hsv,
        rgb2ntsc,
        rgb2cmy,
        rgb2YCbCr,
        rgb2xyz,
        rgb2lab
    }

    //
    public enum AnotherColorSpacetoRGBaXYZLab
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
