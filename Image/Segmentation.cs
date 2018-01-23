using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Image.ArrayOperations;

namespace Image
{
    public static class Segmentation
    {
        public static void FindLines(Bitmap img, LineDirection lineDir, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Segmentation");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] lineRes = new int[img.Height, img.Width];
            string outName = String.Empty;

            var im = Helpers.RGBToGrayArray(img);

            switch (lineDir)
            {
                case LineDirection.horizontal:
                    double[,] horisontalFilter = { { -1, -1, -1 }, { 2, 2, 2 }, { -1, -1, -1 } };

                    lineRes = FindLineHelper(im, horisontalFilter);
                    outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_HorisontalLine" + ImgExtension;
                    break;

                case LineDirection.vertical:
                    double[,] verticalFilter = { { -1, 2, -1 }, { -1, 2, -1 }, { -1, 2, -1 } };

                    lineRes = FindLineHelper(im, verticalFilter);
                    outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_VerticalLine" + ImgExtension;
                    break;

                case LineDirection.plus45:
                    double[,] plus45Filter = { { -1, -1, 2 }, { -1, 2, -1 }, { 2, -1, -1 } };

                    lineRes = FindLineHelper(im, plus45Filter);
                    outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_Plus45Line" + ImgExtension;
                    break;

                case LineDirection.minus45:
                    double[,] minus45Filter = { { 2, -1, -1 }, { -1, 2, -1 }, { -1, -1, 2 } };

                    lineRes = FindLineHelper(im, minus45Filter);
                    outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_Minus45Line" + ImgExtension;
                    break;
            }


            image = Helpers.SetPixels(image, lineRes, lineRes, lineRes);

            outName = MoreHelpers.OutputFileNames(outName);
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static int[,] FindLineHelper(int[,] arr, double[,] filter)
        {           
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];

            var temp = (Filter.Filter_double(arr.ImageUint8ToDouble(), filter, PadType.replicate)).AbsArrayElements();

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

        //bad, very bad. worth
        //segment little default
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, string fileName)
        {
            LittleEdgeMethodVariant(img, inIm, var, 0, fileName);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, double threshold, string fileName)
        {
            LittleEdgeMethodVariant(img, inIm, var, threshold, fileName);
        }

        //
        public static void LittleEdgeMethodVariant(Bitmap img, InEdgeImage inIm, Edgevar var, double threshold, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Segmentation");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 4; // for calculating the automatic threshold

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.GetPixels(img);

            im = UselessCheck(img, ColorList, inIm, Depth, im);

            if ((im.GetLength(0) > 1 && im.GetLength(1) > 1) && (threshold >= 0 && threshold <= 1))
            {
                if (var == Edgevar.var1)
                {
                    result = EdgeHelperv1(scale, im, threshold, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                        EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgeDefaultVar1_temp);
                    if (threshold == 0)
                    {
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefV1" + ImgExtension;
                    }
                    else
                    {
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefV1" + "Th_" + threshold.ToString() + ImgExtension;
                    }
                }
                else
                {
                    result = EdgeHelperv2(scale, im, threshold, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, EdgeDirection.both);
                    if (threshold == 0)
                    {
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefV2" + ImgExtension;
                    }
                    else
                    {
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefV2" + "Th_" + threshold.ToString() + ImgExtension;
                    }
                }

                image = Helpers.SetPixels(image, result, result, result);
                outName = MoreHelpers.OutputFileNames(outName);

                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
            else { Console.WriteLine("Also Threshold must be in range [0..1]"); }
        }

        //segment large
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, string fileName)
        {
            LargeEdgeMethodVariant(img, inIm, var, method, EdgeDirection.def, 0, fileName);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, double threshold, string fileName)
        {
            LargeEdgeMethodVariant(img, inIm, var, method, EdgeDirection.def, threshold, fileName);
        }

        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, EdgeDirection direction, string fileName)
        {
            LargeEdgeMethodVariant(img, inIm, var, method, direction, 0, fileName);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, EdgeDirection direction, double threshold, string fileName)
        {
            LargeEdgeMethodVariant(img, inIm, var, method, direction, threshold, fileName);
        }

