using System;
using System.Drawing;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image.ColorSpaces
{
    public static class RGBandLab
    {
        #region rgb2lab
        public static List<ArraysListDouble> RGB2Lab(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            var labResult = new List<ArraysListDouble>();

            var xyz = RGBandXYZ.RGB2XYZ(ColorList);
            labResult = XYZandLab.XYZ2Lab(xyz);

            return labResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("list R G B arrays size dismatch in rgb2lab operation -> rgb2lab(List<arraysListInt> rgbList) <-");
            }
            else
            {
                var xyz = RGBandXYZ.RGB2XYZ(rgbList);
                labResult = XYZandLab.XYZ2Lab(xyz);
            }

            return labResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab(int[,] r, int[,] g, int[,] b)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (r.Length != g.Length || r.Length != b.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in rgb2lab operation -> rgb2lab(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                var xyz = RGBandXYZ.RGB2XYZ(r, g, b);
                labResult = XYZandLab.XYZ2Lab(xyz);
            }

            return labResult;
        }

        #endregion rgb2lab

        #region lab2rgb
        //bad, when from file. Lost a lot from converting and round
        public static List<ArraysListInt> Lab2RGB(Bitmap img)
        {
            var labxyz = XYZandLab.Lab2XYZ(img);
            var xyzrgb = RGBandXYZ.XYZ2RGB(labxyz);

            return xyzrgb;
        }

        //L a b in double values (as after convert XYZ2lab; not in range [0 1])
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
                List<ArraysListDouble> labxyz = XYZandLab.Lab2XYZ(labList);
                List<ArraysListInt> xyzrgb = RGBandXYZ.XYZ2RGB(labxyz);

                rgbResult = xyzrgb;
            }

            return rgbResult;
        }

        //L a b in double values (as after convert XYZ2lab; not in range [0 1])
        //L a b arrays in In the following order L-a-b
        public static List<ArraysListInt> Lab2RGB(double[,] l, double[,] a, double[,] b)
        {
            List<ArraysListInt> rgbResult = new List<ArraysListInt>();

            if (l.Length != a.Length || l.Length != b.Length)
            {
                Console.WriteLine("L a b arrays size dismatch in lab2rgb operation -> lab2rgb(double[,] L, double[,] a, double[,] b) <-");
            }
            else
            {
                List<ArraysListDouble> labxyz = XYZandLab.Lab2XYZ(l, a, b);
                List<ArraysListInt> xyzrgb = RGBandXYZ.XYZ2RGB(labxyz);

                rgbResult = xyzrgb;
            }

            return rgbResult;
        }

        #endregion lab2rgb

        #region rgb2fakelab1976

        public static List<ArraysListDouble> RGB2Lab1976(Bitmap img)
        {
            List<ArraysListDouble> labResult = RGB2Lab(img);
            labResult[0].Color = (labResult[0].Color).ArrayMultByConst(2.57);

            return labResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab1976(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> labResult = RGB2Lab(rgbList);
            labResult[0].Color = (labResult[0].Color).ArrayMultByConst(2.57);

            return labResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab1976(int[,] r, int[,] g, int[,] b)
        {
            List<ArraysListDouble> labResult = RGB2Lab(r, g, b);
            labResult[0].Color = (labResult[0].Color).ArrayMultByConst(2.57);

            return labResult;
        }

        #endregion rgb2lab1976

        #region fakelab1976torgb

        //bad, when from file. Lost a lot from converting and round
        public static List<ArraysListInt> Lab1976toRGB(Bitmap img)
        {
            List<ArraysListInt> ColorList = Helpers.GetPixels(img);
            var labxyz = XYZandLab.Lab2XYZ((ColorList[0].Color).ArrayToDouble().ArrayDivByConst(2.57),
                ColorList[1].Color.ArrayToDouble(), ColorList[2].Color.ArrayToDouble());

            var xyzrgb = Lab2RGB(labxyz);

            return xyzrgb;
        }

        //L a b in double values (as after convert XYZ2lab; not in range [0 1])
        //list L a b arrays in In the following order L-a-b
        public static List<ArraysListInt> Lab1976toRGB(List<ArraysListDouble> labList)
        {
            labList[0].Color = labList[0].Color.ArrayDivByConst(2.57);
            List<ArraysListInt> rgbResult = Lab2RGB(labList);

            return rgbResult;
        }

        //L a b in double values (as after convert XYZ2lab; not in range [0 1])
        //L a b arrays in In the following order L-a-b
        public static List<ArraysListInt> Lab1976toRGB(double[,] l, double[,] a, double[,] b)
        {
            l = l.ArrayDivByConst(2.57);
            List<ArraysListInt> rgbResult = Lab2RGB(l, a, b);

            return rgbResult;
        }

        #endregion
    }
}
