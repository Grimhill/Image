using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Image
{
    public static class Segmentation
    {
        public static void findLines(Bitmap img, lineDirection lineDir)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] lineRes = new int[img.Height, img.Width];
            string outName = String.Empty;

            var im = Helpers.rgbToGrayArray(img);

            switch (lineDir.ToString())
            {
                case "horizontal":
                    double[,] horisontalFilter = { { -1, -1, -1 }, { 2, 2, 2 }, { -1, -1, -1 } };

                    lineRes = findLineHelper(im, horisontalFilter);
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\HorisontalLine.jpg";
                    break;

                case "vertical":
                    double[,] verticalFilter = { { -1, 2, -1 }, { -1, 2, -1 }, { -1, 2, -1 } };

                    lineRes = findLineHelper(im, verticalFilter);
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\verticalLine.jpg";
                    break;

                case "plus45":
                    double[,] plus45Filter = { { -1, -1, 2 }, { -1, 2, -1 }, { 2, -1, -1 } };

                    lineRes = findLineHelper(im, plus45Filter);
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\plus45Line.jpg";
                    break;

                case "minus45":
                    double[,] minus45Filter = { { 2, -1, -1 }, { -1, 2, -1 }, { -1, -1, 2 } };

                    lineRes = findLineHelper(im, minus45Filter);
                    outName = Directory.GetCurrentDirectory() + "\\Rand\\minus45Line.jpg";
                    break;
            }


            image = Helpers.setPixels(image, lineRes, lineRes, lineRes);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static int[,] findLineHelper(int[,] arr, double[,] filter)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];

            var temp = ArrOp.AbsArrayElements(Filter.filter_double(ArrOp.ImageUint8ToDouble(arr), filter, PadType.replicate));

            var max = temp.Cast<double>().ToArray().Max();

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    if (temp[i, j] >= max)
                    {
                        result[i, j] = 255;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }

            return result;
        }

        //
        //bad, very bad. worth
        public static void Edge(Bitmap img, inEdge inIm, edgevar var)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 4; // for calculating the automatic threshold
            double Depth = 0;

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
            }

            if (var.ToString() == "var1")
            {
                result = edgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both, "EdgeDefaultVar1_temp");
                outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeDefaultVar1.jpg";
            }
            else
            {
                result = edgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both);
                outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeDefaultVar2.jpg";
            }

            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, inEdge inIm, edgevar var, double tresh)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 4; // for calculating the automatic threshold
            double Depth = 0;

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
            }

            if (var.ToString() == "var1")
            {
                result = edgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both, "EdgeDefaultVar1_temp");
                outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeDefaultVar1.jpg";
            }
            else
            {
                result = edgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both);
                outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeDefaultVar2.jpg";
            }

            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static void Edge(Bitmap img, inEdge inIm, edgevar var, edgeMethod method)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 0; // for calculating the automatic threshold
            double Depth = 0;

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
            }

            switch (method.ToString())
            {
                case "Sobel":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both, "EdgeSobelVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar2.jpg";
                    }
                    break;

                case "Prewitt":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, edgeDirection.both, "EdgePrewittVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, edgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar2.jpg";
                    }
                    break;

                case "Roberts":
                    scale = 6;

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, edgeDirection.both, "EdgeRobertsVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, edgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar2.jpg";
                    }
                    break;
            }

            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, inEdge inIm, edgevar var, edgeMethod method, double tresh)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 0; // for calculating the automatic threshold
            double Depth = 0;

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
            }

            switch (method.ToString())
            {
                case "Sobel":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both, "EdgeSobelVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, edgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar2.jpg";
                    }
                    break;

                case "Prewitt":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, edgeDirection.both, "EdgePrewittVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, edgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar2.jpg";
                    }
                    break;

                case "Roberts":
                    scale = 6;

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, edgeDirection.both, "EdgeRobertsVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, edgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar2.jpg";
                    }
                    break;
            }

            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static void Edge(Bitmap img, inEdge inIm, edgevar var, edgeMethod method, edgeDirection direction)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 0; // for calculating the automatic threshold
            double Depth = 0;

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
            }

            switch (method.ToString())
            {
                case "Sobel":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, direction, "EdgeSobelVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar2.jpg";
                    }
                    break;

                case "Prewitt":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, direction, "EdgePrewittVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar2.jpg";
                    }
                    break;

                case "Roberts":
                    scale = 6;

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, direction, "EdgeRobertsVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar2.jpg";
                    }
                    break;
            }

            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, inEdge inIm, edgevar var, edgeMethod method, edgeDirection direction, double tresh)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 0; // for calculating the automatic threshold
            double Depth = 0;

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
            }

            switch (method.ToString())
            {
                case "Sobel":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, direction, "EdgeSobelVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeSobelVar2.jpg";
                    }
                    break;

                case "Prewitt":
                    scale = 4; // for calculating the automatic threshold

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, direction, "EdgePrewittVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgePrewittVar2.jpg";
                    }
                    break;

                case "Roberts":
                    scale = 6;

                    if (var.ToString() == "var1")
                    {
                        result = edgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, direction, "EdgeRobertsVar1_temp");
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar1.jpg";
                    }
                    else
                    {
                        result = edgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Rand\\EdgeRobertsVar2.jpg";
                    }
                    break;
            }

            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static int[,] edgeHelperv1(double scale, int[,] im, double Tresh, double[,] filter, double[,] filterT, double fdiv, edgeDirection direction, string tempName)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            int[,] b = new int[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh  = 0;

            //Sobel approximation to derivative
            var bx = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(im), ArrOp.ArrayDivByConst(filterT, fdiv), PadType.replicate));
            var by = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(im), ArrOp.ArrayDivByConst(filter, fdiv), PadType.replicate));

            //compute the magnitude
            if (direction.ToString() == "horizontal")
            { b = ArrOp.Uint8Range(ArrOp.PowArrayElements(by, 2)); }
            else if (direction.ToString() == "vertical")
            { b = ArrOp.Uint8Range(ArrOp.PowArrayElements(bx, 2)); }
            else if (direction.ToString() == "both")
            {
                b = ArrOp.Uint8Range(ArrOp.SumArrays(ArrOp.Uint8Range(ArrOp.PowArrayElements(bx, 2)), ArrOp.Uint8Range(ArrOp.PowArrayElements(by, 2))));
            }
            else
            { Console.WriteLine("Wrong direction"); }

            Helpers.WriteImageToFile(b, b, b, tempName);

            if (Tresh == 0)
            {
                cutoff = scale * b.Cast<int>().ToArray().Average();
                tresh = Math.Sqrt(cutoff);
            }
            else
            {
                cutoff = Math.Pow(Tresh, 2);
            }

            for (int i = 0; i < im.GetLength(0); i++)
            {
                for (int j = 0; j < im.GetLength(1); j++)
                {
                    if (b[i, j] > cutoff)
                    {
                        result[i, j] = 255;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }

            return result;
        }

        public static int[,] edgeHelperv2(double scale, int[,] im, double Tresh, double[,] filter, double[,] filterT, double fdiv, edgeDirection direction)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            double[,] b = new double[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh  = 0;

            //Sobel approximation to derivative   
            var bx = Filter.filter_double(ArrOp.ArrayToDouble(im), ArrOp.ArrayDivByConst(filterT, fdiv), PadType.replicate);
            var by = Filter.filter_double(ArrOp.ArrayToDouble(im), ArrOp.ArrayDivByConst(filter, fdiv), PadType.replicate);

            //compute the magnitude
            if (direction.ToString() == "horizontal")
            { b = ArrOp.PowArrayElements(by, 2); }
            else if (direction.ToString() == "vertical")
            { b = ArrOp.PowArrayElements(bx, 2); }
            else if (direction.ToString() == "both")
            {
                b = ArrOp.SumArrays(ArrOp.PowArrayElements(bx, 2), ArrOp.PowArrayElements(by, 2));
            }
            else
            { Console.WriteLine("Wrong direction"); }

            if (Tresh == 0)
            {
                cutoff = scale * b.Cast<double>().ToArray().Average();
                tresh = Math.Sqrt(cutoff);
            }
            else
            {
                cutoff = Math.Pow(Tresh, 2);
            }

            for (int i = 0; i < im.GetLength(0); i++)
            {
                for (int j = 0; j < im.GetLength(1); j++)
                {
                    if (b[i, j] > cutoff)
                    {
                        result[i, j] = 255;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }

            return result;
        }

        //
        public static void Graytresh(Bitmap img, inEdge inIm)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;
            double Depth = 0;

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
            }

            double T = 0.5 * (im.Cast<int>().ToArray().Min() + im.Cast<int>().ToArray().Max());
            bool done = false;
            double Tnext = 0;
           
            List<double> tempTrue = new List<double>();
            List<double> tempFalse = new List<double>();
            while (!done)
            {
                for (int i = 0; i < im.GetLength(0); i++)
                {
                    for (int j = 0; j < im.GetLength(1); j++)
                    {
                        if (im[i, j] >= T)
                        {
                            tempTrue.Add(im[i, j]);
                        }
                        else
                        {
                            tempFalse.Add(im[i, j]);
                        }
                    }
                }

                Tnext = 0.5 * (tempTrue.Average() + tempFalse.Average());

                if (Math.Abs(T - Tnext) < 0.5)
                {
                    done = true;
                }

                T = Tnext;

                tempTrue = new List<double>();
                tempFalse = new List<double>();
            }

            for (int i = 0; i < im.GetLength(0); i++)
            {
                for (int j = 0; j < im.GetLength(1); j++)
                {
                    if (im[i, j] > T)
                    {
                        result[i, j] = 255;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }

            outName = Directory.GetCurrentDirectory() + "\\Rand\\GrayTresh.jpg";
            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }
    }

    public enum lineDirection
    {
        horizontal,
        vertical,
        plus45,
        minus45
    }
    
    public enum inEdge
    {
        rgb,
        BW8b,
        BW24b
    }

    public enum edgevar
    {
        var1,
        var2
    }

    public enum edgeMethod
    {
        Sobel,
        Prewitt,
        Roberts
    }

    public enum edgeDirection
    {
        both,
        horizontal,
        vertical
    }
}