        //
        public static void LargeEdgeMethodVariant(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, EdgeDirection direction, double threshold, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Segmentation");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 0; // for calculating the automatic threshold            

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.GetPixels(img);

            im = UselessCheck(img, ColorList, inIm, Depth, im);
            if (direction == EdgeDirection.def) direction = EdgeDirection.both;

            if ((im.GetLength(0) > 1 && im.GetLength(1) > 1) && (threshold >= 0 && threshold <= 1))
            {
                switch (method)
                {
                    case EdgeMethod.Sobel:
                        scale = 4; // for calculating the automatic threshold

                        if (var == Edgevar.var1)
                        {
                            result = EdgeHelperv1(scale, im, threshold, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                                direction, fileName, ImgExtension, EdgeTempName._EdgeSobelVar1_temp);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobV1" + ImgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobV1" + direction.ToString() + ImgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobV1" + "Th_" + threshold.ToString() + ImgExtension;

                            else
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName
                                    + "_EdgeSobV1" + "Th_" + threshold.ToString() + direction.ToString() + ImgExtension;
                        }
                        else
                        {
                            result = EdgeHelperv2(scale, im, threshold, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, direction);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobV2" + ImgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobV2" + direction.ToString() + ImgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobV2" + "Th_" + threshold.ToString() + ImgExtension;

                            else
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName
                                    + "_EdgeSobV2" + "Th_" + threshold.ToString() + direction.ToString() + ImgExtension;
                        }
                        break;

                    case EdgeMethod.Prewitt:
                        scale = 4; // for calculating the automatic threshold

                        if (var == Edgevar.var1)
                        {
                            result = EdgeHelperv1(scale, im, threshold, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8,
                                direction, fileName, ImgExtension, EdgeTempName._EdgePrewittVar1_temp);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewV1" + ImgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewV1" + direction.ToString() + ImgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewV1" + "Th_" + threshold.ToString() + ImgExtension;

                            else
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName
                                    + "_EdgePrewV1" + "Th_" + threshold.ToString() + direction.ToString() + ImgExtension;
                        }
                        else
                        {
                            result = EdgeHelperv2(scale, im, threshold, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, direction);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewV2" + ImgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewV2" + direction.ToString() + ImgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewV2" + "Th_" + threshold.ToString() + ImgExtension;

                            else
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName
                                    + "_EdgePrewV2" + "Th_" + threshold.ToString() + direction.ToString() + ImgExtension;
                        }
                        break;

                    case EdgeMethod.Roberts:
                        scale = 6;

                        if (var == Edgevar.var1)
                        {
                            result = EdgeHelperv1(scale, im, threshold, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2,
                                direction, fileName, ImgExtension, EdgeTempName._EdgeRobertsVar1_temp);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobV1" + ImgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobV1" + direction.ToString() + ImgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobV1" + "Th_" + threshold.ToString() + ImgExtension;

                            else
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName
                                    + "_EdgeRobV1" + "Th_" + threshold.ToString() + direction.ToString() + ImgExtension;
                        }
                        else
                        {
                            result = EdgeHelperv2(scale, im, threshold, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, direction);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobV2" + ImgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobV2" + direction.ToString() + ImgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobV2" + "Th_" + threshold.ToString() + ImgExtension;

                            else
                                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName
                                    + "_EdgeRobV2" + "Th_" + threshold.ToString() + direction.ToString() + ImgExtension;
                        }
                        break;
                }

                image = Helpers.SetPixels(image, result, result, result);

                outName = MoreHelpers.OutputFileNames(outName);

                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
            else { Console.WriteLine("Also Threshold must be in range [0..1]"); }
        }

        public static int[,] EdgeHelperv1(double scale, int[,] im, double threshold, double[,] filter,
            double[,] filterT, double fdiv, EdgeDirection direction, string fName, string extension, EdgeTempName tempName)
        {           
            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            int[,] b = new int[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh = 0;

            //Sobel approximation to derivative            
            var bx = (Filter.Filter_double(im, filterT, fdiv)).ArrayToUint8();           
            var by = (Filter.Filter_double(im, filter, fdiv)).ArrayToUint8();

            //compute the magnitude
            if (direction == EdgeDirection.horizontal)
            { b = by.PowArrayElements(2).Uint8Range(); }
            else if (direction == EdgeDirection.vertical)
            { b = bx.PowArrayElements(2).Uint8Range(); } 
            else if (direction == EdgeDirection.both)
            {
                //var d = ArrOp.Uint8Range(ArrOp.SumArrays(ArrOp.Uint8Range(ArrOp.PowArrayElements(bx, 2)), ArrOp.Uint8Range(ArrOp.PowArrayElements(by, 2)))); //old
                var byt = by.PowArrayElements(2).Uint8Range(); //temp res, part of expression

                b = bx.PowArrayElements(2).Uint8Range().SumArrays(byt).Uint8Range();

            }
            else
            { Console.WriteLine("Wrong direction"); }

            fName = fName + tempName.ToString() + extension;
            Helpers.WriteImageToFile(b, b, b, fName, "Segmentation");

            if (threshold == 0)
            {
                cutoff = scale * b.Cast<int>().ToArray().Average();
                tresh = Math.Sqrt(cutoff);
            }
            else
            {
                cutoff = Math.Pow(threshold, 2);
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

        public static int[,] EdgeHelperv2(double scale, int[,] im, double threshold, double[,] filter,
            double[,] filterT, double fdiv, EdgeDirection direction)
        {
            //ArrayOperations ArrOp = new ArrayOperations();
            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            double[,] b = new double[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh = 0;

            //Sobel approximation to derivative   
            //var bx = Filter.Filter_double(ArrOp.ArrayToDouble(im), ArrOp.ArrayDivByConst(filterT, fdiv), PadType.replicate);
            var bx = Filter.Filter_double(im, filterT, fdiv);
            //var by = Filter.Filter_double(ArrOp.ArrayToDouble(im), ArrOp.ArrayDivByConst(filter, fdiv), PadType.replicate);
            var by = Filter.Filter_double(im, filter, fdiv);

            //compute the magnitude
            if (direction == EdgeDirection.horizontal)
            { b = by.PowArrayElements(2); }  //b = ArrOp.PowArrayElements(by, 2);
            else if (direction == EdgeDirection.vertical)
            { b = bx.PowArrayElements(2); } //b = ArrOp.PowArrayElements(bx, 2);
            else if (direction == EdgeDirection.both)
            {
                //b = ArrOp.SumArrays(ArrOp.PowArrayElements(bx, 2), ArrOp.PowArrayElements(by, 2));
                b = bx.PowArrayElements(2).SumArrays(by.PowArrayElements(2));
            }
            else
            { Console.WriteLine("Wrong direction"); }

            if (threshold == 0)
            {
                cutoff = scale * b.Cast<double>().ToArray().Average();
                tresh = Math.Sqrt(cutoff);
            }
            else
            {
                cutoff = Math.Pow(threshold, 2);
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
        public static void Graythresh(Bitmap img, InEdgeImage inIm, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            //ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Segmentation");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.GetPixels(img);

            im = UselessCheck(img, ColorList, inIm, Depth, im);
            if (im.GetLength(0) > 1 && im.GetLength(1) > 1)
            {
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

                image = Helpers.SetPixels(image, result, result, result);

                outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_GrayTresh" + ImgExtension);
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }

        public static int[,] UselessCheck(Bitmap img, List<ArraysListInt> ColorList, InEdgeImage inIm, double Depth, int[,] im)
        {
            int[,] empty = new int[1, 1];
            if (inIm == InEdgeImage.BW8b)
            {
                if (Depth != 8)
                {
                    Console.WriteLine("Wrong input arguments, input image not BW8b");
                    return empty;
                }
                else { im = ColorList[0].Color; }
            }
            else if (inIm == InEdgeImage.rgb)
            {
                if (Depth != 24)
                {
                    Console.WriteLine("Wrong input arguments, input image not rgb");
                    return empty;
                }
                else { im = Helpers.RGBToGrayArray(img); }
            }
            else if (inIm == InEdgeImage.BW24b)
            {
                if (Depth != 24)
                {
                    Console.WriteLine("Wrong input arguments, input image not BW24b");
                    return empty;
                }
                else { im = ColorList[0].Color; }
            }
            return im;
        }
    }

    public enum LineDirection
    {
        horizontal,
        vertical,
        plus45,
        minus45
    }

    public enum InEdgeImage
    {
        rgb,
        BW8b,
        BW24b
    }

    public enum Edgevar
    {
        var1,
        var2
    }

    public enum EdgeMethod
    {
        Sobel,
        Prewitt,
        Roberts
    }

    public enum EdgeDirection
    {
        both,
        horizontal,
        vertical,
        def
    }

    public enum EdgeTempName
    {
        _EdgeDefaultVar1_temp,
        _EdgeSobelVar1_temp,
        _EdgePrewittVar1_temp,
        _EdgeRobertsVar1_temp
    }
}
