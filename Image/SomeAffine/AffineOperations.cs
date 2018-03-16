using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    //Bad realization. in some places look like wrong T transform array values (horizontal and vertical seems reversed)    
    //Also obtain some 'black' points (zero values), named 'artifacts'. Don`t how fix for now.
    public static class AffineOperations
    {
        //for integer multipliers works good, and for multipliers with 5 after decimal point (.5).
        //for another numbers can appear some 'artifacts'
        public static void Extenstion(Bitmap img, double height, double width)
        { 
            double[,] Tform = new double[3, 3] { { height, 0, 0 }, { 0, width, 0 }, { 0, 0, 1 } };

            int r = (int)Math.Ceiling(Tform[0, 0] * img.Height);
            int c = (int)Math.Ceiling(Tform[1, 1] * img.Width);

            string outName = "_AffineExtenstion_W_" + width + "x_H_" + height + "x";

            if (height >= 1 && width >= 1 && height > 0 && width > 0) 
            {
                ShapkaProcess(img, Tform, r, c, outName, true);
            }
            else
            {     
                Console.WriteLine("Value of width and height must be non-negative and greater 1. " +
                    "\nIf dont want change width or height set it`s multiplier to 1" +
                    "\nMethod AffineOperations.Extension().");
            }
        }                

        //
        public static void Compression(Bitmap img, double height, double width)
        {
            double[,] Tform = new double[3, 3] { { height, 0, 0 }, { 0, width, 0 }, { 0, 0, 1 } };

            int r = (int)Math.Ceiling(Tform[0, 0] * img.Height);
            int c = (int)Math.Ceiling(Tform[1, 1] * img.Width);

            string outName = "_AffineCompression_W_" + width + "x_H_" + height + "x";

            if (height > 0 && width > 0 && height <= 1 && width <= 1)
            {
                ShapkaProcess(img, Tform, r, c, outName, false);
            }
            else
            {
                Console.WriteLine("Value of width and height must be in range [0..1]." +
                    "\nIf dont want change width or height set it`s to 1" +
                    "\nMethod AffineOperations.Compression()");
            }
        }

        //
        public static void Transfer(Bitmap img, int horizontalTransfer, int verticalTransfer)
        {     
            double[,] Tform = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { verticalTransfer, horizontalTransfer, 1 } };

            int r = (int)Math.Round(Tform[0, 0] * img.Height + Math.Abs(Tform[2, 0]));
            int c = (int)Math.Round(Tform[1, 1] * img.Width + Math.Abs(Tform[2, 1]));

            string outName = "_AffineTransferring_W_" + horizontalTransfer + "_H_" + verticalTransfer;

            ShapkaProcess(img, Tform, r, c, outName, false);
        }        

        //careful, at first try with little value for shift coefficients
        public static void Shift(Bitmap img, double horizontalShift, double verticalShift)
        { 
            double[,] Tform = new double[3, 3] { { 1, horizontalShift, 0 }, { verticalShift, 1, 0 }, { 0, 0, 1 } };            

            int r = (int)Math.Round(img.Height + Math.Abs(Tform[1, 0] * img.Width), MidpointRounding.AwayFromZero);
            int c = (int)Math.Round(img.Width + Math.Abs(Tform[0, 1] * img.Height), MidpointRounding.AwayFromZero);

            string outName = "_AffineShift_" + "Hori_" + horizontalShift + "_Vert_" + verticalShift;

            ShapkaProcess(img, Tform, r, c, outName, false);
        }

        //artifacts at some angles
        //lazy to fix normal +1 pixel in width or height, when rotate
        public static void Rotate(Bitmap img, int angle)
        {
            //coercion to radians            
            var a = Math.Round(Math.PI / 180 * (-angle), 4);            

            double[,] Tform = new double[3, 3] { 
                { Math.Cos(a),  Math.Sin(a), 0 }, 
                { (-1) * Math.Sin(a), Math.Cos(a), 0 }, 
                { 0, 0, 1 } };         
            
            int r = (int)Math.Round(Math.Abs(Math.Cos(a) * img.Height) + Math.Abs(Math.Sin(a) * img.Width));
            int c = (int)Math.Round(Math.Abs(Math.Cos(a) * img.Width) + Math.Abs(Math.Sin(a) * img.Height));

            string outName = "_AffineRotate_" + "Angle_" + angle;

            ShapkaProcess(img, Tform, r, c, outName, false);
        }       
        
        private static void ShapkaProcess(Bitmap img, double[,] tform, int r, int c, string fileName, bool ext)
        {
            string ImgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Affine");

            Bitmap image = new Bitmap(c, r, PixelFormat.Format24bppRgb);
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            List<ArraysListInt> ColorList = Helpers.GetPixels(img);

            int[,] resultR = new int[r, c];
            int[,] resultG = new int[r, c];
            int[,] resultB = new int[r, c];

            if (Depth == 1 || Depth == 8)
            {
                if(ext)                
                    resultR = ExtenstionCount(ColorList[0].Color, tform, r, c);
                else
                    resultR = AffineCount(ColorList[0].Color, tform, r, c);

                resultG = resultR; resultB = resultR;
            }
            else
            {
                if (ext)
                {
                    resultR = ExtenstionCount(ColorList[0].Color, tform, r, c);
                    resultG = ExtenstionCount(ColorList[1].Color, tform, r, c);
                    resultB = ExtenstionCount(ColorList[2].Color, tform, r, c);
                }
                else
                {
                    resultR = AffineCount(ColorList[0].Color, tform, r, c);
                    resultG = AffineCount(ColorList[1].Color, tform, r, c);
                    resultB = AffineCount(ColorList[2].Color, tform, r, c);
                }
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            if (Depth == 8) { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }
            if (Depth == 1) { image = PixelFormatWorks.ImageTo1BppBitmap(image, 0.5); }

            string outName = defPath + imgName + fileName + ImgExtension;
            Helpers.SaveOptions(image, outName, ImgExtension);
        }

        private static int[,] AffineCount(int[,] arr, double[,] tform, int r, int c)
        {
            int[,] result = new int[r, c];
            int[,] X = new int[arr.GetLength(0), arr.GetLength(1)];
            int[,] Y = new int[arr.GetLength(0), arr.GetLength(1)];

            int[,] X1 = new int[arr.GetLength(0), arr.GetLength(1)];
            int[,] Y1 = new int[arr.GetLength(0), arr.GetLength(1)];

            int[,] X2 = new int[arr.GetLength(0), arr.GetLength(1)];
            int[,] Y2 = new int[arr.GetLength(0), arr.GetLength(1)];

            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    double[,] temp = new double[1, 3] { { i, j, 1 } };
                    var tempRes = temp.MultArrays(tform);

                    var x = (int)Math.Floor(tempRes[0, 0]);
                    var y = (int)Math.Floor(tempRes[0, 1]);

                    X[i, j] = x;
                    Y[i, j] = y;                    
                    
                    var x1 = (int)Math.Ceiling(tempRes[0, 0]);
                    var y1 = (int)Math.Ceiling(tempRes[0, 1]);

                    X1[i, j] = x1;
                    Y1[i, j] = y1;

                    var x2 = (int)Math.Round(tempRes[0, 0]);
                    var y2 = (int)Math.Round(tempRes[0, 1]);

                    X2[i, j] = x2;
                    Y2[i, j] = y2;
                }
            }

            #region check X area
            if (X.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(X.Cast<int>().Min());
                if (X[0, 0] == 0) //kostul
                    X[0, 0] = -1;                
                
                for (int i = 0; i < X.GetLength(0); i++)
                {
                    for (int j = 0; j < X.GetLength(1); j++)
                    {
                        X[i, j] = X[i, j] + min;
                    }
                }
                
            }            

            if (X1.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(X1.Cast<int>().Min());
                for (int i = 0; i < X1.GetLength(0); i++)
                {
                    for (int j = 0; j < X1.GetLength(1); j++)
                    {
                        X1[i, j] = X1[i, j] + min;
                    }
                }
            }

            if (X2.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(X2.Cast<int>().Min());
                for (int i = 0; i < X2.GetLength(0); i++)
                {
                    for (int j = 0; j < X2.GetLength(1); j++)
                    {
                        X2[i, j] = X2[i, j] + min;
                    }
                }
            }
            #endregion


            #region check Y area
            if (Y.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(Y.Cast<int>().Min());
                if (Y[0, 0] == 0) //kostul
                    Y[0, 0] = -1;
                for (int i = 0; i < Y.GetLength(0); i++)
                {
                    for (int j = 0; j < Y.GetLength(1); j++)
                    {
                        Y[i, j] = Y[i, j] + min;
                    }
                }
            }            

            if (Y1.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(Y1.Cast<int>().Min());
                for (int i = 0; i < Y1.GetLength(0); i++)
                {
                    for (int j = 0; j < Y1.GetLength(1); j++)
                    {
                        Y1[i, j] = Y1[i, j] + min;
                    }
                }
            }     

            if (Y2.Cast<int>().Min() < 0)
            {
                var min = Math.Abs(Y2.Cast<int>().Min());
                for (int i = 0; i < Y2.GetLength(0); i++)
                {
                    for (int j = 0; j < Y2.GetLength(1); j++)
                    {
                        Y2[i, j] = Y2[i, j] + min;
                    }
                }
            }    
            #endregion

            //some unknown creature
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {                    
                    //result[X[i, j], Y[i, j]] = arr[i, j];
                    //
                    ////insurance for comression. We can obtain in round and celling values equal new width or height
                    ////here substract 1 for cycle, coz in array starts from 0 index
                    if (X1[i, j] >= r)                    
                        result[X1[i, j] - 1, Y1[i, j]] = arr[i, j];
                    
                    else if (Y1[i, j] >= c)                    
                        result[X1[i, j], Y1[i, j] - 1] = arr[i, j];
                    
                    else                    
                        result[X1[i, j], Y1[i, j]] = arr[i, j];                    
                    
                    if (X2[i, j] >= r)                    
                        result[X2[i, j] - 1, Y2[i, j]] = arr[i, j];
                    
                    else if (Y2[i, j] >= c)                    
                        result[X2[i, j], Y2[i, j] - 1] = arr[i, j];
                    
                    else                    
                        result[X2[i, j], Y2[i, j]] = arr[i, j];                    
                }
            }                        

            return result;
        }

        private static int[,] ExtenstionCount(int[,] arr, double[,] tform, int r, int c)
        {
            int[,] result = new int[r, c];
            double m = 0;

            for (int i = 1; i <= arr.GetLength(0); i++)
            {
                double n = 0;
                for (int j = 1; j <= arr.GetLength(1); j++)
                {
                    double[,] temp = new double[1, 3] { { i, j, 1 } };
                    var tempRes = temp.MultArrays(tform);

                    var x = (int)Math.Round(tempRes[0, 0], MidpointRounding.AwayFromZero);
                    var y = (int)Math.Round(tempRes[0, 1], MidpointRounding.AwayFromZero);

                    var x1 = (int)Math.Floor(tempRes[0, 0]);
                    var y1 = (int)Math.Floor(tempRes[0, 1]);

                    var x2 = (int)Math.Ceiling(tempRes[0, 0]);
                    var y2 = (int)Math.Ceiling(tempRes[0, 1]);

                    result[x - 1, y - 1] = arr[i - 1, j - 1];
                    result[x1 - 1, y1 - 1] = arr[i - 1, j - 1];
                    result[x2 - 1, y2 - 1] = arr[i - 1, j - 1];

                    for (int k = 1; k <= x - i - m; k++)
                    {
                        result[x - k - 1, y - 1] = arr[i - 1, j - 1];
                    }

                    for (int k = 0; k <= x - i - m; k++)
                    {
                        for (int l = 1; l <= y - j - n; l++)
                        {
                            result[x - k - 1, y - l - 1] = arr[i - 1, j - 1];
                        }
                    }


                    for (int k = 1; k <= x1 - i - m; k++)
                    {
                        result[x1 - k - 1, y1 - 1] = arr[i - 1, j - 1];
                    }

                    for (int k = 0; k <= x1 - i - m; k++)
                    {
                        for (int l = 1; l <= y1 - j - n; l++)
                        {
                            result[x1 - k - 1, y1 - l - 1] = arr[i - 1, j - 1];
                        }
                    }


                    for (int k = 1; k <= x2 - i - m; k++)
                    {
                        result[x2 - k - 1, y2 - 1] = arr[i - 1, j - 1];
                    }

                    for (int k = 0; k <= x2 - i - m; k++)
                    {
                        for (int l = 1; l <= y2 - j - n; l++)
                        {
                            result[x2 - k - 1, y2 - l - 1] = arr[i - 1, j - 1];
                        }
                    }

                    n = n + tform[1, 1] - 1;
                }
                m = m + tform[0, 0] - 1;
            }

            return result;
        }
    }
}
