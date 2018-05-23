using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    /// <summary>
    /// Threshold with bw images
    /// </summary>
    public static class GrayThresh
    {
        //Graythresh by Otsu method?
        //only B&W images
        public static void GlobalThreshold(Bitmap img)
        { 
            ThresholdShapkaProcess(img, img, "_globalThreshold", false);
        }

        public static void AdaptiveThreshold(Bitmap img, int[,] structureElement)
        {
            //even don`t know make check if b&w image and transform before morph operation
            //with 8bpp image seems working fine
            //MorphOperationsCall.MorphOperation(image, MorphOp.imOpen, structureElement);
            Bitmap imOpen = MorphOperationsCall.MorphOperationBitmap(img, MorphOp.imOpen, structureElement);

            ThresholdShapkaProcess(imOpen, img, "_adaptiveThreshold", true);
        }

        private static void ThresholdShapkaProcess(Bitmap img, Bitmap adaptOrig, string method, bool adaptive)
        {
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Segmentation\\Graythresh");

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = GraythreshProcess(img, adaptOrig, adaptive);

            string outName = defPath + imgName + method + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        private static Bitmap GraythreshProcess(Bitmap img, Bitmap adaptOrig, bool adaptive)
        {
            Bitmap image  = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            int[,] result = new int[img.Height, img.Width];
            double Depth  = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (!Checks.BinaryInput(img))
            {
                var im = MoreHelpers.BlackandWhiteProcessHelper(img);
                if (im.GetLength(0) > 1 && im.GetLength(1) > 1)
                {
                    double T = 0.5 * (im.Cast<int>().ToArray().Min() + im.Cast<int>().ToArray().Max());
                    bool done = false;
                    double Tnext = 0;

                    List<double> tempTrue  = new List<double>();
                    List<double> tempFalse = new List<double>();
                    while (!done)
                    {
                        for (int i = 0; i < im.GetLength(0); i++)
                        {
                            for (int j = 0; j < im.GetLength(1); j++)
                            {
                                if (im[i, j] >= T)                                
                                    tempTrue.Add(im[i, j]);                                
                                else                                
                                    tempFalse.Add(im[i, j]);                                
                            }
                        }

                        Tnext = 0.5 * (tempTrue.Average() + tempFalse.Average());

                        if (Math.Abs(T - Tnext) < 0.5) { done = true; }

                        T = Tnext;

                        tempTrue  = new List<double>();
                        tempFalse = new List<double>();
                    }

                    if (adaptive)
                    {
                        im = im.ArraySumWithConst(T);
                        var origCheck = MoreHelpers.BlackandWhiteProcessHelper(adaptOrig);

                        for (int i = 0; i < im.GetLength(0); i++)
                        {
                            for (int j = 0; j < im.GetLength(1); j++)
                            {
                                if (origCheck[i, j] > im[i,j])
                                    result[i, j] = 255;
                                else
                                    result[i, j] = 0;
                            }
                        }                        
                    }
                    else
                    {
                        for (int i = 0; i < im.GetLength(0); i++)
                        {
                            for (int j = 0; j < im.GetLength(1); j++)
                            {
                                if (im[i, j] > T)
                                    result[i, j] = 255;
                                else
                                    result[i, j] = 0;
                            }
                        }
                    }

                    image = Helpers.SetPixels(image, result, result, result);                    

                    if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }                    
                }
            }
            else { Console.WriteLine("What did you expected to make Graythresh with binary image? Return black square."); }

            return image;
        }
    }
}
