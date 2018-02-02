using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Image.Morphology
{
    public static class MorphOperationsCall
    {
        public static Bitmap MorphOperation(Bitmap img, MorphOp operation, int[,] structerElement)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            bool type = true;

            //array, where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            var ColorList = Helpers.GetPixels(img);

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8 || Checks.BlackandWhite24bppCheck(img))
            { type = false; }

            if (type)
            {
                resultR = MorphOperationHelper(ColorList[0].Color, operation, structerElement);
                resultG = MorphOperationHelper(ColorList[1].Color, operation, structerElement);
                resultB = MorphOperationHelper(ColorList[2].Color, operation, structerElement);
            }
            else
            {
                resultR = MorphOperationHelper(ColorList[0].Color, operation, structerElement);
                resultG = resultR; resultB = resultR;
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            if (Depth == 8)
            { image = MoreHelpers.Bbp24Gray2Gray8bppHelper(image); }

            return image;
        }

        public static void MorphOperation2File(Bitmap img, MorphOp operation, int[,] structerElement, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string defPass = Directory.GetCurrentDirectory() + "\\Morph\\";
            Checks.DirectoryExistance(defPass);

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;
            bool type = true;

            //array, where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            //obtain color components. form 8bpp works too, but not recommended to use 8-bit .jpeg\tif\jpg images
            var ColorList = Helpers.GetPixels(img);

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8) { ImgExtension = ".png"; }
            if (Depth == 8 || Checks.BlackandWhite24bppCheck(img))
            { type = false; }

            if (type)
            {
                resultR = MorphOperationHelper(ColorList[0].Color, operation, structerElement);
                resultG = MorphOperationHelper(ColorList[1].Color, operation, structerElement);
                resultB = MorphOperationHelper(ColorList[2].Color, operation, structerElement);
            }
            else
            {
                resultR = MorphOperationHelper(ColorList[0].Color, operation, structerElement);
                resultG = resultR; resultB = resultR;
            }
            outName = defPass + fileName + "_" + operation.ToString() + ImgExtension;
            
            image = Helpers.SetPixels(image, resultR, resultG, resultB);
            outName = Checks.OutputFileNames(outName);

            if (Depth == 8)
            { image = MoreHelpers.Bbp24Gray2Gray8bppHelper(image); }

            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        public static int[,] MorphOperation(int[,] arr, MorphOp operation, int[,] structerElement)
        {
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];
            result = MorphOperationHelper(arr, operation, structerElement);

            return result;
        }

        public static void MorphOperationArray2File(int[,] arr, MorphOp operation, int[,] structerElement)
        {
            string defPass = Directory.GetCurrentDirectory() + "\\Morph\\";
            Checks.DirectoryExistance(defPass);

            Bitmap image = new Bitmap(arr.GetLength(1), arr.GetLength(0), PixelFormat.Format24bppRgb);
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];
            string outName = String.Empty;

            result = MorphOperationHelper(arr, operation, structerElement);

            image = Helpers.SetPixels(image, result, result, result);
            outName = Checks.OutputFileNames(defPass + "Array2File_" + operation.ToString() + ".png");

            image = MoreHelpers.Bbp24Gray2Gray8bppHelper(image);

            Helpers.SaveOptions(image, outName, ".png");
        }

        private static int[,] MorphOperationHelper(int[,] arr, MorphOp operation, int[,] structerElement)
        {
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];

            switch (operation)
            {
                case MorphOp.Dilate:
                    result = Dilate.DilateMe(arr, structerElement);
                    break;

                case MorphOp.Erode:
                    result = Erode.ErodeMe(arr, structerElement);
                    break;

                case MorphOp.imOpen:
                    result = Dilate.DilateMe(Erode.ErodeMe(arr, structerElement), structerElement);
                    break;

                case MorphOp.imClose:
                    result = Erode.ErodeMe(Dilate.DilateMe(arr, structerElement), structerElement);
                    break;
            }

            return result;
        }
    }

    public enum MorphOp
    {
        Dilate,
        Erode,
        imOpen,
        imClose
    }

    public enum StructureElementType
    {
        line,
        square,
        disk,
        rectangle,
        octagon,
        diamond
    }
}
