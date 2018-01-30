using System;
using System.IO;
using System.Drawing;
using Image.ArrayOperations;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace Image
{
    public static class Contour
    {
        ////only for RGB images, b&w 24bbp. (what about8bpp? - colored variant dont work for them)
        public static void GlobalContour(Bitmap img, CountourVariant Variant, string fileName)
        {
            string ImgExtension = Path.GetExtension(fileName).ToLower();
            fileName = Path.GetFileNameWithoutExtension(fileName);
            string defPass = Directory.GetCurrentDirectory() + "\\Contour\\";
            Checks.DirectoryExistance(defPass);

            //new bitmap    
            Bitmap image = new Bitmap(img.Width, img.Height, PixelFormat.Format24bppRgb);
            //array, where store color components result after operations
            int[,] resultR = new int[img.Height, img.Width];
            int[,] resultG = new int[img.Height, img.Width];
            int[,] resultB = new int[img.Height, img.Width];
            //filtered values storage
            List<ArraysListDouble> filt = new List<ArraysListDouble>();

            string outName = String.Empty;
            bool type = true;

            //obtain color components. form 8bpp works too, but not recommended to use 8-bit .jpeg\tif\jpg images
            var ColorList = Helpers.GetPixels(img);
            var Redcolor   = ColorList[0].Color;
            var Greencolor = ColorList[1].Color;
            var Bluecolor  = ColorList[2].Color;
            double Depth = System.Drawing.Image.GetPixelFormatSize(img.PixelFormat);

            if (Depth == 8) { ImgExtension = ".png"; }
            if (Depth == 8 || Checks.BlackandWhite24bppCheck(img))
            { type = false; }

            //variants 1-4 black & white. Variants 5, 6 - colored
            if (Variant == CountourVariant.Variant1_BW || Variant == CountourVariant.Variant5_RGB || Variant == CountourVariant.Variant2_BW)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with double
                if (!type)
                {
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Redcolor, "Sobel") }); //b&w x
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Redcolor, "SobelT") }); //b&w y                 
                }
                else
                {                
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Redcolor, "Sobel") }); //Rx
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Redcolor, "SobelT") }); //Ry
                    
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Greencolor, "Sobel") }); //Gx
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Greencolor, "SobelT") }); //Gy
                   
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Bluecolor, "Sobel") }); //Bx
                    filt.Add(new ArraysListDouble() { Color = Filter.Filter_double(Bluecolor, "SobelT") }); //By
                }

                if (Variant == CountourVariant.Variant1_BW)
                {
                    //gradient for one color component B&W result
                    if (!type)
                    {              
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color).ImageArrayToUint8();
                    }
                    else
                    {         
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color, filt[2].Color,
                            filt[3].Color, filt[4].Color, filt[5].Color).ImageArrayToUint8();
                    }
                    resultG = resultR; resultB = resultR; //Black & White result
                    outName = defPass + fileName + "_ContourV1" + ImgExtension;
                }
                else if (Variant == CountourVariant.Variant2_BW)
                {
                    //gradient for one color component B&W result
                    if (!type)
                    {
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color).ArrayMultByConst(2).ImageArrayToUint8();     
                    }
                    else
                    {                       
                        resultR = Gradient.Grad(filt[0].Color, filt[1].Color, filt[2].Color,
                           filt[3].Color, filt[4].Color, filt[5].Color).ArrayMultByConst(2).ImageArrayToUint8();
                    }
                    resultG = resultR; resultB = resultR; //Black & White result
                    outName = defPass + fileName + "_ContourV2" + ImgExtension;
                }
                else
                {
                    if (type)
                    {
                        //RGB gradients       
                        var RG = (filt[0].Color).PowArrayElements(2).SumArrays((filt[1].Color).PowArrayElements(2)).SqrtArrayElements(); //R gradient               
                        var GG = (filt[2].Color).PowArrayElements(2).SumArrays((filt[3].Color).PowArrayElements(2)).SqrtArrayElements(); //G gradient                   
                        var BG = (filt[4].Color).PowArrayElements(2).SumArrays((filt[5].Color).PowArrayElements(2)).SqrtArrayElements(); //B gradient

                        resultR = RG.ArrayToUint8(); resultG = GG.ArrayToUint8(); resultB = BG.ArrayToUint8();
                        outName = defPass + fileName + "_ContourV5" + ImgExtension;
                    }
                    else
                    {
                        Console.WriteLine("Need RGB image for Variant5_RGB as input. Contour Method.");
                        return;
                    }
                }
            }

            else if (Variant == CountourVariant.Variant3_BW || Variant == CountourVariant.Variant4_BW)
            {
                //convert image into gray scale
                var gray = MoreHelpers.BlackandWhiteProcessHelper(img);

                double[,] GG = new double[img.Height, img.Height]; //gray gradient

                if (Variant == CountourVariant.Variant3_BW)
                {
                    var Gx = Filter.Filter_double(gray, "Sobel");
                    var Gy = Filter.Filter_double(gray, "SobelT");

                    GG = Gx.PowArrayElements(2).SumArrays(Gy.PowArrayElements(2)).SqrtArrayElements();
                    outName = defPass + fileName + "_ContourV3" + ImgExtension;
                }
                else
                {
                    var Gx = Filter.Filter_int(gray, "Sobel");
                    var Gy = Filter.Filter_int(gray, "SobelT");

                    GG = Gx.ArrayToDouble().PowArrayElements(2).SumArrays(Gy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements();
                    outName = defPass + fileName + "_ContourV4" + ImgExtension;
                }

                resultR = GG.ArrayToUint8(); resultG = resultR; resultB = resultR;
            }
            else if (Variant == CountourVariant.Variant6_RGB)
            {
                //using filter and array operations count RGB values in 2d dimentions x and y for variants with int
                if (type)
                {
                    var Rix = Filter.Filter_int(Redcolor, "Sobel");
                    var Riy = Filter.Filter_int(Redcolor, "SobelT");

                    var Gix = Filter.Filter_int(Greencolor, "Sobel");
                    var Giy = Filter.Filter_int(Greencolor, "SobelT");

                    var Bix = Filter.Filter_int(Bluecolor, "Sobel");
                    var Biy = Filter.Filter_int(Bluecolor, "SobelT");

                    var RG = Rix.ArrayToDouble().PowArrayElements(2).SumArrays(Riy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //R gradient               
                    var GG = Gix.ArrayToDouble().PowArrayElements(2).SumArrays(Giy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //G gradient             
                    var BG = Bix.ArrayToDouble().PowArrayElements(2).SumArrays(Biy.ArrayToDouble().PowArrayElements(2)).SqrtArrayElements(); //B gradient

                    resultR = RG.ArrayToUint8(); resultG = GG.ArrayToUint8(); resultB = BG.ArrayToUint8();
                    outName = defPass + fileName + "_ContourV6" + ImgExtension;
                }
                else
                {
                    Console.WriteLine("Need RGB image for Variant5_RGB as input. Contour Method.");
                    return;
                }
            }

            image = Helpers.SetPixels(image, resultR, resultG, resultB);
            outName = Checks.OutputFileNames(outName);

            if (Depth == 8)
            { image = MoreHelpers.Bbp24Gray2Gray8bppHelper(image); }

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
