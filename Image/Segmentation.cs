using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

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
            ArrayOperations ArrOp = new ArrayOperations();
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];

            var temp = ArrOp.AbsArrayElements(Filter.Filter_double(ArrOp.ImageUint8ToDouble(arr), filter, PadType.replicate));

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
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, string fileName)
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

            if (var == Edgevar.var1)
            {
                result = EdgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                    EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgeDefaultVar1_temp);
                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefaultVar1" + ImgExtension;
            }
            else
            {
                result = EdgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, EdgeDirection.both);
                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefaultVar2" + ImgExtension;
            }

            image = Helpers.SetPixels(image, result, result, result);
            outName = MoreHelpers.OutputFileNames(outName);
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, double tresh, string fileName)
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

            if (var == Edgevar.var1)
            {
                result = EdgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                    EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgeDefaultVar1_temp);
                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefaultVar1" + ImgExtension;
            }
            else
            {
                result = EdgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, EdgeDirection.both);
                outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeDefaultVar2" + ImgExtension;
            }

            image = Helpers.SetPixels(image, result, result, result);
            outName = MoreHelpers.OutputFileNames(outName);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, string fileName)
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

            switch (method)
            {
                case EdgeMethod.Sobel:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                            EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgeSobelVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, EdgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Prewitt:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8,
                            EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgePrewittVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, EdgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Roberts:
                    scale = 6;

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2,
                            EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgeRobertsVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, EdgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar2" + ImgExtension;
                    }
                    break;
            }

            image = Helpers.SetPixels(image, result, result, result);

            outName = MoreHelpers.OutputFileNames(outName);
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, double tresh, string fileName)
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

            switch (method)
            {
                case EdgeMethod.Sobel:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                            EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgeSobelVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, EdgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Prewitt:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8,
                            EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgePrewittVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, EdgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Roberts:
                    scale = 6;

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2,
                            EdgeDirection.both, fileName, ImgExtension, EdgeTempName._EdgeRobertsVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, EdgeDirection.both);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar2" + ImgExtension;
                    }
                    break;
            }

            image = Helpers.SetPixels(image, result, result, result);

            outName = MoreHelpers.OutputFileNames(outName);
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, EdgeDirection direction, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Segmentation");

            Bitmap image = new Bitmap(img.Width, img.Height, img.PixelFormat);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double scale = 0; // for calculating the automatic threshold            

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.GetPixels(img);

            im = UselessCheck(img, ColorList, inIm, Depth, im);

            switch (method)
            {
                case EdgeMethod.Sobel:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                            direction, fileName, ImgExtension, EdgeTempName._EdgeSobelVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Prewitt:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8,
                            direction, fileName, ImgExtension, EdgeTempName._EdgePrewittVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Roberts:
                    scale = 6;

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2,
                            direction, fileName, ImgExtension, EdgeTempName._EdgeRobertsVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, 0, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar2" + ImgExtension;
                    }
                    break;
            }

            image = Helpers.SetPixels(image, result, result, result);
            outName = MoreHelpers.OutputFileNames(outName);
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        //if absolutelu black image - low the tresh value
        public static void Edge(Bitmap img, InEdgeImage inIm, Edgevar var, EdgeMethod method, EdgeDirection direction, double tresh, string fileName)
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

            switch (method)
            {
                case EdgeMethod.Sobel:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8,
                            direction, fileName, ImgExtension, EdgeTempName._EdgeSobelVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Sobel"), Filter.Dx3FWindow("SobelT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeSobelVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Prewitt:
                    scale = 4; // for calculating the automatic threshold

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8,
                            direction, fileName, ImgExtension, EdgeTempName._EdgePrewittVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Prewitt"), Filter.Dx3FWindow("PrewittT"), 8, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgePrewittVar2" + ImgExtension;
                    }
                    break;

                case EdgeMethod.Roberts:
                    scale = 6;

                    if (var == Edgevar.var1)
                    {
                        result = EdgeHelperv1(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2,
                            direction, fileName, ImgExtension, EdgeTempName._EdgeRobertsVar1_temp);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar1" + ImgExtension;
                    }
                    else
                    {
                        result = EdgeHelperv2(scale, im, tresh, Filter.Dx3FWindow("Roberts"), Filter.Dx3FWindow("RobertsT"), 2, direction);
                        outName = Directory.GetCurrentDirectory() + "\\Segmentation\\" + fileName + "_EdgeRobertsVar2" + ImgExtension;
                    }
                    break;
            }

            image = Helpers.SetPixels(image, result, result, result);

            outName = MoreHelpers.OutputFileNames(outName);
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static int[,] EdgeHelperv1(double scale, int[,] im, double Tresh, double[,] filter,
            double[,] filterT, double fdiv, EdgeDirection direction, string fName, string extension, EdgeTempName tempName)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            int[,] b = new int[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh = 0;

            //Sobel approximation to derivative
          
            var bx = ArrOp.ArrayToUint8(Filter.Filter_double(im, filterT, fdiv));   
            var by = ArrOp.ArrayToUint8(Filter.Filter_double(im, filter, fdiv));

            //compute the magnitude
            if (direction == EdgeDirection.horizontal)
            { b = ArrOp.Uint8Range(ArrOp.PowArrayElements(by, 2)); }
            else if (direction == EdgeDirection.vertical)
            { b = ArrOp.Uint8Range(ArrOp.PowArrayElements(bx, 2)); }
            else if (direction == EdgeDirection.both)
            {
                b = ArrOp.Uint8Range(ArrOp.SumArrays(ArrOp.Uint8Range(ArrOp.PowArrayElements(bx, 2)), ArrOp.Uint8Range(ArrOp.PowArrayElements(by, 2))));
            }
            else
            { Console.WriteLine("Wrong direction"); }

            fName = fName + tempName.ToString() + extension;
            Helpers.WriteImageToFile(b, b, b, fName, "Segmentation");

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

        public static int[,] EdgeHelperv2(double scale, int[,] im, double Tresh, double[,] filter,
            double[,] filterT, double fdiv, EdgeDirection direction)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            double[,] b = new double[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh = 0;

            //Sobel approximation to derivative  
            var bx = Filter.Filter_double(im, filterT, fdiv);            
            var by = Filter.Filter_double(im, filter, fdiv);

            //compute the magnitude
            if (direction == EdgeDirection.horizontal)
            { b = ArrOp.PowArrayElements(by, 2); }
            else if (direction == EdgeDirection.vertical)
            { b = ArrOp.PowArrayElements(bx, 2); }
            else if (direction == EdgeDirection.both)
            {
                b = ArrOp.SumArrays(ArrOp.PowArrayElements(bx, 2),
                    ArrOp.PowArrayElements(by, 2));
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

        public static int[,] UselessCheck(Bitmap img, List<ArraysListInt> ColorList, InEdgeImage inIm, double Depth, int[,] im)
        {
            if (inIm == InEdgeImage.BW8b)
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else { im = ColorList[0].Color; }
            }
            else if (inIm == InEdgeImage.rgb)
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else { im = Helpers.RGBToGrayArray(img); }
            }
            else if (inIm == InEdgeImage.BW24b)
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else { im = ColorList[0].Color; }
            }
            return im;
        }
        //
        public static void Graytresh(Bitmap img, InEdgeImage inIm, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            ArrayOperations ArrOp = new ArrayOperations();
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Segmentation");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.GetPixels(img);

            im = UselessCheck(img, ColorList, inIm, Depth, im);

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
        vertical
    }

    public enum EdgeTempName
    {
        _EdgeDefaultVar1_temp,
        _EdgeSobelVar1_temp,
        _EdgePrewittVar1_temp,
        _EdgeRobertsVar1_temp
    }
}
