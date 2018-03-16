using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;
using Image.ArrayOperations;

//Helper operations
namespace Image
{
    public static class Helpers
    {
        //obtain gray array from RGB image by rule
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

        #region Pixels area
        //GEt and set pixels operations

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
                        RedColor  [i, j] = pixelColor.R;
                        GreenColor[i, j] = pixelColor.G;
                        BlueColor [i, j] = pixelColor.B;
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
                        RedColor  [i, j] = pixelColor.R;
                        GreenColor[i, j] = pixelColor.G;
                        BlueColor [i, j] = pixelColor.B;
                        Alpha     [i, j] = pixelColor.A;
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
                        int red   = resultR[y, x];
                        int green = resultG[y, x];
                        int blue  = resultB[y, x];
                        
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

        #region SetPixelsAlpha
        
        public static Bitmap SetPixelsAlpha(int[,] r, int[,] g, int[,] b, double alpha)
        {
            int height = r.GetLength(0);
            int width  = r.GetLength(1);

            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            if (r.GetLength(0) != g.GetLength(0) || r.GetLength(0) != b.GetLength(0)
                || r.GetLength(1) != g.GetLength(1) || r.GetLength(1) != b.GetLength(1))
            {
                Console.WriteLine("Array dimentions dismatch in operation. Method: Helpers.SetPixelsAlpha");
            }
            else if (r.Cast<int>().Min() < 0 || g.Cast<int>().Min() < 0 || b.Cast<int>().Min() < 0)
            {
                Console.WriteLine("One of RGB array contain negativ vaules. Method: Helpers.SetPixelsAlpha");
            }
            else
            {                
                int Alpha = (int)(alpha * 255);
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int red   = r[y, x];
                        int green = g[y, x];
                        int blue  = b[y, x];
                        try
                        {
                            image.SetPixel(x, y, Color.FromArgb(Alpha, red, green, blue));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> setPixelsAlpha <-");
                        }
                    }
                }
            }

            return image;
        }

        public static Bitmap SetPixelsAlpha(Bitmap img, double alpha)
        {         
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);

            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            if (Depth != 8)
            {
                int Alpha = (int)(alpha * 255);
                var ColorList = GetPixels(img);

                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int red   = ColorList[0].Color[y, x];
                        int green = ColorList[1].Color[y, x];
                        int blue  = ColorList[2].Color[y, x];
                        try
                        {
                            image.SetPixel(x, y, Color.FromArgb(Alpha, red, green, blue));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> setPixelsAlpha <-");
                        }
                    }
                }
            }
            else
            { Console.WriteLine("Cannot set alpha channel for indexed 8bpp image. Method: SetAlpha"); }

            return image;
        }

        public static Bitmap SetPixelsAlpha(int[,] r, int[,] g, int[,] b, int[,] alpha)
        {
            int height = r.GetLength(0);
            int width  = r.GetLength(1);

            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            if (r.GetLength(0) != g.GetLength(0) || r.GetLength(0) != b.GetLength(0)
                || r.GetLength(1) != g.GetLength(1) || r.GetLength(1) != b.GetLength(1))
            {
                Console.WriteLine("Array dimentions dismatch in operation. Method: Helpers.SetPixelsAlpha");
            }
            else if (r.Cast<int>().Min() < 0 || g.Cast<int>().Min() < 0 || b.Cast<int>().Min() < 0)
            {
                Console.WriteLine("One of RGB array contain negativ vaules. Method: Helpers.SetPixelsAlpha");
            }
            else
            {
                for (int y = 0; y < image.Height; y++)
                {
                    for (int x = 0; x < image.Width; x++)
                    {
                        int red   = r[y, x];
                        int green = g[y, x];
                        int blue  = b[y, x];
                        int Alpha = alpha[y, x];
                        try
                        {
                            image.SetPixel(x, y, Color.FromArgb(Alpha, red, green, blue));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> setPixelsAlpha <-");
                        }
                    }
                }
            }

            return image;
        }
        #endregion

        #endregion Pixels area

        //default such as input format
        public static void SaveOptions(Bitmap image, string path, string imgExtension)
        {
            double Depth = System.Drawing.Image.GetPixelFormatSize(image.PixelFormat);
            if (Depth == 8 || Depth == 1) { imgExtension = ".png"; }

            ImageFormat imageFormat;
            switch (imgExtension)
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

            image.Save(Checks.OutputFileNames(path), imageFormat);
        }

        //Sharp image after some operations with default unsharp filter
        public static Bitmap FastSharpImage(Bitmap img)
        {
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            var resultR = (ImageFilter.Filter_double(Rc, "unsharp")).ArrayToUint8(); 
            var resultG = (ImageFilter.Filter_double(Gc, "unsharp")).ArrayToUint8();
            var resultB = (ImageFilter.Filter_double(Bc, "unsharp")).ArrayToUint8();

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            return image;
        }
                
        #region obtain image in binary representation for some operations
        //Look SomeLittle -> ImageTo1Bpp to obtain binary image with write to file       
        public static int[,] Image2BinaryArray(Bitmap img)
        {            
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            int[,] result = new int[img.Height, img.Width];
           
            double level = 0.5; //default 
            var im = MoreHelpers.BlackandWhiteProcessHelper(img);
            result = im.Uint8ArrayToBinary(level);

            return result;
        }
        
        public static int[,] Image2BinaryArray(Bitmap img, double level)
        {           
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            int[,] result = new int[img.Height, img.Width];                     

            if (level > 1 || level < 0)
            {
                Console.WriteLine("Level value must be in range 0..1. Set to default 0.5");
                level = 0.5;
            }

            var im = MoreHelpers.BlackandWhiteProcessHelper(img);
            result = im.Uint8ArrayToBinary(level);            

            return result;
        }
        #endregion obtain image in binary view for some operations
         
        //obtain array with random int
        public static int[,] RandArray(int r, int c, int min, int max)
        {
            int [,] Result = new int[r,c];
            Random randNum = new Random();

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    Result[i, j] = randNum.Next(min, max);
                }
            }

            return Result;
        }

        public static double[,] RandArray(int r, int c)
        {
            double[,] Result = new double[r, c];
            Random randNum = new Random();

            for (int i = 0; i < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    Result[i, j] = randNum.NextDouble();
                }
            }

            return Result;
        }
    }

    //array to console
    public class ConsArray <T>
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

    //help classes for list of arrays and vectors
    public class ArraysListInt
    {
        public int[,] Color { get; set; }
    }

    public class ArraysListDouble
    {
        public double[,] Color { get; set; }
    }

    public class ArraysListT <T>
    {
        public T[,] Color { get; set; }
    }

    public class VectorsListInt
    {
        public int[] Vect { get; set; }
    }

    public class VectorsListDouble
    {
        public double[] Vect { get; set; }
    }

    public class VectorsListT <T>
    {
        public T[] Vect { get; set; }
    }

    public class ListsListDouble
    {
        public List<double> List { get; set; }
    }
}
