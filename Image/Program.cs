using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Image
{    
    class Program
    {              
        static void Main(string[] args)
        { 
            //recommended 24bbp format image (8 bit per color)
            //!!!!!
            //Все входные не 8 разрядные (uint8), а выше (uint16) првести к uint8. Binary & 8b - some troubles 

            string ImageFilePath = "1.jpeg";
            //string FilePath = "2.jpeg"; //for difference test
                 
            //ArrayOperations ArrOp = new ArrayOperations();

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(ImageFilePath);
            //System.Drawing.Bitmap img   = new System.Drawing.Bitmap(FilePath); //for difference
            
            int[,] Rc = new int[image.Height, image.Width];
            int[,] Gc = new int[image.Height, image.Width];
            int[,] Bc = new int[image.Height, image.Width];

            //read row by row image R\G\B pixels value
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    Color pixelColor = image.GetPixel(j, i);
                    Rc[i, j] = pixelColor.R;
                    Gc[i, j] = pixelColor.G;
                    Bc[i, j] = pixelColor.B;
                }
            }

            //Segmentation.Edge(image, inEdge.BW24b, edgevar.var1, edgeMethod.Sobel, edgeDirection.horizontal);

            #region this
            //System.Drawing.Bitmap imga = new System.Drawing.Bitmap(image.Width, image.Height);
            //int Depth = System.Drawing.Image.GetPixelFormatSize(imga.PixelFormat);
            //imga = Helpers.setPixelsAlpha(imga, Rc, Gc, Bc, 0.4);           
            //imga.Save("fuck.jpg");
            #endregion this                      
                        
            //Graytresh(image, inEdge.BW24b);
            //Edge(image, inEdge.BW24b, edgevar.var2);

            #region
            //Segmentation.findLines(image, lineDirection.vertical);

            //difference(image, img, 40, 20);

            //histeq.hist(image, HisteqColorSpace.RGB);

            //ColorSpaceToFile.colorSpaceToFile(image, ColorSpaceType.rgb2hsv);

            //spFilt.spfilt(image, 2, 2, -1.5, SpfiltType.chmean, false); //Q>0 for pepper & <0 for salt
            //Sharp.unSharp(image, unSharpInColorSpace.fakeCIE1976L, FilterType.unsharp); 
            //Smoothing.Smooth(image, SmoothFilterWindow.window2,SmoothInColorSpace.HSV);
            //Contrast.ContrastBW(image, 0.3, 0.7, 1, 0.8);
            //Contrast.ContrastRGB(image, 0.2, 0.6, 0.3, 0.7, 0, 1);
            //SomeLittle.GammaCorrectionFun(image, 40, 0.3);
            //SomeLittle.MakeNegativeAndBack(image);
            //Contour.GlobalContour(image, CountourVariant.Variant6_RGB);
            #endregion

            Console.ReadLine();        
        }
        
        //random in process
        public static void EnterFilter(Bitmap img, double[,] filter)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width  = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;   

            var ColorList = Helpers.getPixels(img);

            var cPlaneOne   = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[0].c), filter, PadType.replicate));
            var cPlaneTwo   = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[1].c), filter, PadType.replicate));
            var cPlaneThree = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[2].c), filter, PadType.replicate));

            image = Helpers.setPixels(image, cPlaneOne, cPlaneTwo, cPlaneThree);
            outName = Directory.GetCurrentDirectory() + "\\Rand\\ImageEnterFilter.jpg";
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static void RandomFilter(Bitmap img, double min, double max)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width  = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;

            double[,] filter = new double[3, 3];
            Random randNum = new Random();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    //filter[i, j] = randNum.NextDouble() * (max - min) + min;
                    filter[i, j] = randNum.NextDouble();
                }
            }

            var ColorList = Helpers.getPixels(img);

            var cPlaneOne   = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[0].c), filter, PadType.replicate));
            var cPlaneTwo   = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[1].c), filter, PadType.replicate));
            var cPlaneThree = ArrOp.ArrayToUint8(Filter.filter_double(ArrOp.ArrayToDouble(ColorList[2].c), filter, PadType.replicate));

            image = Helpers.setPixels(image, cPlaneOne, cPlaneTwo, cPlaneThree);
            outName = Directory.GetCurrentDirectory() + "\\Rand\\ImageRandomFilter.jpg";
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //mathod with def coeficients and alpha
        //or i am dumb, or sime problems with alpha chennel in bitmap library
        public static void difference(Bitmap imgOrig, Bitmap imgMod, double coefOne, double coefTwo) //, double alpha
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width  = imgOrig.Width;
            int height = imgOrig.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;

            if (imgOrig.Width != imgMod.Width || imgOrig.Height != imgMod.Height)
            {
                Console.WriteLine("Image origin and image modifide dimentions dismatch");
            }
            else
            {
                var cListOrigin = Helpers.getPixels(imgOrig);
                var cListMod    = Helpers.getPixels(imgMod);
                
                //get difference

                //to double, thinking, that have byte(uint8) to obtain negative values
                //var Rdif = ArrOp.AbsArrayElements(ArrOp.SubArrays(ArrOp.ArrayToDouble(cListOrigin[0].c), ArrOp.ArrayToDouble(cListMod[0].c)));
                //var Gdif = ArrOp.AbsArrayElements(ArrOp.SubArrays(ArrOp.ArrayToDouble(cListOrigin[1].c), ArrOp.ArrayToDouble(cListMod[1].c)));
                //var Bdif = ArrOp.AbsArrayElements(ArrOp.SubArrays(ArrOp.ArrayToDouble(cListOrigin[2].c), ArrOp.ArrayToDouble(cListMod[2].c))); 

                //suppose work with uint8
                var Rdif = ArrOp.Uint8Range(ArrOp.ArraySubWithConst(ArrOp.AbsArrayElements(ArrOp.SubArrays(cListOrigin[0].c, cListMod[0].c)), coefOne));
                var Gdif = ArrOp.Uint8Range(ArrOp.ArraySubWithConst(ArrOp.AbsArrayElements(ArrOp.SubArrays(cListOrigin[1].c, cListMod[1].c)), coefOne));
                var Bdif = ArrOp.Uint8Range(ArrOp.ArraySubWithConst(ArrOp.AbsArrayElements(ArrOp.SubArrays(cListOrigin[2].c, cListMod[2].c)), coefOne));

                Rdif = ArrOp.Uint8Range(ArrOp.ArrayMultByConst(Rdif, coefTwo));
                Gdif = ArrOp.Uint8Range(ArrOp.ArrayMultByConst(Gdif, coefTwo));
                Bdif = ArrOp.Uint8Range(ArrOp.ArrayMultByConst(Bdif, coefTwo));

                //make origin image with opacity
                double alpha = 0.9;

                //make fake alpha vision. Asume suppose background - white 255
                //using next formula background + (foreground - background) * alpha
                var fakeR = ArrOp.ArrayToUint8(ArrOp.ArraySumWithConst(ArrOp.ArrayMultByConst(ArrOp.ArrayToDouble(ArrOp.ArraySubWithConst(cListOrigin[0].c, 255)), alpha), 255));
                var fakeG = ArrOp.ArrayToUint8(ArrOp.ArraySumWithConst(ArrOp.ArrayMultByConst(ArrOp.ArrayToDouble(ArrOp.ArraySubWithConst(cListOrigin[1].c, 255)), alpha), 255));
                var fakeB = ArrOp.ArrayToUint8(ArrOp.ArraySumWithConst(ArrOp.ArrayMultByConst(ArrOp.ArrayToDouble(ArrOp.ArraySubWithConst(cListOrigin[2].c, 255)), alpha), 255));

                //image = Helpers.setPixelsAlpha(image, Rdif, Gdif, Bdif, alpha);
                //var cListAl = Helpers.getPixels(image);

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

                arrGen<int> d;
                d = new arrGen<int>();

                var R = d.vecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newrVector.ToArray());
                var G = d.vecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newgVector.ToArray());
                var B = d.vecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newbVector.ToArray());
                //lay difference on alpha image

                image = Helpers.setPixels(image, R, G, B);
                outName = Directory.GetCurrentDirectory() + "\\Rand\\Diff.jpg";
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                image.Save(outName);
            }
        }

        //i am dumb here
        public static void Write8bppImage(Bitmap img, int width, int height, int[,] Imbytes)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format8bppIndexed);

            ColorPalette pal = image.Palette;
            for (int i = 0; i < 256; i++)
            {
                pal.Entries[i] = Color.FromArgb(i, i, i);
            }
            image.Palette = pal;
            image.Save("10.jpg");

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int c = Imbytes[y, x];
                    image.SetPixel(x, y, Color.FromArgb(c, c, c));
                    //image.Palette.Entries[x,y] = Color.FromArgb(red, green, blue);
                }
            }
        }             
    } 
   
    public enum RandFiltOption
    {
        defaulto,
        enterMinMax,        
    }    
}
