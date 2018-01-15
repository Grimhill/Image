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
    class AnotherVariants
    {
        #region 1
        //add rows for padarray function, using CopyTo

        //add top row
        //List<int> b_t = new List<int>();
        //zeros
        //b_t = new List<int>(new int[width]);

        //var b = b_t.ToArray();

        //var z = new int[Rc.Length + b.Length];
        //int[] d = Rc.Cast<int>().ToArray();
        //int[,]result = new int[height + 1, width];

        //b.CopyTo(z, 0);
        //d.CopyTo(z, b.Length);

        ////back to 2d array
        //int c = 0;
        //for (int u = 0; u < result.GetLength(0); u++)
        //{
        //    for (int k = 0; k < result.GetLength(1); k++)
        //    {
        //        result[u, k] = z[c];
        //        c++;
        //    }
        //}
        #endregion 1

        ///////////////////////////////////////////////////////

        #region 2
        public static void im2BW(Bitmap img, inEdge inIm)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height, PixelFormat.Format1bppIndexed);
            int[,] result = new int[img.Height, img.Width];
            string outName = String.Empty;
            double Depth = 0;

            double level = 0.5; //default

            Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);
            int[,] im = new int[img.Height, img.Width];
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
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

            outName = Directory.GetCurrentDirectory() + "\\Rand\\im2bin.jpg";
            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }

        public static void im2BW(Bitmap img, inEdge inIm, double level)
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
            var ColorList = Helpers.getPixels(img);

            if (inIm.ToString() == "BW8b")
            {
                if (Depth != 8)
                { Console.WriteLine("Wrong input arguments, input image not BW8b"); }
                else
                { im = ColorList[0].c; }
            }
            else if (inIm.ToString() == "rgb")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not rgb"); }
                else
                { im = Helpers.rgbToGrayArray(img); }
            }
            else if (inIm.ToString() == "BW24b")
            {
                if (Depth != 24)
                { Console.WriteLine("Wrong input arguments, input image not BW24b"); }
                else
                { im = ColorList[0].c; }
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

            outName = Directory.GetCurrentDirectory() + "\\Rand\\im2bin.jpg";
            image = Helpers.setPixels(image, result, result, result);

            //dont forget, that directory Rand must exist. Later add if not exist - creat
            image.Save(outName);
        }
        #endregion

        /////////////////////////////////////////////////////////
    }
}
