using System.Drawing;
using Image.ArrayOperations;
using System.Drawing.Imaging;

namespace Image
{
    
    public static class BWhitmiss
    {
        // The BWHitMiss ("success / failure") function can help to recognize certain pixel configurations        
        public static void HitMiss(Bitmap img, int[,] FirstStructureElement, int[,] SecondStructureElement, OutType type)
        {
            HitMissShapkaProcess(img, FirstStructureElement, SecondStructureElement, type);
        }

        // default
        public static void HitMiss(Bitmap img, OutType type)
        {
            int[,] FirstStructureElement  = new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
            int[,] SecondStructureElement = new int[3, 3] { { 1, 0, 1 }, { 0, 0, 0 }, { 1, 0, 1 } };

            HitMissShapkaProcess(img, FirstStructureElement, SecondStructureElement, type);
        }

        // return bitmap
        public static Bitmap HitMissBitmap(Bitmap img, int[,] FirstStructureElement, int[,] SecondStructureElement, OutType type)
        {
            return HitMissBitmapHelper(img, FirstStructureElement, SecondStructureElement);
        }

        // return bitmap
        public static Bitmap HitMissBitmap(Bitmap img, OutType type)
        {
            int[,] FirstStructureElement  = new int[3, 3] { { 0, 1, 0 }, { 1, 1, 1 }, { 0, 1, 0 } };
            int[,] SecondStructureElement = new int[3, 3] { { 1, 0, 1 }, { 0, 0, 0 }, { 1, 0, 1 } };
            return HitMissBitmapHelper(img, FirstStructureElement, SecondStructureElement);
        }


        private static void HitMissShapkaProcess(Bitmap img, int[,] FirstStructureElement, int[,] SecondStructureElement, OutType type)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Morph\\BWHitMiss");

            int[,] result = BWHitMissProcess(img, FirstStructureElement, SecondStructureElement);
            string outName = defPath + imgName + "_BWHitMiss" + imgExtension;

            MoreHelpers.WriteImageToFile(result, result, result, outName, type);
        }

        private static Bitmap HitMissBitmapHelper(Bitmap img, int[,] FirstStructureElement, int[,] SecondStructureElement)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] count = BWHitMissProcess(img, FirstStructureElement, SecondStructureElement);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            image = Helpers.SetPixels(image, count, count, count);

            if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
            if (Depth == 1) { image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5); }
            return image;
        }

        private static int[,] BWHitMissProcess(Bitmap img, int[,] FirstStructureElement, int[,] SecondStructureElement)
        {
            int[,] temp   = new int[img.Height, img.Width];
            int[,] result = new int[img.Height, img.Width];

            //Bitmap read binary images as 0/255
            int[,] original = Helpers.Image2BinaryArray(img);
            int[,] inverted = MoreHelpers.InvertBinaryArray(original);

            temp   = MorphOperationsCall.MorphOperationArray(original, MorphOp.Erode, FirstStructureElement);
            result = MorphOperationsCall.MorphOperationArray(inverted, MorphOp.Erode, SecondStructureElement);
            result = result.ArrayMultElements(temp);

            //make result saveble
            for (int i = 0; i < result.GetLength(0); i++)
            {
                for (int j = 0; j < result.GetLength(1); j++)
                {
                    if (result[i, j] == 1)
                        result[i, j] = 255;
                }
            }

            return result;
        }
    }
}
