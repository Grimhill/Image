using System;
using System.Collections.Generic;
using System.Drawing;
using Image.ArrayOperations;

namespace Image.ColorSpaces
{
    public static class RGBandLab
    {
        #region rgb2lab
        public static List<ArraysListDouble> RGB2Lab(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

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
        public static List<ArraysListDouble> RGB2Lab(int[,] R, int[,] G, int[,] B)
        {
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
                var xyz = RGBandXYZ.RGB2XYZ(rgbList);
                labResult = XYZandLab.XYZ2Lab(xyz);
            }

            return labResult;
        }

        #endregion rgb2lab

        #region lab2rgb
        //bad, when from file. Lost a lot from converting and round
        public static List<ArraysListInt> Lab2RGB(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> lablist = new List<ArraysListDouble>
            {
                new ArraysListDouble() { Color = (ColorList[0].Color).ArrayToDouble() },
                new ArraysListDouble() { Color = (ColorList[1].Color).ArrayToDouble() },
                new ArraysListDouble() { Color = (ColorList[2].Color).ArrayToDouble() }
            };

            var labxyz = XYZandLab.Lab2XYZ(lablist);
            var xyzrgb = RGBandXYZ.XYZ2RGB(labxyz);

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
                var labxyz = XYZandLab.Lab2XYZ(labList);
                var xyzrgb = RGBandXYZ.XYZ2RGB(labxyz);

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
                var labxyz = XYZandLab.Lab2XYZ(labList);
                var xyzrgb = RGBandXYZ.XYZ2RGB(labxyz);

                rgbResult = xyzrgb;
            }

            return rgbResult;
        }

        #endregion lab2rgb

        #region rgb2lab1976

        public static List<ArraysListDouble> RGB2Lab1976(Bitmap img)
        {
            var ColorList = Helpers.GetPixels(img);
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            var xyz = RGBandXYZ.RGB2XYZ(ColorList);
            labResult = XYZandLab.XYZ2Lab(xyz);
            labResult[0].Color = (labResult[0].Color).ArrayMultByConst(2.57);

            return labResult;
        }

        //List with R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab1976(List<ArraysListInt> rgbList)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            if (rgbList[0].Color.Length != rgbList[1].Color.Length || rgbList[0].Color.Length != rgbList[2].Color.Length)
            {
                Console.WriteLine("list R G B arrays size dismatch in RGB2Lab1976 operation -> RGB2Lab1976(List<arraysListInt> rgbList) <-");
            }
            else
            {
                var xyz = RGBandXYZ.RGB2XYZ(rgbList);
                labResult = XYZandLab.XYZ2Lab(xyz);
                labResult[0].Color = (labResult[0].Color).ArrayMultByConst(2.57);
            }

            return labResult;
        }

        //R G B arrays in In the following order R G B
        public static List<ArraysListDouble> RGB2Lab1976(int[,] R, int[,] G, int[,] B)
        {
            List<ArraysListDouble> labResult = new List<ArraysListDouble>();

            List<ArraysListInt> rgbList = new List<ArraysListInt>
            {
                new ArraysListInt() { Color = R },
                new ArraysListInt() { Color = G },
                new ArraysListInt() { Color = B }
            };

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("R G B arrays size dismatch in RGB2Lab1976 operation -> RGB2Lab1976(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                var xyz = RGBandXYZ.RGB2XYZ(rgbList);
                labResult = XYZandLab.XYZ2Lab(xyz);
                labResult[0].Color = (labResult[0].Color).ArrayMultByConst(2.57);
            }

            return labResult;
        }

        #endregion rgb2lab1976
    }
}
