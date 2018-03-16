using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using Image.ArrayOperations;

namespace Image
{
    public static class FindLines
    {
        //find lines 
        public static void Lines(Bitmap img, LineDirection lineDirection)
        {
            string imgName = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath = GetImageInfo.MyPath("Segmentation\\Lines");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] lineRes = new int[img.Height, img.Width];
            string outName = String.Empty;

            var imArray = MoreHelpers.BlackandWhiteProcessHelper(img);
            if (imArray.GetLength(0) > 1 && imArray.GetLength(1) > 1)
            {
                //choose filter and use it
                switch (lineDirection)
                {
                    case LineDirection.horizontal:
                        double[,] horisontalFilter = { { -1, -1, -1 }, { 2, 2, 2 }, { -1, -1, -1 } };

                        lineRes = FindLineHelper(imArray, horisontalFilter);
                        outName = defPath + imgName + "_HorisontalLine.png";
                        break;

                    case LineDirection.vertical:
                        double[,] verticalFilter = { { -1, 2, -1 }, { -1, 2, -1 }, { -1, 2, -1 } };

                        lineRes = FindLineHelper(imArray, verticalFilter);
                        outName = defPath + imgName + "_VerticalLine.png";
                        break;

                    case LineDirection.plus45:
                        double[,] plus45Filter = { { -1, -1, 2 }, { -1, 2, -1 }, { 2, -1, -1 } };

                        lineRes = FindLineHelper(imArray, plus45Filter);
                        outName = defPath + imgName + "_Plus45Line.png";
                        break;

                    case LineDirection.minus45:
                        double[,] minus45Filter = { { 2, -1, -1 }, { -1, 2, -1 }, { -1, -1, 2 } };

                        lineRes = FindLineHelper(imArray, minus45Filter);
                        outName = defPath + imgName + "_Minus45Line.png";
                        break;
                }

                image = Helpers.SetPixels(image, lineRes, lineRes, lineRes);
                image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image);

                Helpers.SaveOptions(image, outName, ".png");
            }
        }

        private static int[,] FindLineHelper(int[,] im, double[,] filter)
        {
            int[,] result = new int[im.GetLength(0), im.GetLength(1)];

            var temp = (ImageFilter.Filter_double(im.ImageUint8ToDouble(), filter, PadType.replicate)).AbsArrayElements();

            var max = temp.Cast<double>().ToArray().Max();

            for (int i = 0; i < im.GetLength(0); i++)
            {
                for (int j = 0; j < im.GetLength(1); j++)
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
    }

    public enum LineDirection
    {
        horizontal,
        vertical,
        plus45,
        minus45
    }
}
