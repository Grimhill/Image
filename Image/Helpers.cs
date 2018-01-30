using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using Image.ArrayOperations;

//Helper operations
namespace Image
{
    public static class Helpers
    {
        #region Pixels area
        public static int[,] RGBToGrayArray(Bitmap image)
        {
            int[,] gray = new int[image.Height, image.Width]; ;
            // Loop through the images pixels to reset color.
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    Color pixelColor = image.GetPixel(x, y);

                    int r = pixelColor.R;
                    int g = pixelColor.G;
                    int b = pixelColor.B;
                    //0.2989 * R + 0.5870 * G + 0.1140 * B 
                    gray[y, x] = (int)(0.2989 * r + 0.587 * g + 0.114 * b);
                }
            }
            return gray;
        }

        public static List<ArraysListInt> GetPixels(Bitmap image)
        {
            List<ArraysListInt> aRList = new List<ArraysListInt>();

            int[,] RedColor   = new int[image.Height, image.Width];
            int[,] GreenColor = new int[image.Height, image.Width];
            int[,] BlueColor  = new int[image.Height, image.Width];

            try
            {
                //read row by row image R\G\B pixels value
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        Color pixelColor = image.GetPixel(j, i);
                        RedColor[i, j] = pixelColor.R;
                        GreenColor[i, j] = pixelColor.G;
                        BlueColor[i, j] = pixelColor.B;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem at GetPixels method: " + e.Message);
            }

            aRList.Add(new ArraysListInt() { Color = RedColor });   //R
            aRList.Add(new ArraysListInt() { Color = GreenColor }); //G
            aRList.Add(new ArraysListInt() { Color = BlueColor });  //B

            return aRList;
        }

        public static List<ArraysListInt> GetPixelswithAlpha(Bitmap image)
        {
            List<ArraysListInt> aRList = new List<ArraysListInt>();

            int[,] RedColor   = new int[image.Height, image.Width];
            int[,] GreenColor = new int[image.Height, image.Width];
            int[,] BlueColor  = new int[image.Height, image.Width];
            int[,] Alpha      = new int[image.Height, image.Width];

            try
            {
                //read row by row image R\G\B pixels value
                for (int i = 0; i < image.Height; i++)
                {
                    for (int j = 0; j < image.Width; j++)
                    {
                        Color pixelColor = image.GetPixel(j, i);
                        RedColor[i, j] = pixelColor.R;
                        GreenColor[i, j] = pixelColor.G;
                        BlueColor[i, j] = pixelColor.B;
                        Alpha[i, j] = pixelColor.A;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem at GetPixels method: " + e.Message);
            }

            aRList.Add(new ArraysListInt() { Color = RedColor });   //R
            aRList.Add(new ArraysListInt() { Color = GreenColor }); //G
            aRList.Add(new ArraysListInt() { Color = BlueColor });  //B
            aRList.Add(new ArraysListInt() { Color = Alpha });  //A

            return aRList;
        }

        public static Bitmap SetPixels(Bitmap image, int[,] resultR, int[,] resultG, int[,] resultB)
        {
            try
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int red = resultR[y, x];
                        int green = resultG[y, x];
                        int blue = resultB[y, x];

                        image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> setPixels <-");
            }
            return image;
        }

        //dont know what doing wrong and alpha didnt work correctly
        public static Bitmap SetPixelsAlpha(Bitmap image, int[,] resultR, int[,] resultG, int[,] resultB, double alpha)
        {
            Bitmap img = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            int Alpha = (int)(alpha * 255);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int red   = resultR[y, x];
                    int green = resultG[y, x];
                    int blue  = resultB[y, x];
                    try
                    {
                        img.SetPixel(x, y, Color.FromArgb(Alpha, red, green, blue));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> setPixelsAlpha <-");
                    }
                }
            }
            return img;
        }

        public static Bitmap SetPixelsAlpha(Bitmap image, int[,] resultR, int[,] resultG, int[,] resultB, int[,] Alpha)
        {
            Bitmap img = new Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int red   = resultR[y, x];
                    int green = resultG[y, x];
                    int blue  = resultB[y, x];
                    int alpha = Alpha[y, x];
                    try
                    {
                        img.SetPixel(x, y, Color.FromArgb(alpha, red, green, blue));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> setPixelsAlpha <-");
                    }
                }
            }
            return img;
        }

        #endregion Pixels area

        //default such as input format
        public static void SaveOptions(Bitmap image, string path, string ImgExtension)
        {
            ImageFormat imageFormat;
            switch (ImgExtension)
            {
                case ".jpg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case ".bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case ".png":
                    imageFormat = ImageFormat.Png;
                    break;
                case ".tif":
                    imageFormat = ImageFormat.Tiff;
                    break;
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }

            image.Save(path, imageFormat);
        }

        //Sharp image after some operations with default unsharp filter
        public static Bitmap FastSharpImage(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            var resultR = (Filter.Filter_double(Rc, "unsharp")).ArrayToUint8();
            var resultG = (Filter.Filter_double(Gc, "unsharp")).ArrayToUint8();
            var resultB = (Filter.Filter_double(Bc, "unsharp")).ArrayToUint8();

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            return image;
        }        

        //Look SomeLittle -> ImageTo1Bpp to obtain binary image with write to file
        //make return result as bool?     

        public static int[,] Image2Binary(Bitmap img, double level)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;

            if (level > 1 || level < 0)
            {
                Console.WriteLine("Level value must be in range 0..1. Set to default 0.5");
                level = 0.5;
            }

            var im = MoreHelpers.BlackandWhiteProcessHelper(img);

            for (int i = 0; i < im.GetLength(0); i++)
            {
                for (int j = 0; j < im.GetLength(1); j++)
                {
                    if (im[i, j] > 255 * level) //0..255 - uint8 range
                    {
                        result[i, j] = 1;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }

            return result;
        }                 

        public static void WriteImageToFile(int[,] R, int[,] G, int[,] B, string fileName, OutType type)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            Checks.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in hsv2rgb operation -> WriteImageToFile(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                Bitmap image = new Bitmap(R.GetLength(1), G.GetLength(0), PixelFormat.Format24bppRgb);

                image = SetPixels(image, R, G, B);

                if (type == OutType.OneBpp)
                {
                    SomeLittle.ImageTo1Bpp(image, 0.5, fileName);
                }
                else if (type == OutType.EightBpp)
                {
                    MoreHelpers.Bbp24Gray2Gray8bpp(image, fileName);
                }
                else
                {
                    string outName = Checks.OutputFileNames(Directory.GetCurrentDirectory() + "\\Rand\\" + fileName);
                    //image.Save(outName);
                    Helpers.SaveOptions(image, outName, ImgExtension);
                }
            }
        }

        public static void WriteImageToFile(int[,] R, int[,] G, int[,] B, string fileName, string directoryName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            Checks.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Rand");

            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in hsv2rgb operation -> WriteImageToFile(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                Bitmap image = new Bitmap(R.GetLength(1), G.GetLength(0), PixelFormat.Format24bppRgb);
                image = SetPixels(image, R, G, B);

                string outName = Checks.OutputFileNames(Directory.GetCurrentDirectory() + "\\" + directoryName + "\\" + fileName + ImgExtension);
                //image.Save(outName);
                Helpers.SaveOptions(image, outName, ImgExtension);
            }
        }
              

        //obtain array with random
        public static int[,] RandArray(int w, int h, int min, int max)
        {
            int[,] Result = new int[h, w];
            Random randNum = new Random();

            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    Result[i, j] = randNum.Next(min, max);
                }
            }

            return Result;
        }
    }

    //array to console
    public class ConsArray<T>
    {
        public void ArrayToConsole(T[,] arr)
        {
            //Example to use
            //ConsArray<int> arrcon = new ConsArray<int>();
            //arrcon.ArrayToConsole(Rc);

            //check
            for (int i = 0; i < arr.GetLength(0); i++)
            {
                for (int j = 0; j < arr.GetLength(1); j++)
                {
                    Console.Write(string.Format("{0} ", arr[i, j]));
                }
                Console.Write(Environment.NewLine + Environment.NewLine);
            }
            //Console.ReadLine();
        }
    }

    public class ArraysListInt
    {
        public int[,] Color { get; set; }
    }

    public class ArraysListDouble
    {
        public double[,] Color { get; set; }
    }

    public class ArraysListT<T>
    {
        public T[,] Color { get; set; }
    }
}
