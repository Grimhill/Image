using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

namespace Image
{
    public static class Contour
    {
        ////only for RGB images, b&w 24bbp. (what about8bpp? - colored variant dont work for them)
        public static void GlobalContour(Bitmap img, CountourVariant Variant, string fileName)
        {
            //declarations          
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            MoreHelpers.DirectoryExistance(Directory.GetCurrentDirectory() + "\\Contour");

            //new bitmap    
            Bitmap image = new Bitmap(img.Width, img.Height);

            //obtain color components
            var ColorList = Helpers.GetPixels(img);
            var Redcolor   = ColorList[0].Color;
            var Greencolor = ColorList[1].Color;
            var Bluecolor  = ColorList[2].Color;

            //array, where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            string outName = String.Empty;

            //variants 1-4 black & white. Variants 5, 6 - colored
            if (Variant == CountourVariant.Variant1_BW || Variant == CountourVariant.Variant5_RGB || Variant == CountourVariant.Variant2_BW)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with double
                var Rx = Filter.Filter_double(Redcolor, "Sobel");
                var Ry = Filter.Filter_double(Redcolor, "SobelT");

                var Gx = Filter.Filter_double(Greencolor, "Sobel");
                var Gy = Filter.Filter_double(Greencolor, "SobelT");

                var Bx = Filter.Filter_double(Bluecolor, "Sobel");
                var By = Filter.Filter_double(Bluecolor, "SobelT");

                if (Variant == CountourVariant.Variant1_BW)
                {
                    //gradient for one color component B&W result
                    resultR = Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By).ImageArrayToUint8();
                    resultG = resultR; resultB = resultR; //Black & White result
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\" + fileName + "_ContourV1" + ImgExtension;
                }
                else if (Variant == CountourVariant.Variant2_BW)
                {
                    //gradient for one color component B&W result                    
                    resultR = Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By).SumArrays(Gradient.Grad(Rx, Ry, Gx, Gy, Bx, By)).ImageArrayToUint8();
                    resultG = resultR; resultB = resultR; //Black & White result
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\" + fileName + "_ContourV2" + ImgExtension;
                }
                else
                {
                    //RGB gradients                    
                    var RG = Rx.PowArrayElements(2).SumArrays(Ry.PowArrayElements(2)).SqrtArrayElements(); //R gradient                    
                    var GG = Gx.PowArrayElements(2).SumArrays(Gy.PowArrayElements(2)).SqrtArrayElements(); //G gradient                   
                    var BG = Bx.PowArrayElements(2).SumArrays(By.PowArrayElements(2)).SqrtArrayElements(); //B gradient

                    resultR = RG.ArrayToUint8(); 
                    resultG = GG.ArrayToUint8(); 
                    resultB = BG.ArrayToUint8(); 
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\" + fileName + "_ContourV5" + ImgExtension;
                }
            }

            else if (Variant == CountourVariant.Variant3_BW || Variant == CountourVariant.Variant4_BW)
            {
                //convert image into gray scale
                var gray = Helpers.RGBToGrayArray(img);
                double[,] GG = new double[img.Height, img.Height]; //gray gradient

                if (Variant == CountourVariant.Variant3_BW)
                {                   
                    var Gx = Filter.Filter_double(gray, "Sobel");                    
                    var Gy = Filter.Filter_double(gray, "SobelT");
                   
                    GG = Gx.PowArrayElements(2).SumArrays(Gy.PowArrayElements(2)).SqrtArrayElements();
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\" + fileName + "_ContourV3" + ImgExtension;
                }
                else
                {                    
                    var Gx = Filter.Filter_int(gray, "Sobel");                    
                    var Gy = Filter.Filter_int(gray, "SobelT");
                    
                    GG = Gx.ArrayToDouble().PowArrayElements(2).SumArrays(Gy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements();
                    outName = Directory.GetCurrentDirectory() + "\\Contour\\" + fileName + "_ContourV4" + ImgExtension;
                }

                resultR = GG.ArrayToUint8(); resultG = resultR; resultB = resultR;
            }
            else if (Variant == CountourVariant.Variant6_RGB)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with int
                var Rx = Filter.Filter_int(Redcolor, "Sobel");
                var Ry = Filter.Filter_int(Redcolor, "SobelT");

                var Gx = Filter.Filter_int(Greencolor, "Sobel");
                var Gy = Filter.Filter_int(Greencolor, "SobelT");

                var Bx = Filter.Filter_int(Bluecolor, "Sobel");
                var By = Filter.Filter_int(Bluecolor, "SobelT");
                                
                var RG = Rx.ArrayToDouble().PowArrayElements(2).SumArrays(Ry.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //R gradient                
                var GG = Gx.ArrayToDouble().PowArrayElements(2).SumArrays(Gy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //G gradient                
                var BG = Bx.ArrayToDouble().PowArrayElements(2).SumArrays(By.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //B gradient

                resultR = RG.ArrayToUint8();
                resultG = GG.ArrayToUint8();
                resultB = BG.ArrayToUint8();
                outName = Directory.GetCurrentDirectory() + "\\Contour\\" + fileName + "_ContourV6" + ImgExtension;
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);

            outName = MoreHelpers.OutputFileNames(outName);

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
