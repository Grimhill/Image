using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

namespace Image
{
    class Program
    {
        static void Main(string[] args)
        {
            //recommended 24bbp format image (8 bit per color)
            //!!!!!
            //Все входные не 8 разрядные (uint8), а выше (uint16) првести к uint8. Binary & 8b - some troubles 

            string ImageFilePath = "Contrast_8.jpg";
            string ImgExtension = Path.GetExtension(ImageFilePath).ToLower();

            if (Helpers.CheckForInputFormat(ImgExtension))
            {    
                ArrayOperations ArrOp = new ArrayOperations();

                System.Drawing.Bitmap image = new System.Drawing.Bitmap(ImageFilePath);          

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

                #region exist operations list            
                //serach contours on image. Parameters: image, variants: 1-6
                //!!! if will be enter interface (or in console) lead variant number to enum or delete enum and use only numbers    
                //image for test - imgfortest\dragon.jpg 
                //Contour.GlobalContour(image, CountourVariant.Variant6_RGB, ImgExtension);

                //contrast for black & white image
                //image for test - imgfortest\contrast\Contrast_24.jpg
                //all default
                //Contrast.ContrastBW(image, ImgExtension);
                //Parameters: low_in & high_in contrast limits, gamma coefficient
                //Contrast.ContrastBW(image, 0.3, 0.7, 1, 0.8, ImgExtension);

                //contrast for RGB image
                //Parameters: low_in & high_in contrast limits for all color components
                //image for test - imgfortest\contrast\contrastRGB.png
                //Contrast.ContrastRGB(image, 0.2, 0.6, 0.3, 0.7, 0, 1, ImgExtension);
                #endregion exist operations list             

            }
            else { }

            Console.ReadLine();
        }

        public static void CropImage(Bitmap bitmap, string filename, int cutLeft, int cutRight, int cutTop, int cutBottom, string ImgExtension)
        {
            //count new width and height
            int newWidth = bitmap.Width - cutLeft - cutRight;
            int newHeight = bitmap.Height - cutTop - cutBottom;

            //new bitmam object for cropped image    
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(newWidth, newHeight, bitmap.PixelFormat);

            if (newWidth <= 0 || newHeight <= 0)
            {
                Console.WriteLine("Crop width or height more than image`s one.");
            }
            else
            {
                //left x-coordinate of the upper-left corner of the rectangle.
                //right y-coordinate of the upper-left corner of the rectangle.
                Rectangle rect = new Rectangle(cutLeft, cutTop, newWidth, newHeight);

                //clone selected area into new bitmap object
                Bitmap cropped = bitmap.Clone(rect, bitmap.PixelFormat);

                string outName = Directory.GetCurrentDirectory() + "\\Cropped\\" + filename + "_cropped" + ImgExtension;

                //img.Save(outName, ImageFormat.Jpeg);
                Helpers.SaveOptions(img, outName, ImgExtension);
            }
        }


        //filter in size not more, then 4x4
        public static void EnterFilter(Bitmap img, double[,] filter)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;

            var ColorList = Helpers.GetPixels(img);

            var cPlaneOne = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(ColorList[0].Color), filter, PadType.replicate));
            var cPlaneTwo = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(ColorList[1].Color), filter, PadType.replicate));
            var cPlaneThree = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(ColorList[2].Color), filter, PadType.replicate));

            image = Helpers.SetPixels(image, cPlaneOne, cPlaneTwo, cPlaneThree);
            outName = Directory.GetCurrentDirectory() + "\\Rand\\ImageEnterFilter.jpg";
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //random in process
        //bad idea, couz almost all results will be some ugly pixels
        public static void RandomFilter(Bitmap img, double min, double max)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
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

            var ColorList = Helpers.GetPixels(img);

            var cPlaneOne = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(ColorList[0].Color), filter, PadType.replicate));
            var cPlaneTwo = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(ColorList[1].Color), filter, PadType.replicate));
            var cPlaneThree = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(ColorList[2].Color), filter, PadType.replicate));

            image = Helpers.SetPixels(image, cPlaneOne, cPlaneTwo, cPlaneThree);
            outName = Directory.GetCurrentDirectory() + "\\Rand\\ImageRandomFilter.jpg";
            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        //mathod with default coeficients and alpha
        //or i am dumb, or sime problems with alpha chennel in bitmap library
        public static void Difference(Bitmap imgOrig, Bitmap imgMod, double coefOne, double coefTwo) //, double alpha
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = imgOrig.Width;
            int height = imgOrig.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);
            string outName = String.Empty;

            if (imgOrig.Width != imgMod.Width || imgOrig.Height != imgMod.Height)
            {
                Console.WriteLine("Origin and modified images dimentions dismatch or them different");
            }
            else
            {
                var cListOrigin = Helpers.GetPixels(imgOrig);
                var cListMod = Helpers.GetPixels(imgMod);           

                //suppose work with uint8
                var Rdif = ArrOp.Uint8Range(ArrOp.ArraySubWithConst(ArrOp.AbsArrayElements(ArrOp.SubArrays(cListOrigin[0].Color, cListMod[0].Color)), coefOne));
                var Gdif = ArrOp.Uint8Range(ArrOp.ArraySubWithConst(ArrOp.AbsArrayElements(ArrOp.SubArrays(cListOrigin[1].Color, cListMod[1].Color)), coefOne));
                var Bdif = ArrOp.Uint8Range(ArrOp.ArraySubWithConst(ArrOp.AbsArrayElements(ArrOp.SubArrays(cListOrigin[2].Color, cListMod[2].Color)), coefOne));

                Rdif = ArrOp.Uint8Range(ArrOp.ArrayMultByConst(Rdif, coefTwo));
                Gdif = ArrOp.Uint8Range(ArrOp.ArrayMultByConst(Gdif, coefTwo));
                Bdif = ArrOp.Uint8Range(ArrOp.ArrayMultByConst(Bdif, coefTwo));

                //make origin image with opacity
                double alpha = 0.9;

                //make fake alpha vision. Asume suppose background - white 255
                //using next formula background + (foreground - background) * alpha
                var fakeR = ArrOp.ArrayToUint8(ArrOp.ArraySumWithConst(ArrOp.ArrayMultByConst(ArrOp.ArrayToDouble(ArrOp.ArraySubWithConst(cListOrigin[0].Color, 255)), alpha), 255));
                var fakeG = ArrOp.ArrayToUint8(ArrOp.ArraySumWithConst(ArrOp.ArrayMultByConst(ArrOp.ArrayToDouble(ArrOp.ArraySubWithConst(cListOrigin[1].Color, 255)), alpha), 255));
                var fakeB = ArrOp.ArrayToUint8(ArrOp.ArraySumWithConst(ArrOp.ArrayMultByConst(ArrOp.ArrayToDouble(ArrOp.ArraySubWithConst(cListOrigin[2].Color, 255)), alpha), 255));

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

                ArrGen<int> d;
                d = new ArrGen<int>();

                var R = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newrVector.ToArray());
                var G = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newgVector.ToArray());
                var B = d.VecorToArrayRowByRow(fakeR.GetLength(0), fakeR.GetLength(1), newbVector.ToArray());
                //lay difference on alpha image

                image = Helpers.SetPixels(image, R, G, B);
                outName = Directory.GetCurrentDirectory() + "\\Rand\\Diff.jpg";
                //dont forget, that directory Rand must exist. Later add if not exist - creat
                image.Save(outName);
            }
        }

        //i am dumb here. I give up
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

    public enum SupportFormats
    {
        jpg,
        jpeg,
        png,
        bmp,
        tiff,
        gif
    }
}
