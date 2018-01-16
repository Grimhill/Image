using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

//Helper operations
namespace Image
{
    public static class Helpers
    {
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

            int[,] RedColor = new int[image.Height, image.Width];
            int[,] GreenColor = new int[image.Height, image.Width];
            int[,] BlueColor = new int[image.Height, image.Width];

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

            aRList.Add(new ArraysListInt() { Color = RedColor }); //R
            aRList.Add(new ArraysListInt() { Color = GreenColor }); //G
            aRList.Add(new ArraysListInt() { Color = BlueColor }); //B

            return aRList;
        }

        public static Bitmap SetPixels(Bitmap image, int[,] resultR, int[,] resultG, int[,] resultB)
        {
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int red = resultR[y, x];
                    int green = resultG[y, x];
                    int blue = resultB[y, x];
                    try
                    {
                        image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception in setPixels:" + e.Message + "\n Method: -> setPixels <-");
                    }
                }
            }
            return image;
        }

        //default such as input format
        public static void SaveOptions(Bitmap image, string name, string ImgExtension)
        {
            ImageFormat imageFormat;
            switch (ImgExtension)
            {
                case "jpg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "jpeg":
                    imageFormat = ImageFormat.Jpeg;
                    break;
                case "bmp":
                    imageFormat = ImageFormat.Bmp;
                    break;
                case "png":
                    imageFormat = ImageFormat.Png;
                    break;
                case "tiff":
                    imageFormat = ImageFormat.Tiff;
                    break;
                case "gif":
                    imageFormat = ImageFormat.Gif;
                    break;
                default:
                    imageFormat = ImageFormat.Jpeg;
                    break;
            }

            image.Save(name, imageFormat);
        }
        public static List<string> AvailableFormats = new List<string>() { ".jpg", ".jpeg", ".bmp", ".png", ".tiff", ".gif" };
        public static bool CheckForInputFormat(string ImgExtension)
        {
            if (!AvailableFormats.Contains(ImgExtension))
            {
                Console.WriteLine("Unsupport image format (extension. Support: jpg, jpeg, bmp, png, tiff, gif (only first picture for animation)");
                return false;
            }
            else if (ImgExtension == ".gif")
            {
                Console.WriteLine("Worning! For gif animation take only first picture");
                return true;
            }
            return true;
        }

        //dont know what doing wrong and alpha didnt work correctly
        public static Bitmap SetPixelsAlpha(Bitmap image, int[,] resultR, int[,] resultG, int[,] resultB, double alpha)
        {
            System.Drawing.Bitmap img = new System.Drawing.Bitmap(image.Width, image.Height, PixelFormat.Format32bppArgb);
            int Alpha = (int)(alpha * 255);
            for (int y = 0; y < image.Height; y++)
            {
                for (int x = 0; x < image.Width; x++)
                {
                    int red = resultR[y, x];
                    int green = resultG[y, x];
                    int blue = resultB[y, x];
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
            return image;
        }

        //Sharp image after some operations with default unsharp filter
        public static Bitmap FastSharpImage(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width = img.Width;
            int height = img.Height;

            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.GetPixels(img);
            var Rc = ColorList[0].Color;
            var Gc = ColorList[1].Color;
            var Bc = ColorList[2].Color;

            var resultR = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Rc), Filter.Dx3FWindow("unsharp"), PadType.replicate));
            var resultG = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Gc), Filter.Dx3FWindow("unsharp"), PadType.replicate));
            var resultB = ArrOp.ArrayToUint8(Filter.Filter_double(ArrOp.ArrayToDouble(Bc), Filter.Dx3FWindow("unsharp"), PadType.replicate));

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            return image;
        }

        //some bad
        #region obtain image in binary view for some operations
        //Look SomeLittle -> ImageTo1Bpp to obtain binary image with write to file
        public static int[,] Image2Binary(Bitmap img, inEdge inIm)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            int[,] result = new int[img.Height, img.Width];

            double Depth = 0;
            double level = 0.5; //default

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.GetPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].Color; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.RGBToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].Color; }
            }

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

        public static int[,] Image2Binary(Bitmap img, double level, inEdge inIm)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;
            double Depth = 0;

            if (level > 1 || level < 0)
            {
                Console.WriteLine("Level value must be in range 0..1. Set to default 0.5");
                level = 0.5;
            }

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.GetPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].Color; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.RGBToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].Color; }
            }

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
        #endregion obtain image in binary view for some operations

        //Some scary. Rework all some time
        #region write image to file overload methods
        public static void WriteImageToFile(Bitmap img, string fileName, string ImgExtension)
        {
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + ImgExtension;

            Helpers.SaveOptions(img, outName, ImgExtension);
            //image.Save(outName);
        }

        public static void WriteImageToFile(List<ArraysListInt> Colors, string fileName)
        {
            if (Colors[0].Color.Length != Colors[1].Color.Length || Colors[0].Color.Length != Colors[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> WriteImageToFile(List<arraysListInt> Colors, string fileName) <-");
            }
            else
            {
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(Colors[0].Color.GetLength(1), Colors[0].Color.GetLength(0), PixelFormat.Format24bppRgb);

                image = SetPixels(image, Colors[0].Color, Colors[1].Color, Colors[2].Color);

                //dont forget, that directory Rand must exist. Later add if not exist - creat
                string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + ".jpg";
                image.Save(outName);
            }
        }

        public static void WriteImageToFile(List<ArraysListInt> Colors, string fileName, double alpha)
        {
            if (Colors[0].Color.Length != Colors[1].Color.Length || Colors[0].Color.Length != Colors[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> WriteImageToFile(List<arraysListInt> Colors, string fileName) <-");
            }
            else
            {
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(Colors[0].Color.GetLength(1), Colors[0].Color.GetLength(0));

                image = SetPixelsAlpha(image, Colors[0].Color, Colors[1].Color, Colors[2].Color, alpha);

                //dont forget, that directory Rand must exist. Later add if not exist - creat
                string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + ".jpg";
                image.Save(outName);
            }
        }

        public static void WriteImageToFile(List<ArraysListDouble> Colors, string fileName)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            if (Colors[0].Color.Length != Colors[1].Color.Length || Colors[0].Color.Length != Colors[2].Color.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> WriteImageToFile(List<arraysListDouble> Colors, string fileName) <-");
            }
            else
            {
                int[,] colorPlaneOne = new int[Colors[0].Color.GetLength(0), Colors[0].Color.GetLength(1)];
                int[,] colorPlaneTwo = new int[Colors[0].Color.GetLength(0), Colors[0].Color.GetLength(1)];
                int[,] colorPlaneThree = new int[Colors[0].Color.GetLength(0), Colors[0].Color.GetLength(1)];

                var cat = Colors[0].Color.Cast<double>().ToArray().Max();
                if (cat < 1)
                {
                    colorPlaneOne = ArrOp.ImageArrayToUint8(Colors[0].Color);
                    colorPlaneTwo = ArrOp.ImageArrayToUint8(Colors[1].Color);
                    colorPlaneThree = ArrOp.ImageArrayToUint8(Colors[2].Color);
                }
                else if (cat >= 0 & cat <= 255)
                {
                    colorPlaneOne = ArrOp.ArrayToUint8(Colors[0].Color);
                    colorPlaneTwo = ArrOp.ArrayToUint8(Colors[1].Color);
                    colorPlaneThree = ArrOp.ArrayToUint8(Colors[2].Color);
                }

                System.Drawing.Bitmap image = new System.Drawing.Bitmap(Colors[0].Color.GetLength(1), Colors[0].Color.GetLength(0), PixelFormat.Format24bppRgb);

                image = SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

                //dont forget, that directory Rand must exist. Later add if not exist - creat
                string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + ".jpg";
                image.Save(outName);
            }
        }

        public static void WriteImageToFile(int[,] R, int[,] G, int[,] B, string fileName)
        {
            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("mage plane arrays size dismatch in hsv2rgb operation -> WriteImageToFile(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(R.GetLength(1), G.GetLength(0), PixelFormat.Format24bppRgb);

                image = SetPixels(image, R, G, B);

                //dont forget, that directory Rand must exist. Later add if not exist - creat
                string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + ".jpg";
                image.Save(outName);
            }
        }

        public static void WriteImageToFile(int[,] R, int[,] G, int[,] B, string fileName, double alpha)
        {
            if (R.Length != G.Length || R.Length != B.Length)
            {
                Console.WriteLine("mage plane arrays size dismatch in hsv2rgb operation -> WriteImageToFile(int[,] R, int[,] G, int[,] B) <-");
            }
            else
            {
                System.Drawing.Bitmap image = new System.Drawing.Bitmap(R.GetLength(1), G.GetLength(0));

                image = SetPixelsAlpha(image, R, G, B, alpha);

                //dont forget, that directory Rand must exist. Later add if not exist - creat
                string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + ".jpg";
                image.Save(outName);
            }
        }

        public static void WriteImageToFile(double[,] planeOne, double[,] planeTwo, double[,] planeThree, string fileName)
        {
            ArrayOperations ArrOp = new ArrayOperations();

            if (planeOne.Length != planeTwo.Length || planeTwo.Length != planeThree.Length)
            {
                Console.WriteLine("Image plane arrays size dismatch in operation -> WriteImageToFile(double[,] planeOne, double[,] planeTwo, double[,] planeThree, string fileName) <-");
            }
            else
            {
                int[,] colorPlaneOne = new int[planeOne.GetLength(0), planeOne.GetLength(1)];
                int[,] colorPlaneTwo = new int[planeOne.GetLength(0), planeOne.GetLength(1)];
                int[,] colorPlaneThree = new int[planeOne.GetLength(0), planeOne.GetLength(1)];

                var cat = planeOne.Cast<double>().ToArray().Max(); //risk, then only first plane Meets the condition. Make for all?
                if (cat < 1)
                {
                    colorPlaneOne = ArrOp.ImageArrayToUint8(planeOne);
                    colorPlaneTwo = ArrOp.ImageArrayToUint8(planeTwo);
                    colorPlaneThree = ArrOp.ImageArrayToUint8(planeThree);
                }
                else if (cat >= 0 & cat <= 255)
                {
                    colorPlaneOne = ArrOp.ArrayToUint8(planeOne);
                    colorPlaneTwo = ArrOp.ArrayToUint8(planeTwo);
                    colorPlaneThree = ArrOp.ArrayToUint8(planeThree);
                }
                else if (cat < 0 || cat > 255)
                {
                    colorPlaneOne = ArrOp.ArrayToUint8(planeOne);
                    colorPlaneTwo = ArrOp.ArrayToUint8(planeTwo);
                    colorPlaneThree = ArrOp.ArrayToUint8(planeThree);
                }

                System.Drawing.Bitmap image = new System.Drawing.Bitmap(planeOne.GetLength(1), planeOne.GetLength(0), PixelFormat.Format24bppRgb);

                image = SetPixels(image, colorPlaneOne, colorPlaneTwo, colorPlaneThree);

                //dont forget, that directory Rand must exist. Later add if not exist - creat
                string outName = Directory.GetCurrentDirectory() + "\\Rand\\" + fileName + ".jpg";
                image.Save(outName);
            }
        }
        #endregion write image to file overload methods

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
            //ConsArray<int> arrcon;
            //arrcon = new ConsArray<int>();
            //arrcon.arrayToConsole(Rc);

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
