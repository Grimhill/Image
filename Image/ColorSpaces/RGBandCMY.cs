using System;
using System.Linq;
using System.Drawing;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image.ColorSpaces
{
    public static class RGBandCMY
    {
        #region rgb2cmy

        public static List<ArraysListDouble> RGB2CMY(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
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
        public static List<ArraysListDouble> RGB2CMY(int[,] r, int[,] g, int[,] b)
        {
            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();
            if (r.Length != g.Length || r.Length != b.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2cmy operation -> rgb2cmy(int[,] R, int[,] G, int[,]B) <-");
            }
            else
            {
                cmyResult = RGB2CMYCount(r, g, b);
            }

            return cmyResult;
        }

        //C M Y values - double in range [0:1]
        private static List<ArraysListDouble> RGB2CMYCount(int[,] r, int[,] g, int[,] b)
        {
            int width  = r.GetLength(1);
            int height = r.GetLength(0);

            List<ArraysListDouble> cmyResult = new List<ArraysListDouble>();

            double[,] C = new double[height, width];  //Cyan (голубой)
            double[,] M = new double[height, width];  //Magenta (пурпурный)
            double[,] Y = new double[height, width];  //Yellow

            //formula C = 1 - R/255
            C = r.ImageUint8ToDouble().ConstSubArrayElements(1);
            M = g.ImageUint8ToDouble().ConstSubArrayElements(1);
            Y = b.ImageUint8ToDouble().ConstSubArrayElements(1);

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
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
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
                //may be need transform?
                //cmyList[0].Color = (cmyList[0].c).ArrayDivByConst(255);
                //cmyList[1].Color = (cmyList[1].c).ArrayDivByConst(255);
                //cmyList[2].Color = (cmyList[2].c).ArrayDivByConst(255);                
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
        public static List<ArraysListInt> CMY2RGB(double[,] c, double[,] m, double[,] y)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (c.Length != m.Length || c.Length != y.Length)
            {
                Console.WriteLine("C M Y arrays size dismatch in cmy2rgb operation -> cmy2rgb(double[;] C; double[;] M; double[;] Y) <-");
            }
            else if (c.Cast<double>().ToArray().Max() > 1)
            {
                //may be need transform?
                //C = C.ArrayDivByConst(255);
                //M = M.ArrayDivByConst(255);
                //Y = Y.ArrayDivByConst(255);
                Console.WriteLine("C M Y arrays Values must be in range [0 1]; in cmy2rgb operation -> cmy2rgb(double[;] C; double[;] M; double[;] Y) <-");
            }
            else
            {
                rgbResult = CMY2RGBCount(c, m, y);
            }

            return rgbResult;
        }

        private static List<ArraysListInt> CMY2RGBCount(double[,] c, double[,] m, double[,] y)
        {
            int width  = c.GetLength(1);
            int height = c.GetLength(0);

            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            int[,] R = new int[height, width];
            int[,] G = new int[height, width];
            int[,] B = new int[height, width];

            //formula R = (1 - C) * 255
            //C\M\Y in 0.. 1 range
            R = c.ConstSubArrayElements(1).ImageDoubleToUint8();
            G = m.ConstSubArrayElements(1).ImageDoubleToUint8();
            B = y.ConstSubArrayElements(1).ImageDoubleToUint8();

            rgbResult.Add(new ArraysListInt() { Color = R });
            rgbResult.Add(new ArraysListInt() { Color = G });
            rgbResult.Add(new ArraysListInt() { Color = B });

            return rgbResult;
        }

        #endregion cmy2rgb
    }
}
