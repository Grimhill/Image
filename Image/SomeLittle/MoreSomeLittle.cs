using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

namespace Image
{
    //More some operations with image
    public static class MoreSomeLittle
    {
        #region Difference
        //search difference between origianl and moded image
        //difference writed to file
        public static void DifferenceLight(Bitmap imgOrig, Bitmap imgMod)
        {            
            string defPath = GetImageInfo.MyPath("Rand");

            Bitmap difference;
            int width, height;
            bool identical = true;

            double DepthOrig = System.Drawing.Image.GetPixelFormatSize(imgOrig.PixelFormat);
            double DepthMod  = System.Drawing.Image.GetPixelFormatSize(imgMod.PixelFormat);            

            if (imgOrig.Width != imgMod.Width || imgOrig.Height != imgMod.Height)
            {
                Console.WriteLine("Origin and modified images dimentions dismatch or them different");
            }
            else if(DepthOrig != DepthMod)
            {
                Console.WriteLine("Can`t obtain difference between 8bit and 24bit iamge");
            }
            else
            {
                List<ArraysListInt> cListOrigin = Helpers.GetPixels(imgOrig);
                List<ArraysListInt> cListMod    = Helpers.GetPixels(imgMod);

                width  = imgOrig.Width;
                height = imgOrig.Height;
                difference = new Bitmap(width, height, PixelFormat.Format24bppRgb);               

                var Rdif = (cListOrigin[0].Color).SubArrays(cListMod[0].Color).AbsArrayElements().ArraySubWithConst(40).Uint8Range();
                var Gdif = (cListOrigin[1].Color).SubArrays(cListMod[1].Color).AbsArrayElements().ArraySubWithConst(40).Uint8Range();
                var Bdif = (cListOrigin[2].Color).SubArrays(cListMod[2].Color).AbsArrayElements().ArraySubWithConst(40).Uint8Range();

                Rdif = Rdif.ArrayMultByConst(20).Uint8Range();
                Gdif = Gdif.ArrayMultByConst(20).Uint8Range();
                Bdif = Bdif.ArrayMultByConst(20).Uint8Range();

                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        if (Rdif[y, x] == 0)
                            difference.SetPixel(x, y, Color.White);
                        else
                        {
                            difference.SetPixel(x, y, Color.FromArgb(Rdif[y, x], Gdif[y, x], Bdif[y, x]));
                            identical = false;
                        }
                    }
                }

