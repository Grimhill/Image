using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    public static class MorphOperationsCall
    {
        //return bitmap after morph operation
        public static Bitmap MorphOperationBitmap(Bitmap img, MorphOp operation, int[,] structerElement)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            bool type = true;

            //array; where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            var ColorList = Helpers.GetPixels(img);

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8 || Checks.BlackandWhite24bppCheck(ColorList))
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
            { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }

            return image;
        }

        //morph operation result into file
        public static void MorphOperation(Bitmap img, MorphOp operation, int[,] structerElement)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Morph");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            bool type = true;

            //array, where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            //obtain color components. form 8bpp works too; but not recommended to use 8-bit .jpeg\tif\jpg images
            var ColorList = Helpers.GetPixels(img);

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8 || Checks.BlackandWhite24bppCheck(ColorList))
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

            string outName = defPath + imgName + "_" + operation.ToString() + imgExtension;

            #region ka
            //switch (operation)
            //{
            //    case MothOp.Dilate:
            //        if(type)
            //        {
            //            resultR = Dilate.DilateMe(ColorList[0].Color, structerElement);
            //            resultG = Dilate.DilateMe(ColorList[1].Color, structerElement);
            //            resultB = Dilate.DilateMe(ColorList[2].Color, structerElement);
            //        }
            //        else
            //        {
            //            resultR = Dilate.DilateMe(ColorList[0].Color, structerElement);
            //            resultG = resultR; resultB = resultR;
            //        }
            //        outName = defPass + fileName + "_Dilate" + ImgExtension;
            //        break;
            //
            //    case MothOp.Erode:
            //        if (type)
            //        {
            //            resultR = Erode.ErodeMe(ColorList[0].Color, structerElement);
            //            resultG = Erode.ErodeMe(ColorList[1].Color, structerElement);
            //            resultB = Erode.ErodeMe(ColorList[2].Color, structerElement);
            //        }
            //        else
            //        {
            //            resultR = Erode.ErodeMe(ColorList[0].Color, structerElement);
            //            resultG = resultR; resultB = resultR;
            //        }
            //        outName = defPass + fileName + "_Erode" + ImgExtension;
            //        break;
            //
            //    case MothOp.imOpen:
            //        if (type)
            //        {
            //            resultR = Dilate.DilateMe(Erode.ErodeMe(ColorList[0].Color, structerElement), structerElement);
            //            resultG = Dilate.DilateMe(Erode.ErodeMe(ColorList[1].Color, structerElement), structerElement);
            //            resultB = Dilate.DilateMe(Erode.ErodeMe(ColorList[2].Color, structerElement), structerElement);
            //        }
            //        else
            //        {
            //            resultR = Dilate.DilateMe(Erode.ErodeMe(ColorList[0].Color, structerElement), structerElement);
            //            resultG = resultR; resultB = resultR;
            //        }
            //        outName = defPass + fileName + "_imOpen" + ImgExtension;
            //        break;
            //
            //    case MothOp.imClose:
            //        if (type)
            //        {
            //            resultR = Erode.ErodeMe(Dilate.DilateMe(ColorList[0].Color, structerElement), structerElement);
            //            resultG = Erode.ErodeMe(Dilate.DilateMe(ColorList[1].Color, structerElement), structerElement);
            //            resultB = Erode.ErodeMe(Dilate.DilateMe(ColorList[2].Color, structerElement), structerElement);
            //        }
            //        else
            //        {
            //            resultR = Erode.ErodeMe(Dilate.DilateMe(ColorList[0].Color, structerElement), structerElement);
            //            resultG = resultR; resultB = resultR;
            //        }
            //        outName = defPass + fileName + "_imClose" + ImgExtension;
            //        break;
            //}
            #endregion

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            if (Depth == 8)
            { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }

            Helpers.SaveOptions(image, outName, imgExtension);
        }

        //return array after morph operation
        public static int[,] MorphOperationArray(int[,] arr, MorphOp operation, int[,] structerElement)
        {
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];
            result = MorphOperationHelper(arr, operation, structerElement);

            return result;
        }

        //array after some morph operation to file
        public static void MorphOperationArray2File(int[,] arr, MorphOp operation, int[,] structerElement)
        {
            string defPath = GetImageInfo.MyPath("Morph");

            Bitmap image = new Bitmap(arr.GetLength(1), arr.GetLength(0), PixelFormat.Format24bppRgb);
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];            

            result = MorphOperationHelper(arr, operation, structerElement);

            image = Helpers.SetPixels(image, result, result, result);
            string outName = defPath + "_Array2File_" + operation.ToString() + ".png";

            image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image);
            Helpers.SaveOptions(image, outName, ".png");
        }

        //call and process selected morph opeartion at image array(-s)
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
