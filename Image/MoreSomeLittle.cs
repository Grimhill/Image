﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Image.ArrayOperations;

namespace Image
{
    public static class MoreSomeLittle
    {
        public static void DifferenceLight(Bitmap imgOrig, Bitmap imgMod)
        {
            Bitmap difference;
            int width, height;
            bool identical = true;
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            if (imgOrig.Width != imgMod.Width || imgOrig.Height != imgMod.Height)
            {
                Console.WriteLine("Origin and modified images dimentions dismatch or them different");
            }

            else
            {
                var cListOrigin = Helpers.GetPixels(imgOrig);
                var cListMod = Helpers.GetPixels(imgMod);

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
                    string outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + "_difference.jpg");
                   
                    difference.Save(outName);
                    //Helpers.SaveOptions(image, outName, ImgExtension);
                }
            }
        }

        public static void Difference(Bitmap imgOrig, Bitmap imgMod, double coefOne, double coefTwo, double alpha) //, double alpha
        {
            int width  = imgOrig.Width;
            int height = imgOrig.Height;
            Bitmap image = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;

            if (imgOrig.Width != imgMod.Width || imgOrig.Height != imgMod.Height)
            {
                Console.WriteLine("Origin and modified images dimentions dismatch or them different");
            }
            else if (alpha < 0 || alpha > 1)
            {
                Console.WriteLine("Alpha must be in range [0..1]");
            }
            else
            {
                var cListOrigin = Helpers.GetPixels(imgOrig);
                var cListMod = Helpers.GetPixels(imgMod);

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

                ArrGen<int> d;
                d = new ArrGen<int>();

                var R = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newrVector.ToArray());
                var G = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newgVector.ToArray());
                var B = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newbVector.ToArray());
                //lay difference on alpha image

                image = Helpers.SetPixels(image, R, G, B);
                outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\Diff.jpg");                
                image.Save(outName);
            }
        }

        //n - pixels per cell
        public static void CheckerBoard(int n, OutType type)
        {
            int[,] result = new int[n * 8, n * 8]; //8 -default cells
            result = CheckerBoardHelper(n, 8, 8);

            Helpers.WriteImageToFile(result, result, result, "checkerboard.jpg", type);
        }

        //r & c must be greater than 1
        public static void CheckerBoard(int n, int r, int c, OutType type)//r - rows, c - cols, n - pixels per cell
        {
            int[,] result = new int[n * r, n * c];
            result = CheckerBoardHelper(n, r, c);

            Helpers.WriteImageToFile(result, result, result, "checkerboard_.jpg", type);
        }

        public static int[,] CheckerBoardHelper(int n, int r, int c)//r - rows, c - cols, n - pixels per cell
        {
            int[] wCell = new int[n];
            int[] bCell = new int[n];
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

            return d.VecorToArrayRowByRow(n * r, n * c, resVect); //r,c, vector            
        }


        public static void WildBoard(Bitmap img, int n, WildBoardVariant var, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            var ColorList = Helpers.GetPixels(img);
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            string outName = String.Empty;

            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            int Boardrows = (int)Math.Ceiling((double)img.Height / n);
            int Boardcols = (int)Math.Ceiling((double)img.Width / n);

            var board = CheckerBoardHelper(n, Boardrows, Boardcols);
            var gray = Helpers.RGBToGrayArray(img);

            for (int i = 0; i < img.Height; i++)
            {
                for (int j = 0; j < img.Width; j++)
                {
                    if (var == WildBoardVariant.Variant1)
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
            outName = MoreHelpers.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + "_CheckerBoard" + ImgExtension);
            image.Save(outName);
        }
    }

    public enum WildBoardVariant
    {
        Variant1,
        Variant2
    }
}