                if (identical)
                {
                    Console.WriteLine("Images are identical");
                }
                else
                {
                    string outName = defPath + "_differenceLight.png";

                    if (DepthOrig == 8)
                    { difference = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(difference); }                      
                   
                    Helpers.SaveOptions(difference, outName, ".png");
                }
            }         
        }
        
        //difference shown at original image by using alpha channel
        public static void Difference(Bitmap imgOrig, Bitmap imgMod, double coefOne, double coefTwo, double alpha)
        {           
            string defPath = GetImageInfo.MyPath("Rand");

            int width  = imgOrig.Width;
            int height = imgOrig.Height;
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);            

            double DepthOrig = System.Drawing.Image.GetPixelFormatSize(imgOrig.PixelFormat);
            double DepthMod  = System.Drawing.Image.GetPixelFormatSize(imgMod.PixelFormat);

            if (imgOrig.Width != imgMod.Width || imgOrig.Height != imgMod.Height)
            {
                Console.WriteLine("Origin and modified images dimentions dismatch or them different");
            }
            else if(alpha < 0 || alpha > 1)
            {
                Console.WriteLine("Alpha must be in range [0..1]");
            }
            else if (DepthOrig != DepthMod)
            {
                Console.WriteLine("Can`t obtain difference between 8bit and 24bit iamge");
            }
            else
            {
                List<ArraysListInt> cListOrigin = Helpers.GetPixels(imgOrig);
                List<ArraysListInt> cListMod    = Helpers.GetPixels(imgMod);

                //get difference             
                var Rdif = (cListOrigin[0].Color).SubArrays(cListMod[0].Color).AbsArrayElements().ArraySubWithConst(coefOne).Uint8Range();
                var Gdif = (cListOrigin[1].Color).SubArrays(cListMod[1].Color).AbsArrayElements().ArraySubWithConst(coefOne).Uint8Range();
                var Bdif = (cListOrigin[2].Color).SubArrays(cListMod[2].Color).AbsArrayElements().ArraySubWithConst(coefOne).Uint8Range();

                Rdif = Rdif.ArrayMultByConst(coefTwo).Uint8Range();
                Gdif = Gdif.ArrayMultByConst(coefTwo).Uint8Range();
                Bdif = Bdif.ArrayMultByConst(coefTwo).Uint8Range();

                var fakeR = (cListOrigin[0].Color).ArrayToDouble().ArrayMultByConst(alpha).ArrayToUint8();
                var fakeG = (cListOrigin[1].Color).ArrayToDouble().ArrayMultByConst(alpha).ArrayToUint8();
                var fakeB = (cListOrigin[2].Color).ArrayToDouble().ArrayMultByConst(alpha).ArrayToUint8();

                //obtain indexes with difference
                List<int> rIndexes = new List<int>();

                var rVector = Rdif.Cast<int>().ToArray();
                for (int i = 0; i < rVector.Length; i++)
                {
                    if (rVector[i] > 0)
                    {
                        rIndexes.Add(i);
                    }
                }

                var newrVector = fakeR.Cast<int>().ToList();
                for (int i = 0; i < rVector.Length; i++)
                {
                    for (int j = 0; j < rIndexes.Count(); j++)
                    {
                        if (i == rIndexes[j])
                        {
                            newrVector[i] = rVector[i];
                        }
                    }
                }

                //*****************

                List<int> gIndexes = new List<int>();

                var gVector = Gdif.Cast<int>().ToArray();
                for (int i = 0; i < gVector.Length; i++)
                {
                    if (gVector[i] > 0)
                    {
                        gIndexes.Add(i);
                    }
                }

                var newgVector = fakeG.Cast<int>().ToList();
                for (int i = 0; i < gVector.Length; i++)
                {
                    for (int j = 0; j < gIndexes.Count(); j++)
                    {
                        if (i == gIndexes[j])
                        {
                            newgVector[i] = gVector[i];
                        }
                    }
                }

                //*****************

                List<int> bIndexes = new List<int>();

                var bVector = Bdif.Cast<int>().ToArray();
                for (int i = 0; i < bVector.Length; i++)
                {
                    if (bVector[i] > 0)
                    {
                        bIndexes.Add(i);
                    }
                }

                var newbVector = fakeB.Cast<int>().ToList();
                for (int i = 0; i < bVector.Length; i++)
                {
                    for (int j = 0; j < bIndexes.Count(); j++)
                    {
                        if (i == bIndexes[j])
                        {
                            newbVector[i] = bVector[i];
                        }
                    }
                }

                ArrGen<int> d = new ArrGen<int>();

                var R = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newrVector.ToArray());
                var G = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newgVector.ToArray());
                var B = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newbVector.ToArray());
                //lay difference on alpha image

                image = Helpers.SetPixels(image, R, G, B);
                string outName = defPath + "Difference.png";

                if (DepthOrig == 8)
                { image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image); }

                image.Save(outName);
            }
        }
        #endregion

        #region CheckerBoard
        //create CheckerBoard

        //n - pixels per cell
        public static void CheckerBoard(int n, OutType type) 
        {
            string defPath = GetImageInfo.MyPath("Rand");
            int[,] result = new int[n * 8, n * 8]; //8 -default cells
            result = CheckerBoardHelper(n, 8, 8);

            string outName = defPath + "checkerboardClassik_" + "pixelsPerCell_" + n + ".png";      

            MoreHelpers.WriteImageToFile(result, result, result, outName, type);               
        }
       
        //rows, cols, n - pixels per cell
        public static void CheckerBoard(int n, int rows, int cols, OutType type)
        {
            string defPath = GetImageInfo.MyPath("Rand");
            int[,] result = new int[n * rows, n * cols];

            if (rows > 0 && cols > 0)
            {
                result = CheckerBoardHelper(n, rows, cols);

                string outName = defPath + "checkerboard_" + "rows_" + rows + "_cols_" + cols + "_pixelsPerCell_" + n + ".png";

                MoreHelpers.WriteImageToFile(result, result, result, outName, type);
            }
            else
            { Console.WriteLine("r and c must be positive values greater than 1. Method: CheckerBoard"); }
        }

        //create CheckerBoard and return as bitmap object
        public static Bitmap CheckerBoardBitmap(int n, int rows, int cols, OutType type)
        {            
            int[,] result = new int[n * rows, n * cols];
            Bitmap image = new Bitmap(result.GetLength(1), result.GetLength(0), PixelFormat.Format24bppRgb);

            if (rows > 0 && cols > 0)
            {
                result = CheckerBoardHelper(n, rows, cols);

                image = Helpers.SetPixels(image, result, result, result);

                if (type == OutType.OneBpp)
                {
                    image = PixelFormatWorks.ImageTo1BppBitmap(image);
                }
                else if (type == OutType.EightBpp)
                {
                    image = PixelFormatWorks.Bpp24Gray2Gray8bppBitMap(image);                    
                }
            }
            else
            { Console.WriteLine("r and c must be positive values greater than 1. Method: CheckerBoard"); }

            return image;
        }

        //create CheckerBoard process
        private static int[,] CheckerBoardHelper(int n, int r, int c)
        {
            int[] wCell   = new int[n];
            int[] bCell   = new int[n];
            int[,] result = new int[n * r, n * c];

            for (int i = 0; i < n; i++)
                wCell[i] = 255;

            ArrGen<int> d = new ArrGen<int>();

            int[] temp1 = new int[n * c];
            int[] temp2 = new int[n * c];
            var resVect = result.Cast<int>().ToArray();

            for (int i = 0; i < temp1.Length / n; i++)
                if (i % 2 == 0)
                    bCell.CopyTo(temp1, bCell.Length * i);
                else wCell.CopyTo(temp1, wCell.Length * i);

            for (int i = 0; i < temp2.Length / n; i++)
                if (i % 2 == 0)
                    wCell.CopyTo(temp2, wCell.Length * i);
                else bCell.CopyTo(temp2, bCell.Length * i);

            int count = 0;
            int countn = 0;
            if (r % 2 == 0)
            {
                for (int i = 0; i < r / 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        temp1.CopyTo(resVect, temp1.Length * (i + countn));
                        countn++;
                    }

                    //count++;                 

                    for (int j = 0; j < n; j++)
                    {
                        temp2.CopyTo(resVect, temp2.Length * (i + countn));
                        countn++;
                    }

                    countn--;
                }
            }

            else
            {
                for (int i = 0; i < (r - 1) / 2; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        temp1.CopyTo(resVect, temp1.Length * (i + count));
                        count++;
                    }

                    //count++;                 

                    for (int j = 0; j < n; j++)
                    {
                        temp2.CopyTo(resVect, temp2.Length * (i + count));
                        count++;
                    }

                    count--;
                }
                int baka = n;
                for (int j = 0; j < n; j++)
                {
                    temp1.CopyTo(resVect, resVect.Length - (temp2.Length * baka));
                    baka--;
                }
            }

            return d.VecorToArrayRowByRow(n * r, n * c, resVect); //r;c; vector            
        }

        #endregion

        #region WildBoard
        //creater CheckerBoard on RGB image alternating gray and color cells 

        //n - pixels per cell
        public static void WildBoard(Bitmap img, int n, WildBoardVariant variant)
        {  
            string imgExtension = GetImageInfo.Imginfo(Imageinfo.Extension);
            string imgName      = GetImageInfo.Imginfo(Imageinfo.FileName);
            string defPath      = GetImageInfo.MyPath("Rand");
            
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            image = WildBoardHelepr(img, n, variant);

            string outName = defPath + imgName + "_CheckerBoard" + imgExtension;
            Helpers.SaveOptions(image, outName, imgExtension);
        }

        public static Bitmap WildBoardBitmap(Bitmap img, int n, WildBoardVariant variant)
        {    
            return WildBoardHelepr(img, n, variant);
        }

        private static Bitmap WildBoardHelepr(Bitmap img, int n, WildBoardVariant variant)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            if (Checks.RGBinput(img))
            {
                var ColorList = Helpers.GetPixels(img);
                int[,] resultR = new int[img.Height, img.Width];
                int[,] resultG = new int[img.Height, img.Width];
                int[,] resultB = new int[img.Height, img.Width];

                int Boardrows = (int)Math.Ceiling((double)img.Height / n);
                int Boardcols = (int)Math.Ceiling((double)img.Width / n);

                var board = CheckerBoardHelper(n, Boardrows, Boardcols);
                var gray = Helpers.RGBToGrayArray(img);

                for (int i = 0; i < img.Height; i++)
                {
                    for (int j = 0; j < img.Width; j++)
                    {
                        if (variant == WildBoardVariant.Variant1)
                        {
                            if (board[i, j] == 0)
                            {
                                resultR[i, j] = ColorList[0].Color[i, j];
                                resultG[i, j] = ColorList[1].Color[i, j];
                                resultB[i, j] = ColorList[2].Color[i, j];
                            }
                            else
                            {
                                resultR[i, j] = gray[i, j];
                                resultG[i, j] = gray[i, j];
                                resultB[i, j] = gray[i, j];
                            }
                        }
                        else
                        {
                            if (board[i, j] == 255)
                            {
                                resultR[i, j] = ColorList[0].Color[i, j];
                                resultG[i, j] = ColorList[1].Color[i, j];
                                resultB[i, j] = ColorList[2].Color[i, j];
                            }
                            else
                            {
                                resultR[i, j] = gray[i, j];
                                resultG[i, j] = gray[i, j];
                                resultB[i, j] = gray[i, j];
                            }
                        }
                    }
                }

                image = Helpers.SetPixels(image, resultR, resultG, resultB);
            }

            return image;
        }
        #endregion
    }

    public enum WildBoardVariant
    {
        Variant1,
        Variant2
    }

    public enum OutType
    {
        OneBpp,
        EightBpp,
        TwentyFourBpp
    }
}
