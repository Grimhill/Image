using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    public static class MorphOperationsCall
    { 
        //return bitmap after morph operation
        public static Bitmap MorphOperationBitmap(Bitmap img, MorphOp operation, int[,] structureElement)
        {
            return MorphOperationProcess(img, operation, structureElement);
        }

        //morph operation result into file
        public static void MorphOperation(Bitmap img, MorphOp operation, int[,] structureElement)
        {          
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Morph");           

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = MorphOperationProcess(img, operation, structureElement);

            string outName = defPath + imgName + "_" + operation.ToString() + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static void MorphOperation(Bitmap img, MorphOp operation, int[,] structureElement, string elementInf)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Morph");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = MorphOperationProcess(img, operation, structureElement);

            string outName = defPath + imgName + "_" + operation.ToString() + elementInf + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        private static Bitmap MorphOperationProcess(Bitmap img, MorphOp operation, int[,] structureElement)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            bool type = true;

            //array; where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];

            var ColorList = Helpers.GetPixels(img);

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8 || Depth == 1 || Checks.BlackandWhite24bppCheck(ColorList)) { type = false; }

            if (type)
            {
                resultR = MorphOperationHelper(ColorList[0].Color, operation, structureElement);
                resultG = MorphOperationHelper(ColorList[1].Color, operation, structureElement);
                resultB = MorphOperationHelper(ColorList[2].Color, operation, structureElement);
            }
            else
            {
                resultR = MorphOperationHelper(ColorList[0].Color, operation, structureElement);
                resultG = resultR; resultB = resultR;
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
            if (Depth == 1) { image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5); }

            return image;
        }


        //return array after morph operation
        public static int [,] MorphOperationArray(int[,] arr, MorphOp operation, int[,] structureElement)
        {
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];
            result = MorphOperationHelper(arr, operation, structureElement);

            return result;
        }

        //array after some morph operation to file
        public static void MorphOperationArray2File(int[,] arr, MorphOp operation, int[,] structureElement)
        {
            string defPath = GetImageInfo.MyPath("Morph");

            Bitmap image = new Bitmap(arr.GetLength(1), arr.GetLength(0), PixelFormat.Format24bppRgb);
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];            

            result = MorphOperationHelper(arr, operation, structureElement);

            image = Helpers.SetPixels(image, result, result, result);
            string outName = defPath + "_Array2File_" + operation.ToString() + ".png";

            image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image);
            Helpers.SaveOptions(image, outName, ".png");
        }

        public static void MorphOperationArray2File(int[,] arr, MorphOp operation, int[,] structureElement, string elementInf)
        {
            string defPath = GetImageInfo.MyPath("Morph");

            Bitmap image = new Bitmap(arr.GetLength(1), arr.GetLength(0), PixelFormat.Format24bppRgb);
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];

            result = MorphOperationHelper(arr, operation, structureElement);

            image = Helpers.SetPixels(image, result, result, result);
            string outName = defPath + "_Array2File_" + operation.ToString() + elementInf + ".png";

            image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image);
            Helpers.SaveOptions(image, outName, ".png");
        }


        //call and process selected morph opeartion at image array(-s)
        private static int[,] MorphOperationHelper(int [,] arr, MorphOp operation, int[,] structureElement)
        {
            int[,] result = new int[arr.GetLength(0), arr.GetLength(1)];

            switch (operation)
            {
                case MorphOp.Dilate:
                    result = Dilate.DilateMe(arr, structureElement);               
                    break;

                case MorphOp.Erode:
                    result = Erode.ErodeMe(arr, structureElement);
                    break;

                case MorphOp.imOpen:
                    result = Dilate.DilateMe(Erode.ErodeMe(arr, structureElement), structureElement);
                    break;

                case MorphOp.imClose:
                    result = Erode.ErodeMe(Dilate.DilateMe(arr, structureElement), structureElement);
                    break;
            }

            return result;
        }


        #region structure element Tuple

        public static (int[,] structerElement, string elementInf) LineStructure(int size, LineStructElementDegree lineType)
        {
            return (StructureElement.Line(size, lineType), "strElemLine" + "_Size_" + size + "_lineType_" + lineType);
        }

        public static (int[,] structerElement, string elementInf) SquareStructure(int size)
        {
            return (StructureElement.Square(size), "strElemSquare" + "_SideSize_" + size);
        }

        public static (int[,] structerElement, string elementInf) RectangleStructure(int height, int width)
        {
            return (StructureElement.Rectangle(height, width), "strElemRectangle" + "_height_" + height + "_width_" + width);
        }

        public static (int[,] structerElement, string elementInf) DiskStructure(int radius)
        {
            return (StructureElement.Disk(radius), "strElemDisk" + "_radius_" + radius);
        }

        public static (int[,] structerElement, string elementInf) DiamondStructure(int dist)
        {
            return (StructureElement.Diamond(dist), "strElemDiamind" + "_dist_" + dist);
        }

        public static (int[,] structerElement, string elementInf) OctagonStructure(int dist)
        {
            return (StructureElement.Octagon(dist), "strElemOctagon" + "_dist_" + dist);
        }        

        #endregion structure element Tuple
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
