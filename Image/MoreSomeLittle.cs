using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

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

                width = imgOrig.Width;
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

                    //dont forget, that directory Contour must exist. Later add if not exist - creat
                    difference.Save(outName);
                    //Helpers.SaveOptions(image, outName, ImgExtension);
                }
            }
        }

        public static void Difference(Bitmap imgOrig, Bitmap imgMod, double coefOne, double coefTwo, double alpha) //, double alpha
        {
            int width = imgOrig.Width;
            int height = imgOrig.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);
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

                //suppose work with uint8
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
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                image.Save(outName);
            }
        }
    }
}
