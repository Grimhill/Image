using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Image.ArrayOperations;

namespace Image
{
    public static class FindEdge
    {
        //Find edges in intensity image
        //bad realization

        //segment little
        public static void Edge(Bitmap img, Edgevariant variant)
        {
            LittleEdgeMethodVariant(img, variant, 0);
        }

        //if absolutely black image - low the thresh value
        public static void Edge(Bitmap img, Edgevariant variant, double threshold)
        {
            LittleEdgeMethodVariant(img, variant, threshold);
        }

        //
        private static void LittleEdgeMethodVariant(Bitmap img, Edgevariant variant, double threshold)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Segmentation\\Edge");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            string outName = String.Empty;

            double scale = 4; // for calculating the automatic threshold              

            var imArray = MoreHelpers.BlackandWhiteProcessHelper(img);

            if ((imArray.GetLength(0) > 1 && imArray.GetLength(1) > 1) && (threshold >= 0 && threshold <= 1) && !(Checks.BinaryInput(img) && threshold == 0))
            {
                if (variant == Edgevariant.var1)
                {
                    result = EdgeHelperv1(scale, imArray, threshold, ImageFilter.Dx3FWindow("Sobel"), ImageFilter.Dx3FWindow("SobelT"), 8,
                        EdgeDirection.both, imgName, imgExtension, EdgeTempName._EdgeDefaultVar1_temp);
                    if (threshold == 0)
                        outName = defPath + imgName + "_EdgeDefV1" + imgExtension;

                    else
                        outName = defPath + imgName + "_EdgeDefV1" + "Th_" + threshold.ToString() + imgExtension;
                }
                else
                {
                    result = EdgeHelperv2(scale, imArray, threshold, ImageFilter.Dx3FWindow("Sobel"), ImageFilter.Dx3FWindow("SobelT"), 8, EdgeDirection.both);
                    if (threshold == 0)
                        outName = defPath + imgName + "_EdgeDefV2" + imgExtension;

                    else
                        outName = defPath + imgName + "_EdgeDefV2" + "Th_" + threshold.ToString() + imgExtension;
                }

                image = Helpers.SetPixels(image, result, result, result);                

                if (Depth == 8)
                { PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }

                Helpers.SaveOptions(image, outName, imgExtension);
            }
            else
            {
                Console.WriteLine("Threshold must be in range [0..1]." +
             "\nOr may be Binary image at input and threshold = 0 - can`t process with such condition.");
            }
        }

        //segment large
        public static void Edge(Bitmap img, Edgevariant variant, EdgeMethod method)
        {
            LargeEdgeMethodVariant(img, variant, method, EdgeDirection.def, 0);
        }

        public static void Edge(Bitmap img, Edgevariant variant, EdgeMethod method, double threshold)
        {
            LargeEdgeMethodVariant(img, variant, method, EdgeDirection.def, threshold);
        }

        public static void Edge(Bitmap img, Edgevariant variant, EdgeMethod method, EdgeDirection direction)
        {
            LargeEdgeMethodVariant(img, variant, method, direction, 0);
        }

        public static void Edge(Bitmap img, Edgevariant variant, EdgeMethod method, EdgeDirection direction, double threshold)
        {
            LargeEdgeMethodVariant(img, variant, method, direction, threshold);
        }

        //
        private static void LargeEdgeMethodVariant(Bitmap img, Edgevariant variant, EdgeMethod method, EdgeDirection direction, double threshold)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Segmentation\\Edge");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            double scale = 0; // for calculating the automatic threshold           

            var imArray = MoreHelpers.BlackandWhiteProcessHelper(img);
            if (direction == EdgeDirection.def) direction = EdgeDirection.both;

