using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    public static class Contour
    {
        ////only for RGB images, b&w 24bbp. (what about8bpp?)
        public static void GlobalContour(Bitmap img, CountourVariant Variant)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            int width  = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            var ColorList = Helpers.getPixels(img);
            var Rc = ColorList[0].c;
            var Gc = ColorList[1].c;
            var Bc = ColorList[2].c;

            int[,] resultR = new int[height, width];
            int[,] resultG = new int[height, width];
            int[,] resultB = new int[height, width];
            string outName = String.Empty;

            //variants 1-4 black & white. Variants 5, 6 - colored
            if (Variant.ToString() == "Variant1_BW" || Variant.ToString() == "Variant5_RGB" || Variant.ToString() == "Variant2_BW")
            {
                var Rx = Filter.filter_double(ArrOp.ArrayToDouble(Rc), Filter.Dx3FWindow("Sobel"), PadType.replicate);
                var Ry = Filter.filter_double(ArrOp.ArrayToDouble(Rc), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                var Gx = Filter.filter_double(ArrOp.ArrayToDouble(Gc), Filter.Dx3FWindow("Sobel"), PadType.replicate);
                var Gy = Filter.filter_double(ArrOp.ArrayToDouble(Gc), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                var Bx = Filter.filter_double(ArrOp.ArrayToDouble(Bc), Filter.Dx3FWindow("Sobel"), PadType.replicate);
                var By = Filter.filter_double(ArrOp.ArrayToDouble(Bc), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                if (Variant.ToString() == "Variant1_BW")
                {
                    resultR = ArrOp.ImageArrayToUint8(Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By));
                    resultG = resultR; resultB = resultR;
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV1.jpg";
                }
                else if (Variant.ToString() == "Variant2_BW")
                {
                    resultR = ArrOp.ImageArrayToUint8(ArrOp.SumArrays(Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By), Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By)));
                    resultG = resultR; resultB = resultR;
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV2.jpg";
                }
                else
                {
                    var RG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Rx, 2), ArrOp.PowArrayElements(Ry, 2))); //R gradient
                    var GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Gx, 2), ArrOp.PowArrayElements(Gy, 2))); //G gradient
                    var BG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Bx, 2), ArrOp.PowArrayElements(By, 2))); //B gradient

                    resultR = ArrOp.ArrayToUint8(RG);
                    resultG = ArrOp.ArrayToUint8(GG);
                    resultB = ArrOp.ArrayToUint8(BG);
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV5.jpg";
                }
            }

            else if (Variant.ToString() == "Variant3_BW" || Variant.ToString() == "Variant4_BW")
            {
                var gray = Helpers.rgbToGrayArray(img);
                double[,] GG = new double[height, width]; //gray gradient

                if (Variant.ToString() == "Variant3_BW")
                {
                    var Gx = Filter.filter_double(ArrOp.ArrayToDouble(gray), Filter.Dx3FWindow("Sobel"), PadType.replicate);
                    var Gy = Filter.filter_double(ArrOp.ArrayToDouble(gray), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                    GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Gx, 2), ArrOp.PowArrayElements(Gy, 2)));
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV3.jpg";
                }
                else
                {
                    var Gx = Filter.filter_int(gray, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                    var Gy = Filter.filter_int(gray, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                    GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gy), 2)));
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV4.jpg";
                }

                resultR = ArrOp.ArrayToUint8(GG); resultG = resultR; resultB = resultR;
            }
            else if (Variant.ToString() == "Variant6_RGB")
            {
                var Rx = Filter.filter_int(Rc, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                var Ry = Filter.filter_int(Rc, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                var Gx = Filter.filter_int(Gc, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                var Gy = Filter.filter_int(Gc, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                var Bx = Filter.filter_int(Bc, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                var By = Filter.filter_int(Bc, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                var RG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Rx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Ry), 2))); //R gradient
                var GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gy), 2))); //G gradient
                var BG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Bx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(By), 2))); //B gradient

                resultR = ArrOp.ArrayToUint8(RG);
                resultG = ArrOp.ArrayToUint8(GG);
                resultB = ArrOp.ArrayToUint8(BG);
                outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV6.jpg";
            }      

            image = Helpers.setPixels(image, resultR, resultG, resultB);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            image.Save(outName);
        }

        #region Experiments
        //Self realizated variants
        //*******************************************************************
        //Variant 1
        /*
        public static void ImageOutlines(int width, int height, int[,] Rc, int[,] Gc, int[,] Bc)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height);

            var Rx = Filter.filter_double(ArrOp.ArrayToDouble(Rc), Filter.Dx3FWindow("Sobel"), "replicate");
            var Ry = Filter.filter_double(ArrOp.ArrayToDouble(Rc), Filter.Dx3FWindow("SobelT"), "replicate");

            var Gx = Filter.filter_double(ArrOp.ArrayToDouble(Gc), Filter.Dx3FWindow("Sobel"), "replicate");
            var Gy = Filter.filter_double(ArrOp.ArrayToDouble(Gc), Filter.Dx3FWindow("SobelT"), "replicate");

            var Bx = Filter.filter_double(ArrOp.ArrayToDouble(Bc), Filter.Dx3FWindow("Sobel"), "replicate");
            var By = Filter.filter_double(ArrOp.ArrayToDouble(Bc), Filter.Dx3FWindow("SobelT"), "replicate");

            var result = ArrOp.ImageArrayToUint8(Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By));

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = result[y, x];
                    int green = result[y, x];
                    int blue = result[y, x];
                    image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            image.Save("4.jpg");
        }

        //variant 2
        public static void ImageOutlinesV(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height);

            var gray = Helpers.rgbToGrayArray(img);

            var GRx = Filter.filter_double(ArrOp.ArrayToDouble(gray), Filter.Dx3FWindow("Sobel"), "replicate");
            var GRy = Filter.filter_double(ArrOp.ArrayToDouble(gray), Filter.Dx3FWindow("SobelT"), "replicate");

            var GRG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(GRx, 2), ArrOp.PowArrayElements(GRy, 2)));

            var PGRG = ArrOp.ArrayToUint8(GRG);
            //var PGRG = ArrOp.ArrayToUint(ArrOp.ArrayDivByConst(GRG, GRG.Cast<double>().Max()), 255); //thin

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int red = PGRG[y, x];
                    int green = PGRG[y, x];
                    int blue = PGRG[y, x];
                    image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            image.Save("5.jpg");
        }

        //variant 3
        public static void ImageOutlinesVCut(Bitmap img)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(img.Width, img.Height);

            var gray = Helpers.rgbToGrayArray(img);

            var GRx = Filter.filter_int(gray, Filter.Ix3FWindow("Sobel"), "replicate");
            var GRy = Filter.filter_int(gray, Filter.Ix3FWindow("SobelT"), "replicate");

            var GRG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(GRx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(GRy), 2)));

            var PGRG = ArrOp.ArrayToUint8(GRG);
            //var PGRG = ArrOp.ArrayToUint(ArrOp.ArrayDivByConst(GRG, GRG.Cast<double>().Max()), 255); //thin

            for (int y = 0; y < img.Height; y++)
            {
                for (int x = 0; x < img.Width; x++)
                {
                    int red = PGRG[y, x];
                    int green = PGRG[y, x];
                    int blue = PGRG[y, x];
                    image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            image.Save("6.jpg");
        }

        //Variant 4
        public static void ImageColorOutlines(int width, int height, int[,] Rc, int[,] Gc, int[,] Bc)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height);

            var Rx = Filter.filter_double(ArrOp.ArrayToDouble(Rc), Filter.Dx3FWindow("Sobel"), "replicate");
            var Ry = Filter.filter_double(ArrOp.ArrayToDouble(Rc), Filter.Dx3FWindow("SobelT"), "replicate");

            var Gx = Filter.filter_double(ArrOp.ArrayToDouble(Gc), Filter.Dx3FWindow("Sobel"), "replicate");
            var Gy = Filter.filter_double(ArrOp.ArrayToDouble(Gc), Filter.Dx3FWindow("SobelT"), "replicate");

            var Bx = Filter.filter_double(ArrOp.ArrayToDouble(Bc), Filter.Dx3FWindow("Sobel"), "replicate");
            var By = Filter.filter_double(ArrOp.ArrayToDouble(Bc), Filter.Dx3FWindow("SobelT"), "replicate");

            var RG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Rx, 2), ArrOp.PowArrayElements(Ry, 2)));
            var GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Gx, 2), ArrOp.PowArrayElements(Gy, 2)));
            var BG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Bx, 2), ArrOp.PowArrayElements(By, 2)));

            var PRG = ArrOp.ArrayToUint8(RG);
            var PGG = ArrOp.ArrayToUint8(GG);
            var PBG = ArrOp.ArrayToUint8(BG);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = PRG[y, x];
                    int green = PGG[y, x];
                    int blue = PBG[y, x];
                    image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            image.Save("7.jpg");
        }

        //Variant 5
        public static void ImageColorOutlinesCut(int width, int height, int[,] Rc, int[,] Gc, int[,] Bc)
        {
            ArrayOperations ArrOp = new ArrayOperations();
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height);

            var Rx = Filter.filter_int(Rc, Filter.Ix3FWindow("Sobel"), "replicate");
            var Ry = Filter.filter_int(Rc, Filter.Ix3FWindow("SobelT"), "replicate");

            var Gx = Filter.filter_int(Gc, Filter.Ix3FWindow("Sobel"), "replicate");
            var Gy = Filter.filter_int(Gc, Filter.Ix3FWindow("SobelT"), "replicate");

            var Bx = Filter.filter_int(Bc, Filter.Ix3FWindow("Sobel"), "replicate");
            var By = Filter.filter_int(Bc, Filter.Ix3FWindow("SobelT"), "replicate");

            var RG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Rx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Ry), 2)));
            var GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gy), 2)));
            var BG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Bx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(By), 2)));

            var PRG = ArrOp.ArrayToUint8(RG);
            var PGG = ArrOp.ArrayToUint8(GG);
            var PBG = ArrOp.ArrayToUint8(BG);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int red = PRG[y, x];
                    int green = PGG[y, x];
                    int blue = PBG[y, x];
                    image.SetPixel(x, y, Color.FromArgb(red, green, blue));
                }
            }
            image.Save("8.jpg");
        }*/
        #endregion
    }

    public enum CountourVariant
    {
        Variant1_BW  = 1,
        Variant2_BW  = 2,
        Variant3_BW  = 3,
        Variant4_BW  = 4,
        Variant5_RGB = 5,
        Variant6_RGB = 6
    }
}
