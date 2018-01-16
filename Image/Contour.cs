using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    public static class Contour
    {
        ////only for RGB images, b&w 24bbp. (what about8bpp? - colored variant dont work for them)
        public static void GlobalContour(Bitmap img, CountourVariant Variant, string ImgExtension)
        {
            //create new class object for array operations
            ArrayOperations ArrOp = new ArrayOperations();

            //image parameters
            int width = img.Width;
            int height = img.Height;
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(width, height, PixelFormat.Format24bppRgb);

            //obtain color components
            var ColorList = Helpers.GetPixels(img);
            var Redcolor = ColorList[0].Color;
            var Greencolor = ColorList[1].Color;
            var Blueccolor = ColorList[2].Color;

            //array, where store color components result after operations
            int[,] resultR = new int[height, width];
            int[,] resultG = new int[height, width];
            int[,] resultB = new int[height, width];
            string outName = String.Empty;

            //variants 1-4 black & white. Variants 5, 6 - colored
            if (Variant == CountourVariant.Variant1_BW || Variant == CountourVariant.Variant5_RGB || Variant == CountourVariant.Variant2_BW)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with double
                var Rx = Filter.Filter_double(Redcolor, "Sobel");
                var Ry = Filter.Filter_double(ArrOp.ArrayToDouble(Redcolor), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                var Gx = Filter.Filter_double(ArrOp.ArrayToDouble(Greencolor), Filter.Dx3FWindow("Sobel"), PadType.replicate);
                var Gy = Filter.Filter_double(ArrOp.ArrayToDouble(Greencolor), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                var Bx = Filter.Filter_double(ArrOp.ArrayToDouble(Blueccolor), Filter.Dx3FWindow("Sobel"), PadType.replicate);
                var By = Filter.Filter_double(ArrOp.ArrayToDouble(Blueccolor), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                if (Variant == CountourVariant.Variant1_BW)
                {
                    //gradient for one color component B&W result
                    resultR = ArrOp.ImageArrayToUint8(Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By));
                    resultG = resultR; resultB = resultR; //Black & White result
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV1" + ImgExtension;
                }
                else if (Variant == CountourVariant.Variant2_BW)
                {
                    //gradient for one color component B&W result
                    resultR = ArrOp.ImageArrayToUint8(ArrOp.SumArrays(Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By), Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By)));
                    resultG = resultR; resultB = resultR; //Black & White result
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV2" + ImgExtension;
                }
                else
                {
                    //RGB gradients
                    var RG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Rx, 2), ArrOp.PowArrayElements(Ry, 2))); //R gradient
                    var GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Gx, 2), ArrOp.PowArrayElements(Gy, 2))); //G gradient
                    var BG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Bx, 2), ArrOp.PowArrayElements(By, 2))); //B gradient

                    resultR = ArrOp.ArrayToUint8(RG);
                    resultG = ArrOp.ArrayToUint8(GG);
                    resultB = ArrOp.ArrayToUint8(BG);
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV5" + ImgExtension;
                }
            }

            else if (Variant == CountourVariant.Variant3_BW || Variant == CountourVariant.Variant4_BW)
            {
                //convert image inti gray scale
                var gray = Helpers.RGBToGrayArray(img);
                double[,] GG = new double[height, width]; //gray gradient

                if (Variant == CountourVariant.Variant3_BW)
                {
                    var Gx = Filter.Filter_double(ArrOp.ArrayToDouble(gray), Filter.Dx3FWindow("Sobel"), PadType.replicate);
                    var Gy = Filter.Filter_double(ArrOp.ArrayToDouble(gray), Filter.Dx3FWindow("SobelT"), PadType.replicate);

                    GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(Gx, 2), ArrOp.PowArrayElements(Gy, 2)));
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV3" + ImgExtension;
                }
                else
                {
                    var Gx = Filter.Filter_int(gray, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                    var Gy = Filter.Filter_int(gray, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                    GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gy), 2)));
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV4" + ImgExtension;
                }

                resultR = ArrOp.ArrayToUint8(GG); resultG = resultR; resultB = resultR;
            }
            else if (Variant == CountourVariant.Variant6_RGB)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with int
                var Rx = Filter.Filter_int(Redcolor, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                var Ry = Filter.Filter_int(Redcolor, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                var Gx = Filter.Filter_int(Greencolor, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                var Gy = Filter.Filter_int(Greencolor, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                var Bx = Filter.Filter_int(Blueccolor, Filter.Ix3FWindow("Sobel"), PadType.replicate);
                var By = Filter.Filter_int(Blueccolor, Filter.Ix3FWindow("SobelT"), PadType.replicate);

                var RG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Rx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Ry), 2))); //R gradient
                var GG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Gy), 2))); //G gradient
                var BG = ArrOp.SqrtArrayElements(ArrOp.SumArrays(ArrOp.PowArrayElements(ArrOp.ArrayToDouble(Bx), 2), ArrOp.PowArrayElements(ArrOp.ArrayToDouble(By), 2))); //B gradient

                resultR = ArrOp.ArrayToUint8(RG);
                resultG = ArrOp.ArrayToUint8(GG);
                resultB = ArrOp.ArrayToUint8(BG);
                outName = Directory.GetCurrentDirectory() + "\\Contour\\imgOutlineV6" + ImgExtension;
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            //dont forget, that directory Contour must exist. Later add if not exist - creat
            //image.Save(outName);
            Helpers.SaveOptions(image, outName, ImgExtension);
        }
    }

    public enum CountourVariant
    {
        Variant1_BW = 1,
        Variant2_BW = 2,
        Variant3_BW = 3,
        Variant4_BW = 4,
        Variant5_RGB = 5,
        Variant6_RGB = 6
    }
}