            if ((imArray.GetLength(0) > 1 && imArray.GetLength(1) > 1) && (threshold >= 0 && threshold <= 1) && !(Checks.BinaryInput(img) && threshold == 0))
            {
                switch (method)
                {
                    case EdgeMethod.Sobel:
                        scale = 4; // for calculating the automatic threshold

                        if (variant == Edgevariant.var1)
                        {
                            result = EdgeHelperv1(scale, imArray, threshold, ImageFilter.Dx3FWindow("Sobel"), ImageFilter.Dx3FWindow("SobelT"), 8,
                                direction, imgName, imgExtension, EdgeTempName._EdgeSobelVar1_temp);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeSobV1" + imgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeSobV1_" + direction.ToString() + imgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeSobV1" + "Th_" + threshold.ToString() + imgExtension;

                            else
                                outName = defPath + imgName + "_EdgeSobV1" + "Th_" + threshold.ToString() + "_" + direction.ToString() + imgExtension;
                        }
                        else
                        {
                            result = EdgeHelperv2(scale, imArray, threshold, ImageFilter.Dx3FWindow("Sobel"), ImageFilter.Dx3FWindow("SobelT"), 8, direction);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeSobV2" + imgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeSobV2_" + direction.ToString() + imgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeSobV2" + "Th_" + threshold.ToString() + imgExtension;

                            else
                                outName = defPath + imgName + "_EdgeSobV2" + "Th_" + threshold.ToString() + "_" + direction.ToString() + imgExtension;
                        }
                        break;

                    case EdgeMethod.Prewitt:
                        scale = 4; // for calculating the automatic threshold

                        if (variant == Edgevariant.var1)
                        {
                            result = EdgeHelperv1(scale, imArray, threshold, ImageFilter.Dx3FWindow("Prewitt"), ImageFilter.Dx3FWindow("PrewittT"), 8,
                                direction, imgName, imgExtension, EdgeTempName._EdgePrewittVar1_temp);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgePrewV1" + imgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = defPath + imgName + "_EdgePrewV1_" + direction.ToString() + imgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgePrewV1" + "Th_" + threshold.ToString() + imgExtension;

                            else
                                outName = defPath + imgName + "_EdgePrewV1" + "Th_" + threshold.ToString() + "_" + direction.ToString() + imgExtension;
                        }
                        else
                        {
                            result = EdgeHelperv2(scale, imArray, threshold, ImageFilter.Dx3FWindow("Prewitt"), ImageFilter.Dx3FWindow("PrewittT"), 8, direction);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgePrewV2" + imgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = defPath + imgName + "_EdgePrewV2_" + direction.ToString() + imgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgePrewV2" + "Th_" + threshold.ToString() + imgExtension;

                            else
                                outName = defPath + imgName + "_EdgePrewV2" + "Th_" + threshold.ToString() + "_" + direction.ToString() + imgExtension;
                        }
                        break;

                    case EdgeMethod.Roberts:
                        scale = 6;

                        if (variant == Edgevariant.var1)
                        {
                            result = EdgeHelperv1(scale, imArray, threshold, ImageFilter.Dx3FWindow("Roberts"), ImageFilter.Dx3FWindow("RobertsT"), 2,
                                direction, imgName, imgExtension, EdgeTempName._EdgeRobertsVar1_temp);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeRobV1" + imgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeRobV1_" + direction.ToString() + imgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeRobV1" + "Th_" + threshold.ToString() + imgExtension;

                            else
                                outName = defPath + imgName + "_EdgeRobV1" + "Th_" + threshold.ToString() + "_" + direction.ToString() + imgExtension;
                        }
                        else
                        {
                            result = EdgeHelperv2(scale, imArray, threshold, ImageFilter.Dx3FWindow("Roberts"), ImageFilter.Dx3FWindow("RobertsT"), 2, direction);
                            if (threshold == 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeRobV2" + imgExtension;

                            else if (threshold == 0 && direction != EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeRobV2_" + direction.ToString() + imgExtension;

                            else if (threshold != 0 && direction == EdgeDirection.def)
                                outName = defPath + imgName + "_EdgeRobV2" + "Th_" + threshold.ToString() + imgExtension;

                            else
                                outName = defPath + imgName + "_EdgeRobV2" + "Th_" + threshold.ToString() + "_" + direction.ToString() + imgExtension;
                        }
                        break;
                }

                image = Helpers.SetPixels(image, result, result, result);                

                if (Depth == 8)
                { PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }

                Helpers.SaveOptions(image, outName, imgExtension);
            }
            else
            {
                Console.WriteLine("Also Threshold must be in range [0..1]." +
             "\nOr may be Binary image at input and threshold = 0 - can`t process with such condition.");
            }
        }

        private static int[,] EdgeHelperv1(double scale, int[,] im, double threshold, double[,] filter,
            double[,] filterT, double fdiv, EdgeDirection direction, string fName, string extension, EdgeTempName tempName)
        {
            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            int[,] b = new int[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh = 0;

            //Sobel approximation to derivative           
            var bx = (ImageFilter.Filter_double(im, filterT, fdiv)).ArrayToUint8();
            var by = (ImageFilter.Filter_double(im, filter, fdiv)).ArrayToUint8();

            //compute the magnitude
            if (direction == EdgeDirection.horizontal)
                b = by.PowArrayElements(2).Uint8Range();
            else if (direction == EdgeDirection.vertical)
                b = bx.PowArrayElements(2).Uint8Range();
            else if (direction == EdgeDirection.both || direction == EdgeDirection.def)
            {
                var bt = by.PowArrayElements(2).Uint8Range(); //temp res, part of expression
                b = bx.PowArrayElements(2).Uint8Range().SumArrays(bt).Uint8Range();
            }
            else
            {
                Console.WriteLine("Wrong direction");
            }

            fName = fName + tempName.ToString() + extension;
            MoreHelpers.WriteImageToFile(b, b, b, fName, "Segmentation");

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

        private static int[,] EdgeHelperv2(double scale, int[,] im, double threshold, double[,] filter,
            double[,] filterT, double fdiv, EdgeDirection direction)
        {
            int[,] result = new int[im.GetLength(0), im.GetLength(1)];
            double[,] b = new double[im.GetLength(0), im.GetLength(1)];

            double cutoff = 0;
            double tresh = 0;

            //Sobel approximation to derivative             
            var bx = ImageFilter.Filter_double(im, filterT, fdiv);
            var by = ImageFilter.Filter_double(im, filter, fdiv);

            //compute the magnitude
            if (direction == EdgeDirection.horizontal)
                b = by.PowArrayElements(2);
            else if (direction == EdgeDirection.vertical)
                b = bx.PowArrayElements(2);
            else if (direction == EdgeDirection.both || direction == EdgeDirection.def)
                b = bx.PowArrayElements(2).SumArrays(by.PowArrayElements(2));

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
    }

    public enum Edgevariant
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
        def //zatu4ka
    }

    public enum EdgeTempName
    {
        _EdgeDefaultVar1_temp,
        _EdgeSobelVar1_temp,
        _EdgePrewittVar1_temp,
        _EdgeRobertsVar1_temp
    }
}
